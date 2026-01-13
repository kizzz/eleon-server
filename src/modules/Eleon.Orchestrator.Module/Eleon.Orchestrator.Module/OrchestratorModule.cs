using Eleon.Logging.Lib.VportalLogging;
using EleonsoftSdk.Auth;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using ServicesOrchestrator.HealthChecks;
using ServicesOrchestrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using VPortal;

namespace Eleon.Orchestrator.App;

[DependsOn(typeof(AbpAutoMapperModule))]
public class OrchestratorModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    var services = context.Services;
    var configuration = context.Services.GetConfiguration();

    context.Services.AddAutoMapperObjectMapper<OrchestratorModule>();
    services.AddMemoryCache();
    services.AddHttpClient();
    services.AddHttpContextAccessor();

    services.AddHealthCheck<OrchestratingHealthCheck>();


    services.AddOrchestrator(configuration);

    services.AddVportalLogging();
  }

  public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
  {
    var app = context.GetApplicationBuilder();

    app.UseOrchestratorEndpoints("/orchestrator");
  }
}
