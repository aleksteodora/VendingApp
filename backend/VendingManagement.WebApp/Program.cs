using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Services.Implementations;
using VendingManagement.BLL.Clients;

using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.DAL.UOW.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://localhost:5245", "https://localhost:7142");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VendingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VendingConnection")));

builder.Services.AddScoped<IProcessingFeeService, ProcessingFeeService>();
builder.Services.AddScoped<ISecurityModuleClient, SecurityModuleClient>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
