using Common.EventBus.Module;
using EleonsoftModuleCollector.Commons.Module.BasicNotificators.Helpers;
using EleonsoftSdk.Auth;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.AspNetCore.HttpOverrides;
using ServicesOrchestrator.HealthChecks;
using ServicesOrchestrator.Helpers;
using ServicesOrchestrator.Services;
using ServicesOrchestrator.Services.Abstractions;
using SharedModule.modules.Logging.Module;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace ServicesOrchestrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(CommonEventBusModule)
    )]
public class OrchestratorHostModule : AbpModule
{

}
