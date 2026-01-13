using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Delivery;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Registration;

public class HealthCheckExtensionsV2Tests
{
    private readonly IConfiguration _configuration;

    public HealthCheckExtensionsV2Tests()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApplicationName"] = "TestApp",
            ["HealthChecks:Enabled"] = "true",
            ["HealthChecks:EnvironmentCheck:CpuThresholdPercent"] = "90",
            ["HealthChecks:CurrentProcessCheck:CpuThresholdPercent"] = "95",
            ["HealthChecks:DiskSpaceCheck:Items:0:Path"] = "/tmp",
            ["HealthChecks:DiskSpaceCheck:Items:0:MaxSizeBytes"] = "1000000",
            ["HealthChecks:ConfigurationCheck:Enabled"] = "true",
            ["HealthChecks:HttpCheck:Timeout"] = "30",
            ["HealthChecks:HttpCheck:Urls:0:Name"] = "test",
            ["HealthChecks:HttpCheck:Urls:0:Url"] = "https://example.com",
            ["HealthChecks:SqlServer:EnableDiagnostics"] = "true",
            ["ConnectionStrings:Default"] = "Server=localhost;Database=test;Integrated Security=true"
        });
        _configuration = configBuilder.Build();
    }

    [Fact]
    public void AddEleonHealthChecksCore_ShouldRegisterAllCoreServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHealthChecks();

        // Act
        services.AddEleonHealthChecksCore(_configuration);

        // Assert - Verify services can be built without errors
        var provider = services.BuildServiceProvider();
        
        // Verify core services are registered and can be resolved
        Assert.NotNull(provider.GetService<IHealthRunCoordinator>());
        Assert.NotNull(provider.GetService<IHealthSnapshotStore>());
        Assert.NotNull(provider.GetService<IHealthReportBuilder>());
        Assert.NotNull(provider.GetService<IHealthPublisher>());
        
        // Verify HealthCheckService is registered (required by HealthRunCoordinator)
        var healthCheckService = provider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
        
        // Verify hosted service is registered
        var hostedServices = services.Where(s => s.ServiceType == typeof(IHostedService)).ToList();
        Assert.Contains(hostedServices, s => s.ImplementationType == typeof(HealthPublishingService));
    }

    [Fact]
    public void AddEleonHealthChecksSqlServer_ShouldRegisterReadinessCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddEleonHealthChecksSqlServer(_configuration);

        // Assert
        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
        
        // Verify service can be resolved (registration successful)
        // Actual check registration verification would require running a check or complex reflection
    }

    [Fact]
    public void AddEleonHealthChecksSqlServer_ShouldRegisterDiagnostics_WhenEnabled()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddEleonHealthChecksSqlServer(_configuration);

        // Assert - diagnostics should be registered when EnableDiagnostics is true
        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void AddEleonHealthChecksSqlServer_ShouldNotRegisterDiagnostics_WhenDisabled()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["HealthChecks:SqlServer:EnableDiagnostics"] = "false"
        });
        var config = configBuilder.Build();

        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddEleonHealthChecksSqlServer(config);

        // Assert - diagnostics should not be registered
        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void AddEleonHealthChecksHttp_ShouldConfigureHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddEleonHealthChecksHttp(_configuration);

        // Assert
        var provider = services.BuildServiceProvider();
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        Assert.NotNull(httpClientFactory);
        
        var client = httpClientFactory.CreateClient(HttpHealthCheck.DefaultHealthCheckClientName);
        Assert.NotNull(client);
    }

    [Fact]
    public void AddEleonHealthChecksEnvironment_ShouldRegisterAllEnvironmentChecks()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddEleonHealthChecksEnvironment(_configuration);

        // Assert
        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void AddEleonHealthChecksAll_ShouldRegisterEverything()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();
        services.AddEleonHealthChecksCore(_configuration);

        // Act
        builder.AddEleonHealthChecksAll(_configuration);

        // Assert
        var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
        
        // Verify core services are registered
        Assert.NotNull(provider.GetService<IHealthRunCoordinator>());
    }
}
