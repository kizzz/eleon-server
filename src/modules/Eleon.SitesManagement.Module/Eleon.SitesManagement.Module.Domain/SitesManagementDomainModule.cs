using Microsoft.Extensions.DependencyInjection;
using TenantSettings.Module;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(SitesManagementDomainSharedModule),
    typeof(AbpAutoMapperModule),
    typeof(TenantSettingsModule)
)]
public class SitesManagementDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<SitesManagementDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<SitesManagementDomainModule>(validate: true);
    });
  }
}


