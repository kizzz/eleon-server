using Common.EventBus.Module;
using Common.Module;
using Messaging.Module;
using Microsoft.Extensions.DependencyInjection;
using TenantSettings.Module.Cache;
using Volo.Abp.Modularity;

namespace TenantSettings.Module
{
  [DependsOn(typeof(CommonEventBusModule))]
  public class TenantSettingsModule : AbpModule
  {
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
      Configure<TenantSettingsCacheOptions>(context.Services.GetConfiguration());
    }
  }
}
