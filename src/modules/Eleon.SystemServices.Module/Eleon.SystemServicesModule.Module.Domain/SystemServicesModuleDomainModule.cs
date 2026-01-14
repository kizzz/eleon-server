using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.SystemServicesModule.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(SystemServicesDomainSharedModule))]
public class SystemServicesModuleDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<SystemServicesModuleDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<SystemServicesModuleDomainModule>(validate: true);
    });
  }
}


