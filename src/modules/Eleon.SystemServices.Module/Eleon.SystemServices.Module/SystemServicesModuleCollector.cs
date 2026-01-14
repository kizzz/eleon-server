using Volo.Abp.Modularity;

namespace VPortal.SystemServicesModule;

[DependsOn(
    typeof(VPortal.SystemServicesModule.Module.SystemServicesHttpApiModule),
    typeof(VPortal.SystemServicesModule.Module.SystemServicesApplicationModule),
    typeof(VPortal.SystemServicesModule.Module.EntityFrameworkCore.SystemServicesModuleEntityFrameworkCoreModule))]
public class SystemServicesModuleCollector : AbpModule
{ }

