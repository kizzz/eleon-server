using Eleoncore.SDK.CoreEvents;
using EleoncoreAspNetCoreSdk.HealthChecks.CheckQueue;
using EleoncoreAspNetCoreSdk.HealthChecks.Overrides;
using Microsoft.Extensions.DependencyInjection;
using ProxyRouter.Minimal.Host;
using ProxyRouter.Minimal.Host.CoreEvents;
using ProxyRouter.Minimal.HttpApi;
using ProxyRouter.Minimal.HttpApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Eleoncore.Proxy.App;

[DependsOn()]
public class EleoncoreProxyModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<EleoncoreProxyModule>();
    context.Services.AddAutoMapper(typeof(EleoncoreProxyModule));
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<EleoncoreProxyModule>(validate: true);
    });

    context.Services.AddQueueScheduling("ProxyQueue", 1000, "*", 30);
    context.Services.AddScoped<IMessageHandler, UpdateCacheMessageHandler>();

    context.Services.AddQueueHealthCheck();
  }

  public override void PostConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddProxyRouter();
    context.Services.AddTransient<ILocationProvider, SdkLocationProvider>();
  }

  public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
  {
    var app = context.GetApplicationBuilder();
    // app.StartScheduleEvents();
  }

  public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
  {
    var app = context.GetApplicationBuilder();
    app.UseProxyRouter();
  }
}
