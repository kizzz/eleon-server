using abp_sdk.Middlewares;
using EleonsoftSdk.Helpers;
using EleonsoftSdk.Middlewares;
using EleonsoftSdk.Overrides;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedModule.modules.MultiTenancy.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.MultiTenancy;
public static class EleonsoftMultiTenancyExtensions
{
  public static IServiceCollection AddEleonsoftMultiTenancy(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<EleonMultiTenancyOptions>(configuration.GetSection(EleonMultiTenancyOptions.DefaultSectionName));

    if (configuration.GetValue($"{EleonMultiTenancyOptions.DefaultSectionName}:Enabled", true))
    {
      services.AddTransient<IAuthorizationHandler, EleonsoftTenantAuthorizationHandler>();
      services.AddTransient<EleonsoftMultiTenancyMiddleware>();
      services.AddScoped<EleonsoftTenantResolveResultAccessor>();
    }

    return services;
  }

  public static IApplicationBuilder UseEleonsoftMultiTenancy(this IApplicationBuilder app)
  {
    var options = app.ApplicationServices.GetRequiredService<IOptions<EleonMultiTenancyOptions>>();

    if (options.Value?.Enabled == true)
    {
      app.UseMiddleware<EleonsoftMultiTenancyMiddleware>();
    }
    return app;
  }
}
