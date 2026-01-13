using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Modularity;

namespace VPortal.NotificatorModule
{
  [DependsOn(
  typeof(VPortal.Notificator.Module.ModuleHttpApiModule),
  typeof(VPortal.Notificator.Module.ModuleApplicationModule),
  typeof(VPortal.Notificator.Module.EntityFrameworkCore.NotificatorFrameworkCoreModule),
  typeof(AbpAspNetCoreSignalRModule)
  )]
  public class NotificatorModuleCollector : AbpModule
  {

  }
}
