using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    typeof(MinimalInfrastructureDomainModule),
    typeof(AbpAspNetCoreMvcModule))]
public class InfrastructureUtilsHttpApiModule : AbpModule
{
}
