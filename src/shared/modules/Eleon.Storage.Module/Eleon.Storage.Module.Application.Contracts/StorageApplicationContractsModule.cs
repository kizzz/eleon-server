using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(StorageDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule))]
public class StorageApplicationContractsModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<StorageApplicationContractsModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<StorageApplicationContractsModule>(validate: true);
    });
  }
}
