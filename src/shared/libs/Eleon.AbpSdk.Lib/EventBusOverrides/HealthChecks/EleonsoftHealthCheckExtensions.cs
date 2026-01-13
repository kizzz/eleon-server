using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckEventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;
public static class EleonsoftHealthCheckExtensions
{
  public static IServiceCollection AddEventBusHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddTransient<IEleonsoftHealthCheck, EventBusHealthCheck>();
    services.AddTransient<BusHealthEventHandler>();
    return services;
  }

  public static IServiceCollection AddRabbitMqHealthCheck(this IServiceCollection services, IConfiguration configuration)
  {
    var rabbitSection = configuration.GetSection($"{HealthCheckExtensions.HealthChecksConfigurationSectionName}:RabbitMqCheck");

    if (!rabbitSection.Exists())
      return services;

    services.Configure<RabbitManagementOptions>(rabbitSection);
    services.AddTransient<IEleonsoftHealthCheck, RabbitMqQueuesHealthCheck>();
    return services;
  }

  public static IServiceCollection AddEleonsoftHealthChecks(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddTransient<HealthCheckEventHandler>();

    if (configuration.GetValue<bool>($"{HealthCheckExtensions.HealthChecksConfigurationSectionName}:SendHealthChecks", true))
    {
      services.RemoveAll<IHealthCheckService>();
      services.AddTransient<IHealthCheckService, EventBusHealthCheckService>();
    }

    // Migrated to V2 HealthChecks architecture
    // Old: .AddCommonHealthChecks(configuration)
    // Register core infrastructure first
    services.AddEleonHealthChecksCore(configuration);
    // Register all health checks
    services.AddHealthChecks()
        .AddEleonHealthChecksAll(configuration);

    // EventBus and RabbitMQ checks still use old interface (IEleonsoftHealthCheck)
    // These can coexist and will be converted to IHealthCheck in a future update
    return services
        .AddEventBusHealthCheck(configuration)
        .AddRabbitMqHealthCheck(configuration);
  }
}
