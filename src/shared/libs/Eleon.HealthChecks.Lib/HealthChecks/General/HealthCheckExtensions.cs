using Eleon.HealthChecks.Lib.HealthChecks.General;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.CheckDatabase;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.System;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using EleonsoftSdk.modules.HealthCheck.Module.General.BackgroundExecution;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckDatabase;
using HealthChecks.UI.Client;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SharedModule.modules.HealthCheck.Module.General;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

public static class HealthCheckExtensions
{
  public static string HealthChecksConfigurationSectionName = "HealthChecks";

  public static IServiceCollection AddEleonHealthChecks(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<HealthCheckOptions>(configuration.GetSection(HealthChecksConfigurationSectionName));
    services.Configure<HealthCheckOptions>(opt => opt.ApplicationName = configuration.GetValue<string>("ApplicationName") ?? "Undefined");
    services.AddSingleton<HealthCheckManager>();
    services.AddSingleton<IHealthCheckService, EmptyHealthChecksService>();

    services.AddSingleton<OnStartHealthCheckService>();
    services.AddHostedService<HealthCheckSendingBackgroundService>();
    services.AddHostedService<OnStartHealthCheckService>();

    services.AddTransient<RestartService>();

    return services;
  }

  public static IServiceCollection AddConfigurationHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddTransient<IEleonsoftHealthCheck, ConfigurationHealthCheck>();
    services.AddCheckConfiguration(configuration.GetSection($"{HealthChecksConfigurationSectionName}:ConfigurationCheck"));
    return services;
  }

  public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<DatabaseMaxTablesSizeOptions>(configuration.GetSection($"{HealthChecksConfigurationSectionName}:DatabaseMaxTablesSizeCheck"));

    services.AddTransient<IEleonsoftHealthCheck, DatabaseHealthCheck>();
    services.AddTransient<IEleonsoftHealthCheck, DatabaseTablesSizeHealthCheck>();
    services.AddTransient<IEleonsoftHealthCheck, DatabaseMaxTablesSizeHealthCheck>();
    return services;
  }

  public static IServiceCollection AddEnvironmentHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<EnvironmentHealthCheckOptions>(configuration.GetSection($"{HealthChecksConfigurationSectionName}:EnvironmentCheck"));
    services.Configure<CurrentProcessHealthCheckOptions>(configuration.GetSection($"{HealthChecksConfigurationSectionName}:CurrentProcessCheck"));
    services.Configure<DiskSpaceHealthCheckOptions>(configuration.GetSection($"{HealthChecksConfigurationSectionName}:DiskSpaceCheck"));
    services.AddSingleton<IEleonsoftHealthCheck, EnvironmentHealthCheck>();
    services.AddSingleton<IEleonsoftHealthCheck, CurrentProccessHealthCheck>();
    services.AddSingleton<IEleonsoftHealthCheck, DiskSpaceHealthCheck>();
    return services;
  }

  public static IServiceCollection AddHealthCheck<T>(this IServiceCollection services) where T : class, IEleonsoftHealthCheck
  {
    services.AddTransient<IEleonsoftHealthCheck, T>();
    return services;
  }

  public static IServiceCollection AddEleonsoftHealthCheckUI(this IServiceCollection services)
  {
    services.AddTransient<EleonsoftHealthCheckMiddleware>();
    return services;
  }

  public static IServiceCollection AddHttpHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    var httpSection = configuration.GetSection($"{HealthChecksConfigurationSectionName}:HttpCheck");

    if (!httpSection.Exists())
      return services;

    services.Configure<HttpHealthCheckOptions>(httpSection);
    var clientBuilder = services.AddHttpClient(HttpHealthCheck.DefaultHealthCheckClientName)
        .ConfigureHttpClient(client =>
        {
          client.Timeout = TimeSpan.FromSeconds(httpSection.GetValue(nameof(HttpHealthCheckOptions.Timeout), 40));
        });

    if (httpSection.GetValue(nameof(HttpHealthCheckOptions.IgnoreSsl), false))
    {
      clientBuilder
          .ConfigurePrimaryHttpMessageHandler(() =>
          {
            return new HttpClientHandler
            {
              // ! Ignores SSL validation
              ServerCertificateCustomValidationCallback =
                          HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
          });
    }

    services.AddTransient<IEleonsoftHealthCheck, HttpHealthCheck>();
    return services;
  }

  public static IServiceCollection AddCommonHealthChecks(this IServiceCollection services, IConfiguration configuration)
  {
    //services.AddMicrosoftHealthChecks(configuration);
    
    // Ensure AddHealthChecks() is called even when AddMicrosoftHealthChecks is disabled
    // This is required for UseHealthChecks() to work in UseEleonsoftHealthChecksMiddleware
    services.AddHealthChecks();
    
    services.AddEleonHealthChecks(configuration);
    services.AddEleonsoftHealthCheckUI();
    services.AddConfigurationHealthCheck(configuration);
    services.AddDatabaseHealthCheck(configuration);
    services.AddEnvironmentHealthCheck(configuration);
    services.AddHttpHealthCheck(configuration);
    services.AddHealthCheck<SystemLogHealthCheck>();

    services.AddWarmupBackgroundService();

    return services;
  }

  public static IApplicationBuilder UseEleonsoftHealthChecksMiddleware(this IApplicationBuilder app)
  {
    app.UseMiddleware<EleonsoftHealthCheckMiddleware>();

    app.UseHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
      Predicate = _ => true,
      ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    });

    app.UseHealthChecksUI(options =>
    {
      //options.UIPath = "/health-ui";
      //options.ApiPath = "/health-ui-api";
    });

    return app;
  }

  public static IServiceCollection AddMicrosoftHealthChecks(this IServiceCollection services, IConfiguration configuration)
  {
    var healthChecksBuilder = services
        .AddHealthChecksUI(setupSettings: setup =>
        {
          setup.SetHeaderText(configuration.GetValue("ApplicationName", "Unknown Application"));
          //setup.AddHealthCheckEndpoint("eleonsoft", "https://localhost:5050/healthz");

          setup.UseApiEndpointHttpMessageHandler(sp =>
              {
                return new HttpClientHandler
                {
                  ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                };
              });

          setup.SetEvaluationTimeInSeconds(30); // Check every 30 seconds
          setup.MaximumHistoryEntriesPerEndpoint(10); // Keep 10 history entries
        });

    //// Use SQL Server storage instead of InMemory due to EF Core 10 incompatibility with HealthChecks.UI 9.0.0
    //// InMemory storage uses EF Core InMemory which has breaking changes in EF Core 10
    //// SQL Server storage uses EF Core SQL Server which is compatible
    //var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren().ToList();
    //if (connectionStrings.Any())
    //{
    //  // Use the first available connection string for HealthChecks UI storage
    //  var healthChecksConnectionString = connectionStrings.First().Value;
    //  healthChecksBuilder.AddSqlServerStorage(healthChecksConnectionString);
    //}
    //else
    //{
    //  // If no connection strings available, HealthChecks UI won't have storage
    //  // This will cause the same error, but at least we tried
    //  // TODO: Consider making HealthChecks UI optional or providing a default connection string
    //}

    var microsoftChecks = healthChecksBuilder.Services.AddHealthChecks();

    foreach (var conString in configuration.GetSection("ConnectionStrings").GetChildren())
    {
      microsoftChecks.AddSqlServer(
            connectionString: conString.Value,
            name: conString.Key,
            tags: new[] { "database", conString.Key });
    }

    microsoftChecks.AddCheck<EnvironmentHealthCheck>("EnvironmentCheck", tags: new[] { "environment" });

    return services;
  }
}
