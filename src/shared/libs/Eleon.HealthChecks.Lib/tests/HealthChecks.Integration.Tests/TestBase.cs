using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Integration;

/// <summary>
/// Base class for integration tests with Testcontainers support.
/// </summary>
public abstract class TestBase : IAsyncLifetime
{
    protected MsSqlContainer? SqlContainer { get; private set; }
    protected IConfiguration Configuration { get; private set; } = null!;
    protected ILoggerFactory LoggerFactory { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        // Create logger factory
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole().SetMinimumLevel(LogLevel.Warning);
        });

        // Create configuration
        var configBuilder = new ConfigurationBuilder();
        Configuration = configBuilder.Build();
    }

    public virtual async Task DisposeAsync()
    {
        if (SqlContainer != null)
        {
            await SqlContainer.DisposeAsync();
        }

        LoggerFactory?.Dispose();
    }

    protected async Task<MsSqlContainer> CreateSqlContainerAsync()
    {
        SqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();

        await SqlContainer.StartAsync();
        return SqlContainer;
    }

    protected IConfiguration CreateConfigurationWithConnectionString(string connectionString)
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = connectionString
        });
        return configBuilder.Build();
    }
}
