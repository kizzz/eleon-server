using Volo.Abp.Auditing;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.Auditor.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ModuleDomainSharedModule),
    typeof(AbpAuditingModule)
)]
public class ModuleDomainModule : AbpModule
{
}
