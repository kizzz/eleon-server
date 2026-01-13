using EleonsoftSdk.modules.HealthCheck.Module.Api;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Configuration;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.System;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Delivery;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.General.BackgroundExecution;
using Eleon.HealthChecks.Lib.HealthChecks.General;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

/// <summary>
/// New registration extensions for the redesigned health checks architecture.
/// These can coexist with the old extensions during migration.
/// </summary>
public static class HealthCheckExtensionsV2
{
    public static string HealthChecksConfigurationSectionName = "HealthChecks";

    /// <summary>
    /// Registers core health check infrastructure.
    /// </summary>
    public static IServiceCollection AddEleonHealthChecksCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<HealthCheckOptions>(configuration.GetSection(HealthChecksConfigurationSectionName));
        services.Configure<HealthCheckOptions>(opt => 
            opt.ApplicationName = configuration.GetValue<string>("ApplicationName") ?? "Undefined");

        var options = configuration.GetSection(HealthChecksConfigurationSectionName).Get<HealthCheckOptions>() 
            ?? new HealthCheckOptions();

        // Backward-compatible middleware (IMiddleware requires DI registration)
        services.AddTransient<EleonsoftHealthCheckMiddleware>();

        // Core infrastructure
        services.AddSingleton<IHealthSnapshotStore, InMemoryHealthSnapshotStore>();
        services.AddSingleton<IHealthReportBuilder>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<HealthReportBuilder>>();
            return new HealthReportBuilder(logger, options.ApplicationName);
        });
        services.AddSingleton<IHealthRunCoordinator>(sp =>
        {
            var healthCheckService = sp.GetRequiredService<HealthCheckService>();
            var snapshotStore = sp.GetRequiredService<IHealthSnapshotStore>();
            var reportBuilder = sp.GetRequiredService<IHealthReportBuilder>();
            var logger = sp.GetRequiredService<ILogger<HealthRunCoordinator>>();
            return new HealthRunCoordinator(healthCheckService, snapshotStore, reportBuilder, logger, options.ApplicationName);
        });

        // Publishers
        // Register IHealthCheckService with default implementation (required by HttpHealthPublisher)
        services.TryAddSingleton<IHealthCheckService, EmptyHealthChecksService>();
        services.TryAddSingleton<HealthCheckManager>();
        services.AddSingleton<IHealthPublisher, HttpHealthPublisher>();
        services.Configure<HealthPublishingOptions>(configuration.GetSection($"{HealthChecksConfigurationSectionName}:Publishing"));
        services.AddHostedService<HealthPublishingService>();

        // Run an initial health check once the host starts (parity with legacy registration)
        services.AddSingleton<OnStartHealthCheckService>();
        services.AddHostedService<OnStartHealthCheckService>();

        // Restart service
        services.AddTransient<RestartService>();

        return services;
    }

    /// <summary>
    /// Registers SQL Server health checks (readiness + diagnostics).
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksSqlServer(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var sqlOptions = configuration.GetSection($"{HealthChecksConfigurationSectionName}:SqlServer")
            .Get<SqlServerHealthCheckOptions>() ?? new SqlServerHealthCheckOptions();

        // Readiness check (always enabled, safe)
        builder.AddCheck<SqlServerReadinessHealthCheck>(
            "sql-readiness",
            tags: new[] { "ready", "live" });

        // Diagnostics checks (only if enabled)
        if (sqlOptions.EnableDiagnostics)
        {
            builder.AddCheck<SqlServerDiagnosticsHealthCheck>("sql-diag", tags: new[] { "diag" });
        }

        return builder;
    }

    /// <summary>
    /// Registers HTTP health checks.
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksHttp(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var httpSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:HttpCheck");
        if (!httpSection.Exists())
            return builder;

        // Configure HttpClient
        var clientBuilder = builder.Services.AddHttpClient(HttpHealthCheck.DefaultHealthCheckClientName)
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(httpSection.GetValue(nameof(HttpHealthCheckOptions.Timeout), 40));
            });

        if (httpSection.GetValue(nameof(HttpHealthCheckOptions.IgnoreSsl), false))
        {
            clientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
        }

        // Register check
        builder.AddCheck<HttpHealthCheckV2>("http", tags: new[] { "ready" });

        return builder;
    }

    /// <summary>
    /// Registers environment health checks (CPU, memory, disk, process).
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksEnvironment(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var envSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:EnvironmentCheck");
        if (envSection.Exists())
        {
            builder.Services.Configure<EnvironmentHealthCheckOptions>(envSection);
            builder.AddCheck<EnvironmentHealthCheckV2>("environment", tags: new[] { "ready" });
        }

        var processSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:CurrentProcessCheck");
        if (processSection.Exists())
        {
            builder.Services.Configure<CurrentProcessHealthCheckOptions>(processSection);
            builder.AddCheck<CurrentProcessHealthCheckV2>("current-process", tags: new[] { "diag" });
        }

        var diskSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:DiskSpaceCheck");
        if (diskSection.Exists())
        {
            builder.Services.Configure<DiskSpaceHealthCheckOptions>(diskSection);
            builder.AddCheck<DiskSpaceHealthCheckV2>("disk-space", tags: new[] { "diag" });
        }

        return builder;
    }

    /// <summary>
    /// Registers configuration health check.
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksConfiguration(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var configSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:ConfigurationCheck");
        if (configSection.Exists())
        {
            // Register CheckConfigurationService if not already registered
            builder.Services.AddCheckConfiguration(configSection);
            builder.AddCheck<ConfigurationHealthCheckV2>("configuration", tags: new[] { "ready" });
        }

        return builder;
    }

    /// <summary>
    /// Registers system health checks (log sinks).
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksSystem(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        // System log check doesn't require configuration, but we check if it should be enabled
        var systemSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:SystemCheck");
        var enabled = systemSection.GetValue<bool>("Enabled", true);
        
        if (enabled)
        {
            builder.AddCheck<SystemLogHealthCheckV2>("system-log", tags: new[] { "diag" });
        }

        return builder;
    }

    /// <summary>
    /// Convenience method to register all health checks.
    /// </summary>
    public static IHealthChecksBuilder AddEleonHealthChecksAll(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        return builder
            .AddEleonHealthChecksSqlServer(configuration)
            .AddEleonHealthChecksHttp(configuration)
            .AddEleonHealthChecksEnvironment(configuration)
            .AddEleonHealthChecksConfiguration(configuration)
            .AddEleonHealthChecksSystem(configuration);
    }

    /// <summary>
    /// Maps health check endpoints.
    /// </summary>
    public static IEndpointRouteBuilder UseEleonHealthChecksEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthCheckEndpoints();
        return endpoints;
    }
}
