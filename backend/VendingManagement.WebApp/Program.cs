using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Services.Implementations;
using VendingManagement.BLL.Clients;
using Serilog;

using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.DAL.UOW.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using VendingManagement.BLL.Caching;

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

builder.WebHost.UseUrls("http://localhost:5245", "https://localhost:7142");

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

builder.Services.AddScoped<IProcessingFeeService, ProcessingFeeService>();
builder.Services.AddScoped<ISecurityModuleClient, SecurityModuleClient>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtSecret = builder.Configuration["Jwt:Secret"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

var app = builder.Build();

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
