using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
public static class CheckConfigurationExtensions
{
  public static IServiceCollection AddCheckConfiguration(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<CheckConfigurationOptions>(configuration);
    services.AddTransient<CheckConfigurationMiddleware>();
    services.AddSingleton<CheckConfigurationService>();

    return services;
  }

  public static IApplicationBuilder UseCheckConfigurationMiddleware(this IApplicationBuilder app)
  {
    return app.UseMiddleware<CheckConfigurationMiddleware>();
  }
}
