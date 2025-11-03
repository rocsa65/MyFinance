using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using MyFinance.Infrastructure.Data;
using MyFinance.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MyFinance.IntegrationTests
{
    public class MyTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public MyTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyArray_WhenNoAccounts()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/account");

            // Assert
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<AccountDto[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(accounts);
            Assert.Empty(accounts);
        }

        [Fact(Skip = "Temporarily ignored")]
        public async Task Post_CreateAccount_ThenGetAllContainsIt()
        {
            // Arrange
            var client = _factory.CreateClient();
            var payload = JsonSerializer.Serialize(new
            {
                Name = "Integration Test Account",
                Identifier = "int-1",
                // Currency enum underlying value (e.g., 1 = USD)
                Currency = 1,
                Balance = 123.45m
            });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Act - create
            var postResponse = await client.PostAsync("/api/account", content);

            // Assert create
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            // Act - get all
            var getResponse = await client.GetAsync("/api/account");
            getResponse.EnsureSuccessStatusCode();
            var json = await getResponse.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<AccountDto[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(accounts);
            var created = accounts.SingleOrDefault(a => a.Name == "Integration Test Account" && a.Identifier == "int-1");
            Assert.NotNull(created);
            Assert.Equal(123.45m, created.Balance);
        }
    }

    // Custom factory replaces the real DbContext registration with an in-memory DB for tests
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        public string TestDbFilePath { get; }

        public CustomWebApplicationFactory()
        {
            // create a unique file per factory instance; tests in parallel should isolate by file
            TestDbFilePath = Path.Combine(Path.GetTempPath(), $"MyFinance.Tests.{Guid.NewGuid()}.db");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Tell Program.cs this is a test run
            builder.UseEnvironment("Testing");

            // Provide the test DB file path to the app configuration so Program.cs picks it up
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                var settings = new Dictionary<string, string>
                {
                    ["TestingSqlite:FilePath"] = TestDbFilePath
                };
                conf.AddInMemoryCollection(settings);
            });

            // IMPORTANT: do NOT remove or add DbContext registrations here.
            // Program.cs will register the SQLite provider for the Testing environment.
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // Ensure clean DB schema using the final service provider
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return host;
        }

        public new void Dispose()
        {
            base.Dispose();
            // Optionally delete the DB file after tests complete
            try { if (File.Exists(TestDbFilePath)) File.Delete(TestDbFilePath); } catch { /* ignore */ }
        }
    }
}