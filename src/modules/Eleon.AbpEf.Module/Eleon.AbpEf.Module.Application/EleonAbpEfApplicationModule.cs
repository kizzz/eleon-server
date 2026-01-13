using EleoncoreMultiTenancy.Module;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace VPortal.EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleonAbpEfDomainModule),
    typeof(EleonAbpEfApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class EleonAbpEfApplicationModule : AbpModule
{
}
