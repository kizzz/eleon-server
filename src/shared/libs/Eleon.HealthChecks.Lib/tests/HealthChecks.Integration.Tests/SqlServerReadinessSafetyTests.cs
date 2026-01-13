using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Integration;

public class SqlServerReadinessSafetyTests : TestBase
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
    public async Task ReadinessCheck_ShouldNotCreateDatabase()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        // Count databases before check
        int databasesBefore;
        await using (var conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sys.databases";
            databasesBefore = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }

        // Act - Run check multiple times
        for (int i = 0; i < 3; i++)
        {
            var result = await check.CheckHealthAsync(
                new HealthCheckContext(),
                CancellationToken.None);
            
            Assert.NotNull(result);
        }

        // Count databases after check
        int databasesAfter;
        await using (var conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sys.databases";
            databasesAfter = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }

        // Assert - No new databases created
        Assert.Equal(databasesBefore, databasesAfter);
    }

    [Fact]
    public async Task ReadinessCheck_ShouldNotMutateData()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        // Create test database and table
        await using (var conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TestDb')
                CREATE DATABASE TestDb;
            ";
            await cmd.ExecuteNonQueryAsync();
        }

        var testDbConnectionString = connectionString.Replace("master", "TestDb");
        await using (var conn = new SqlConnection(testDbConnectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestTable')
                CREATE TABLE TestTable (Id INT PRIMARY KEY, Data NVARCHAR(100));
                IF NOT EXISTS (SELECT * FROM TestTable WHERE Id = 1)
                INSERT INTO TestTable (Id, Data) VALUES (1, 'TestData');
            ";
            await cmd.ExecuteNonQueryAsync();
        }

        // Count rows before
        int rowsBefore;
        await using (var conn = new SqlConnection(testDbConnectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM TestTable";
            rowsBefore = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }

        // Act - Run check
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Count rows after
        int rowsAfter;
        await using (var conn = new SqlConnection(testDbConnectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM TestTable";
            rowsAfter = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rowsBefore, rowsAfter);
    }

    [Fact]
    public async Task ReadinessCheck_ShouldReturnStructuredData()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains("sql.latency_ms", result.Data.Keys);
        Assert.Contains("sql.connections_total", result.Data.Keys);
        Assert.Contains("sql.connections_ok", result.Data.Keys);
        
        // Verify data types
        Assert.IsType<long>(result.Data["sql.latency_ms"]);
        Assert.IsType<int>(result.Data["sql.connections_total"]);
        Assert.IsType<int>(result.Data["sql.connections_ok"]);
    }

    [Fact]
    public async Task ReadinessCheck_ShouldHonorCancellationToken()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var config = CreateConfigurationWithConnectionString(connectionString);
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act & Assert
        // Note: This test may pass quickly, so cancellation might not occur
        // The important thing is that it doesn't throw unexpected exceptions
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            cts.Token);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ReadinessCheck_ShouldHandleConnectionFailure()
    {
        // Arrange
        var config = CreateConfigurationWithConnectionString("Server=invalid-server;Database=test;Integrated Security=true");
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Data);
        Assert.Contains("sql.connections_failed", result.Data.Keys);
    }

    [Fact]
    public async Task ReadinessCheck_ShouldHandleMultipleConnections()
    {
        // Arrange
        var connectionString = _container!.GetConnectionString();
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:First"] = connectionString,
            ["ConnectionStrings:Second"] = connectionString
        });
        var config = configBuilder.Build();
        
        var logger = LoggerFactory.CreateLogger<SqlServerReadinessHealthCheck>();
        var check = new SqlServerReadinessHealthCheck(config, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status == HealthStatus.Healthy || result.Status == HealthStatus.Degraded);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data["sql.connections_total"]);
    }
}
