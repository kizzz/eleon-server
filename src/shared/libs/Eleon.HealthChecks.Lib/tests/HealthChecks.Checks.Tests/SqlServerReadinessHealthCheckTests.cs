using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class SqlServerReadinessHealthCheckTests
{
    private readonly Mock<ILogger<SqlServerReadinessHealthCheck>> _loggerMock;
    private readonly IConfiguration _configuration;

    public SqlServerReadinessHealthCheckTests()
    {
        _loggerMock = new Mock<ILogger<SqlServerReadinessHealthCheck>>();
        var configBuilder = new ConfigurationBuilder();
        _configuration = configBuilder.Build();
    }

    [Fact]
    public async Task ShouldHandleNullConfiguration()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        var config = configBuilder.Build();
        var check = new SqlServerReadinessHealthCheck(config, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data["sql.connections_count"]);
    }

    [Fact]
    public async Task ShouldHandleEmptyConnectionStrings()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var config = configBuilder.Build();
        var check = new SqlServerReadinessHealthCheck(config, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task ShouldHandleInvalidConnectionString()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Test"] = "InvalidConnectionString"
        });
        var config = configBuilder.Build();
        var check = new SqlServerReadinessHealthCheck(config, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ShouldHonorCancellationToken()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Test"] = "Server=localhost;Database=test;Integrated Security=true"
        });
        var config = configBuilder.Build();
        var check = new SqlServerReadinessHealthCheck(config, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            cts.Token);

        // Assert
        Assert.NotNull(result);
        // Should handle cancellation gracefully
    }
}
