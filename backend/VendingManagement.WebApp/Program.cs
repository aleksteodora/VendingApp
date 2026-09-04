using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Services.Implementations;
using Serilog;
using System.IO;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.DAL.UOW.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using VendingManagement.BLL.Caching;
using VendingManagement.BLL.Messaging;
using VendingManagement.WebApp.Workers;
using VendingManagement.BLL.Notifications;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//builder.WebHost.UseUrls("http://localhost:5245", "https://localhost:7142");

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VendingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VendingConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

var rabbitHost = builder.Configuration["RabbitMQ:Host"];
var rabbitPort = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672");

builder.Services.AddSingleton<IMessagePublisher>(sp =>
    new RabbitMqPublisher(rabbitHost!, rabbitPort));

builder.Services.AddHostedService<SecurityResponseWorker>();

builder.Services.AddScoped<IProcessingFeeService, ProcessingFeeService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IWebhookNotifier, WebhookNotifier>();

// JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VendingDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Customers.Any())
    {
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "seed-data.sql");

        if (File.Exists(seedFilePath))
        {
            var seedSql = File.ReadAllText(seedFilePath);
            dbContext.Database.ExecuteSqlRaw(seedSql);
        }
    }
}

app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
