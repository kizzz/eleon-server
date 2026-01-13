using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.EventManagementModule.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(EventManagementDomainSharedModule))]
public class EventManagementModuleDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<EventManagementModuleDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<EventManagementModuleDomainModule>(validate: true);
    });
  }
}

