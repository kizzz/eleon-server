using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProxyRouter.Minimal.HttpApi;
using ProxyRouter.Minimal.HttpApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using VPortal.ProxyRouter;
using Volo.Abp.AutoMapper;

namespace Eleon.InternalProxy.App;

[DependsOn(
    )]
public class InternalProxyModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<InternalProxyModule>();
    context.Services.AddAutoMapper(typeof(InternalProxyModule));

    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<InternalProxyModule>(validate: true);
    });
  }

  public override void PostConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();

    context.Services.AddProxyRouter();

    if (configuration.GetValue<bool>("ProxyRouter:UseEvents", true))
    {
      context.Services.AddSingleton<ILocationProvider, EventBusLocationProvider>();
    }
  }

  public override void OnApplicationInitialization(ApplicationInitializationContext context)
  {
    IApplicationBuilder app;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }
    app.UseProxyRouter();
  }
}
