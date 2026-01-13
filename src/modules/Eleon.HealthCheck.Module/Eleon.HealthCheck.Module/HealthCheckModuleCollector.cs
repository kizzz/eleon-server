using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule;

[DependsOn(
    typeof(VPortal.HealthCheckModule.Module.HealthCheckHttpApiModule),
    typeof(VPortal.HealthCheckModule.Module.HealthCheckApplicationModule),
    typeof(VPortal.HealthCheckModule.Module.EntityFrameworkCore.HealthCheckModuleEntityFrameworkCoreModule))]
public class HealthCheckModuleCollector : AbpModule
{ }
