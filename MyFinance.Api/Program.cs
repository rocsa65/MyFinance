using Microsoft.EntityFrameworkCore;
using MyFinance.Core.Interfaces;
using MyFinance.Core.Services;
using MyFinance.Infrastructure.Data;
using MyFinance.Infrastructure.Repositories;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// --- register DbContext first ---
builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    // In non-testing environment read connection string from config
    if (builder.Environment.IsEnvironment("Testing"))
    {
        var testFile = builder.Configuration["TestingSqlite:FilePath"]
                       ?? Environment.GetEnvironmentVariable("TEST_SQLITE_FILE")
                       ?? Path.Combine(Path.GetTempPath(), "MyFinance.Testing.db");
        options.UseSqlite($"Data Source={testFile}");
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Add health checks AFTER DbContext registration
builder.Services.AddHealthChecks()
    .AddCheck<DbContextHealthCheck<FinanceDbContext>>("FinanceDbContext");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

// Register repositories and services for DI
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map health checks
app.MapHealthChecks("/health");

// Map controllers so attribute routes like "api/account" are exposed
app.MapControllers();

app.Run();

// Make the implicit Program class accessible to integration tests
public partial class Program { }