using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule;

[DependsOn(
    typeof(VPortal.HealthCheckModule.Module.IdentityQueryingHttpApiModule),
    typeof(VPortal.HealthCheckModule.Module.IdentityQueryingApplicationModule)
  )]
public class IdentityQueryingModuleCollector : AbpModule
{ }
