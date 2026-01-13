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
using VPortal.TenantManagement.Module.Middlewares;

namespace abp_sdk.Middlewares;
public static class EleoncoreMultiTenacyExtensions
{
  public static IServiceCollection AddEleoncoreMultiTenancy(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<EleonMultiTenancyOptions>(configuration.GetSection(EleonMultiTenancyOptions.DefaultSectionName));

    if (configuration.GetValue($"{EleonMultiTenancyOptions.DefaultSectionName}:Enabled", true))
    {
      services.AddTransient<IAuthorizationHandler, EleoncoreTenantAuthorizationHandler>();
      services.AddTransient<EleoncoreMutliTenancyMiddleware>();
      services.AddScoped<EleoncoreTenantResolveResultAccessor>();
    }

    return services;
  }

  public static IApplicationBuilder UseEleoncoreMultiTenancy(this IApplicationBuilder app)
  {
    var options = app.ApplicationServices.GetRequiredService<IOptions<EleonMultiTenancyOptions>>();
    if (options.Value?.Enabled == true)
    {
      app.UseMiddleware<EleoncoreMutliTenancyMiddleware>();
    }
    return app;
  }
}
