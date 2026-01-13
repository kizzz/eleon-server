using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Microservices;
// using VPortal.SitesManagement.Module.PermissionGroups;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(SitesManagementDomainModule),
    typeof(SitesManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class SitesManagementApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<SitesManagementApplicationModule>();
    context.Services.AddHostedService<MicroserviceMonitoringService>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<SitesManagementApplicationModule>(validate: true);
    });

    // context.Services.AddHostedService<EleoncoreMicroserviceInitializationHostedService>();
  }
}


