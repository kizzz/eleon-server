using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace Eleon.Templating.Module;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(ModuleApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class TemplatingApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<TemplatingApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<TemplatingApplicationModule>(validate: true);
    });
  }
}
