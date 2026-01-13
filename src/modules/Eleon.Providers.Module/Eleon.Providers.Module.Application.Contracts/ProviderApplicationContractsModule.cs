using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(ProvidersDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule))]
public class ProviderApplicationContractsModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ProviderApplicationContractsModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ProviderApplicationContractsModule>(validate: true);
    });
  }
}
