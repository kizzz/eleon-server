
using Volo.Abp.Modularity;
using Volo.Abp.Domain;

namespace EleonCore.Modules.S3;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(EleonS3DomainSharedModule)
)]
public class EleonS3DomainModule : AbpModule { }
