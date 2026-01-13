using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.EventManagementModule.Module;

[DependsOn(
    typeof(EventManagementModuleDomainModule),
    typeof(EventManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class EventManagementApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<EventManagementApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<EventManagementApplicationModule>(validate: true);
    });
  }
}
