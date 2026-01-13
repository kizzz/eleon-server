using EleonCore.Modules.S3;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace EleonS3.Application.Contracts;

[DependsOn(
    typeof(EleonS3DomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule))]
public class EleonS3ApplicationContractsModule : AbpModule
{ }
