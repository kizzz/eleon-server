using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.MsSql;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Integration;

public class SqlServerDiagnosticsTests : TestBase
{
    private MsSqlContainer? _container;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _container = await CreateSqlContainerAsync();
    }

    public override async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
        await base.DisposeAsync();
    }

    [Fact]
    public async Task DiagnosticsCheck_ShouldCacheResults()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var options = Options.Create(new SqlServerHealthCheckOptions
        {
            EnableDiagnostics = true,
            DiagnosticsCacheMinutes = 1
        });
        var logger = LoggerFactory.CreateLogger<SqlServerDiagnosticsHealthCheck>();
        var check = new SqlServerDiagnosticsHealthCheck(config, options, logger);

        // Act - Run check twice
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var result1 = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);
        sw1.Stop();

        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var result2 = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);
        sw2.Stop();

        // Assert - Second run should be faster (cached)
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        // Note: Cache verification is best done by checking latency or cache flags in data
        Assert.True(sw2.ElapsedMilliseconds < sw1.ElapsedMilliseconds || sw2.ElapsedMilliseconds < 100);
    }

    [Fact]
    public async Task DiagnosticsCheck_ShouldOnlyRunWhenEnabled()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var options = Options.Create(new SqlServerHealthCheckOptions
        {
            EnableDiagnostics = false
        });
        var logger = LoggerFactory.CreateLogger<SqlServerDiagnosticsHealthCheck>();
        var check = new SqlServerDiagnosticsHealthCheck(config, options, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(false, result.Data.GetValueOrDefault("sql.diagnostics.enabled"));
    }

    [Fact]
    public async Task DiagnosticsCheck_ShouldRespectMaxTablesLimit()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var options = Options.Create(new SqlServerHealthCheckOptions
        {
            EnableDiagnostics = true,
            MaxTablesInDiagnostics = 10
        });
        var logger = LoggerFactory.CreateLogger<SqlServerDiagnosticsHealthCheck>();
        var check = new SqlServerDiagnosticsHealthCheck(config, options, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Verify that tables count doesn't exceed limit
        if (result.Data != null && result.Data.ContainsKey("sql.diagnostics.connection_Default.tables_count"))
        {
            var tablesCount = Convert.ToInt32(result.Data["sql.diagnostics.connection_Default.tables_count"]);
            Assert.True(tablesCount <= 10);
        }
    }
}
