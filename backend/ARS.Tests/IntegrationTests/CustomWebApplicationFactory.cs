using ARS.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ARS.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private MongoDbRunner? _mongoRunner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Start in-memory MongoDB
            _mongoRunner = MongoDbRunner.Start();

            // Override configuration
            var testConfig = new Dictionary<string, string>
            {
                {"MongoDbSettings:ConnectionString", _mongoRunner.ConnectionString},
                {"MongoDbSettings:DatabaseName", "IntegrationTestDB"},
                {"AzureAd:Instance", "https://login.microsoftonline.com/"},
                {"AzureAd:TenantId", "test-tenant"},
                {"AzureAd:ClientId", "test-client"},
                {"AzureAd:Scopes", "access_as_user"}
            };

            config.AddInMemoryCollection(testConfig!);
        });

        builder.ConfigureServices(services =>
        {
            // Remove authentication for testing
            services.AddAuthentication("Test")
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });
    }

    // Create HttpClient with JSON options configured
    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mongoRunner?.Dispose();
        }
        base.Dispose(disposing);
    }
}