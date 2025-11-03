using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinance.Core.Interfaces;
using MyFinance.Core.Services;
using MyFinance.Infrastructure.Data;
using MyFinance.Infrastructure.Repositories;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

// Conditional DB registration for tests or normal runs
if (builder.Environment.IsEnvironment("Testing"))
{
    // Prefer explicit file path from config or env var so tests can control it
    var testFile = builder.Configuration["TestingSqlite:FilePath"]
                   ?? Environment.GetEnvironmentVariable("TEST_SQLITE_FILE")
                   ?? Path.Combine(Path.GetTempPath(), "MyFinance.Testing.db");

    var connectionString = $"Data Source={testFile}";
    builder.Services.AddDbContext<FinanceDbContext>(options =>
    {
        options.UseSqlite(connectionString);
    });
}
else
{
    // Normal registration (dev/prod)
    builder.Services.AddDbContext<FinanceDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
}

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

// Map controllers so attribute routes like "api/account" are exposed
app.MapControllers();

app.Run();

// Make the implicit Program class accessible to integration tests
public partial class Program { }
