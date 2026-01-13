using Authorization.Module;
using TenantSettings.Module;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpMultiTenancyModule),
    typeof(EleoncoreMultiTenancyDomainSharedModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(TenantSettingsModule)
)]
public class EleoncoreMultiTenancyDomainModule : AbpModule
{
}
