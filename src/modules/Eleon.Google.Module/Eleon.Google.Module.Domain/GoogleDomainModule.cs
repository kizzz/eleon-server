using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.Google.Module;

namespace VPortal.Google;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(GoogleDomainSharedModule)
)]
public class GoogleDomainModule : AbpModule
{ }
