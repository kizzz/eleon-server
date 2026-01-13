using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using VPortal.InitilizationServices;
using VPortal.TenantManagement.Module.Microservices;
using VPortal.TenantManagement.Module.PermissionGroups;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(TenantManagementDomainModule),
    typeof(TenantManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class TenantManagementApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<TenantManagementApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<TenantManagementApplicationModule>(validate: true);
    });

    //context.Services.Replace(ServiceDescriptor.Transient(typeof(IPermissionAppService), typeof(FeaturePermissionAppService)));
    context.Services.AddHostedService<StartupNotifierHostedService>();
  }
}
