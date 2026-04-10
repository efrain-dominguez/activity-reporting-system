using ARS.Infrastructure.Data;
using Microsoft.Extensions.Options;
using Mongo2Go;

namespace ARS.Tests.Helpers;

public abstract class TestBase : IDisposable
{
    protected MongoDbRunner _mongoRunner;
    protected MongoDbContext _context;
    protected string _connectionString;

    protected TestBase()
    {
        // Start in-memory MongoDB
        _mongoRunner = MongoDbRunner.Start();
        _connectionString = _mongoRunner.ConnectionString;

        // Create MongoDbSettings
        var settings = new MongoDbSettings
        {
            ConnectionString = _connectionString,
            DatabaseName = "TestDB"
        };

        // Wrap in IOptions
        var options = Options.Create(settings);

        // Create context
        _context = new MongoDbContext(options);
    }

    public void Dispose()
    {
        _mongoRunner?.Dispose();
    }
}