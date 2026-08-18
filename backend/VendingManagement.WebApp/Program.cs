using Microsoft.EntityFrameworkCore;
using VendingManagement.WebApp.Data;
using VendingManagement.WebApp.Services;
using VendingManagement.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHttpClient();

var app = builder.Build();

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
