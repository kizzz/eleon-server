using EleonS3.Application.EventHandlers;
using EleonS3.Domain;
using EleonsoftSdk.modules.StorageProvider.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.Storage.Module.DynamicOptions;
//using VPortal.Storage.Module.DynamicOptions;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringFileSystemModule),
    typeof(BlobStoringDatabaseDomainSharedModule),
    typeof(StorageDomainSharedModule)
    )]
public class StorageDomainModule : AbpModule
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

    context.Services.AddTransient<IBlobSftpPathCalculator, DefaultSftpBlobPathCalculator>();
    context.Services.Replace(ServiceDescriptor.Transient<IBlobContainerConfigurationProvider, DynamicBlobContainerConfigurationProvider>());
    context.Services.AddAbpDynamicOptions<AbpBlobStoringOptions, DynamicBlobStoringOptionsManager>();
  }
}
