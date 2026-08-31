using Microsoft.OpenApi.Models;
using SecurityModule.BLL.Services.Implementations;
using SecurityModule.BLL.Services.Interfaces;
using SecurityModule.BLL.Messaging;
using SecurityModule.WebApp.Workers;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5244", "https://localhost:7141");

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<ISecurityModuleService, SecurityModuleService>();
builder.Services.AddSingleton<IMessagePublisher>(sp => new RabbitMqPublisher("localhost"));
builder.Services.AddHostedService<SecurityModuleWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("x-api-key", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key needed to access the endpoints"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "x-api-key"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();