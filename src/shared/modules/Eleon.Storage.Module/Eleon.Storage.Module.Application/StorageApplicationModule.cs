using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(StorageDomainModule),
    typeof(StorageApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
public class StorageApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<StorageApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<StorageApplicationModule>(validate: true);
    });
  }
}

