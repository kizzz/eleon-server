using Authorization.Module;
using Common.EventBus.Module;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.SystemHealth;
using EleonsoftSdk.modules.Migration.Module;
using Logging.Module;
using Messaging.Module;
using Messaging.Module.Messages;
using Messaging.Module.SystemMessages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using SharedModule.modules.Helpers.Module;
using SharedCollector.deprecated.Messaging.Module.SystemMessages;
using TenantSettings.Module;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EleonsoftSdk;


[DependsOn(typeof(AbpEntityFrameworkCoreModule),typeof(CommonEventBusModule),typeof(TenantSettingsModule),typeof(AuthorizationModule))]
public class EleonsoftSdkModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpDbContextOptions>(options =>
    {
      options.UseSqlServer(
                opt => opt.UseCompatibilityLevel(context.Services.GetConfiguration().GetValue("SqlServer:CompatibilityLevel", 120)));
    });

    context.Services.AddSingleton<IDbMigrationService, EleonDefaultDbMigrationService>();

    var concurrencyOptions = new ConcurrencyHandlingOptions();
    context.Services.GetConfiguration()
      .GetSection(ConcurrencyHandlingOptions.DefaultSectionName)
      .Bind(concurrencyOptions);
    ConcurrencyExtensions.DefaultOptions = concurrencyOptions;
    context.Services.Configure<ConcurrencyHandlingOptions>(
      context.Services.GetConfiguration().GetSection(ConcurrencyHandlingOptions.DefaultSectionName));

    // register system event types
    MessagingConsts.SystemEventTypes.AddRange([
        typeof(DocMessageLogCreatedMsg),
        typeof(DocMessageLogPushMsg),
        typeof(DocMessageLogRetryMsg),
        typeof(SyncDocumentWithLifecycleMsg),
        typeof(StartingJobExecutionMsg),
        typeof(BackgroundJobRetriedMsg),
        typeof(BackgroundJobCompletedMsg),
        typeof(LifecycleRuleCheckMsg),
        typeof(ApplicationUpdatedMsg),
        typeof(UpdateFeaturesPermissionsRequestMsg),
        typeof(CustomSmsMsg),
        typeof(LifecycleRemovedMsg),
        typeof(HealthCheckStartedMsg),
        typeof(SystemHealthSettingsUpdatedMsg)
      ]);
  }

  public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
  {
    StaticServicesAccessor.Initialize(context.ServiceProvider);
  }
}
