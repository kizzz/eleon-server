using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Contract;

/// <summary>
/// Contract tests that enforce SQL Server safety invariants.
/// These tests prevent regressions that could allow DB creation or mutations.
/// </summary>
public class SqlServerSafetyInvariants
{
    [Fact]
    public void ReadinessCheck_ShouldUseHardcodedSafeQuery()
    {
        // Arrange
        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<SqlServerReadinessHealthCheck>>();
        var check = new SqlServerReadinessHealthCheck(config.Object, logger.Object);

        // Act - Use reflection to verify the query is hardcoded
        var field = typeof(SqlServerReadinessHealthCheck).GetField("SafeReadinessQuery", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        var query = field?.GetValue(null) as string;

        // Assert
        Assert.NotNull(query);
        Assert.Contains("DB_NAME()", query);
        Assert.Contains("@@SERVERNAME", query);
        Assert.DoesNotContain("CREATE", query, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ALTER", query, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DROP", query, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("INSERT", query, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UPDATE", query, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DELETE", query, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DiagnosticsCheck_ShouldRequireFeatureFlag()
    {
        // Arrange
        var config = new Mock<IConfiguration>();
        var options = new Microsoft.Extensions.Options.OptionsWrapper<SqlServerHealthCheckOptions>(
            new SqlServerHealthCheckOptions { EnableDiagnostics = false });
        var logger = new Mock<ILogger<SqlServerDiagnosticsHealthCheck>>();
        var check = new SqlServerDiagnosticsHealthCheck(config.Object, options, logger.Object);

        // Act
        var result = check.CheckHealthAsync(
            new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext(),
            CancellationToken.None).Result;

        // Assert
        Assert.Equal(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy, result.Status);
        // Should return early without running diagnostics
    }
}
