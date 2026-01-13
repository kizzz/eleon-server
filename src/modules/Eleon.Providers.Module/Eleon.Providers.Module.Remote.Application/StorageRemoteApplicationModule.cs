using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Domain.Shared.Options;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using VPortal.Storage.Module;
using VPortal.Storage.Remote.Application.Contracts;

namespace VPortal.Storage.Remote.Application;

[DependsOn(
    typeof(ProviderDomainModule),
    typeof(StorageRemoteApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
public class StorageRemoteApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<StorageModuleOptions>(opt =>
    {
      opt.IsProxy = true;
    });

    context.Services.AddAutoMapperObjectMapper<StorageRemoteApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<StorageRemoteApplicationModule>(validate: true);
    });
  }
}
