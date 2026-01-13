using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    typeof(InfrastructureDomainModule),
    typeof(ModuleApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
public class ModuleApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ModuleApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ModuleApplicationModule>(validate: true);
    });

    Configure<AbpDistributedEntityEventOptions>(options =>
    {
      options.AutoEventSelectors.AddAll();
    });
  }
}
