using EleonS3.Application.EventHandlers;
using EleonS3.Domain;
using EleonsoftSdk.modules.StorageProvider.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.Core.Infrastructure.Module;
//using VPortal.DataSource.Module;
// using VPortal.ProxyManagement;
using VPortal.Storage.Module.DynamicOptions;
//using VPortal.Storage.Remote.Application.Contracts;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringFileSystemModule),
    typeof(BlobStoringDatabaseDomainSharedModule),
    //typeof(StorageRemoteApplicationContractsModule),
    //typeof(DataSourceDomainSharedModule),
    typeof(ProvidersDomainSharedModule),
    typeof(ProviderApplicationContractsModule)
    //typeof(CoreInfrastructureApplicationContractsModule),
    //typeof(ProxyManagementDomainModule)
    )]
public class ProviderDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpBlobStoringOptions>(options =>
    {
      options.Containers.ConfigureDefault(container =>
          {
          container.UseFileSystem(cfg =>
              {
            });
        });
    });

    context.Services.Replace(ServiceDescriptor.Transient<IBlobContainerConfigurationProvider, DynamicBlobContainerConfigurationProvider>());
    context.Services.AddAbpDynamicOptions<AbpBlobStoringOptions, DynamicBlobStoringOptionsManager>();
  }
}
