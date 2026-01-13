using Volo.Abp.Modularity;

namespace VPortal.EventManagementModule;

[DependsOn(
    typeof(VPortal.EventManagementModule.Module.EventManagementHttpApiModule),
    typeof(VPortal.EventManagementModule.Module.EventManagementApplicationModule),
    typeof(VPortal.EventManagementModule.Module.EntityFrameworkCore.EventManagementModuleEntityFrameworkCoreModule))]
public class EventManagementModuleCollector : AbpModule
{ }
