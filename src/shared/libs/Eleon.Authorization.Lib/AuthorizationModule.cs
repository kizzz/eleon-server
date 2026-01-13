using Common.EventBus.Module;
using Messaging.Module;
using TenantSettings.Module;
using Volo.Abp.Modularity;

namespace Authorization.Module
{
  [DependsOn(
      typeof(TenantSettingsModule),
      typeof(CommonEventBusModule))]
  public class AuthorizationModule : AbpModule
  {
  }
}
