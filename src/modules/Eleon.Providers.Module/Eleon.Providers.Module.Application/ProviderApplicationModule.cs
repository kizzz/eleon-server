using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(ProviderDomainModule),
    typeof(ProviderApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
public class ProviderApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ProviderApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ProviderApplicationModule>(validate: true);
    });
  }
}

