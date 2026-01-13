using Eleon.Storage.Lib.Constants;
using Microsoft.Extensions.Options;
using SharedModule.modules.Blob.Module.Constants;
using Storage.Module.BlobProviders.GoogleDrive;
using Storage.Module.BlobProviders.Proxy;
using Storage.Module.Domain.Shared.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Options;

namespace VPortal.Storage.Module.DynamicOptions
{
  public class DynamicBlobStoringOptionsManager : AbpDynamicOptionsManager<AbpBlobStoringOptions>
  {
    private readonly StorageModuleOptions moduleOptions;
    private readonly StorageProviderOptionsManager storageProviderOptionsManager;

    public DynamicBlobStoringOptionsManager(
        IOptionsFactory<AbpBlobStoringOptions> factory,
        IOptions<StorageModuleOptions> options,
        StorageProviderOptionsManager storageProviderOptionsManager)
        : base(factory)
    {
      this.moduleOptions = options.Value;
      this.storageProviderOptionsManager = storageProviderOptionsManager;
    }

    protected override async Task OverrideOptionsAsync(string name, AbpBlobStoringOptions options)
    {
      // Get settingsGroup from ambient context (set by callers via SetAmbientSettingsGroup before container operations)
      // Pass null to use ambient context
      var maybeCurrentSettings = await storageProviderOptionsManager.GetCurrentStorageProviderSettings(settingsGroup: null);
      if (maybeCurrentSettings == null)
      {
        ConfigureDefault(options);
        return;
      }

      var currentSettings = maybeCurrentSettings.Value;
      // Get settingsGroup from ambient context for providers that need it
      var settingsGroup = storageProviderOptionsManager.GetAmbientSettingsGroup() ?? string.Empty;

      switch (currentSettings.Key)
      {
        case StorageProviderDomainConstants.StorageTypeDatabase:
          ConfigureDatabase(options, currentSettings.Value);
          break;
        case StorageProviderDomainConstants.StorageTypeAzure:
          break;
        case StorageProviderDomainConstants.StorageTypeAWS:
          break;
        case StorageProviderDomainConstants.StorageTypeFileSystem:
          ConfigureFileSystem(options, currentSettings.Value);
          break;
        case StorageProviderDomainConstants.StorageTypeProxy:
          ConfigureProxy(options, currentSettings.Value);
          break;
        case StorageProviderDomainConstants.StorageTypeGoogleDrive:
          ConfigureGoogleDrive(options, currentSettings.Value, settingsGroup);
          break;
        default:
          break;
      }
    }

    private void ConfigureDefault(AbpBlobStoringOptions options) => ConfigureDatabase(options, new());

    private void ConfigureDatabase(AbpBlobStoringOptions options, Dictionary<string, string> settings)
        => options.Containers.ConfigureDefault((cfg) =>
        {
          cfg.UseDatabase();
        });
    private void ConfigureGoogleDrive(AbpBlobStoringOptions options, Dictionary<string, string> settings, string settingsGroup)
        => options.Containers.ConfigureDefault((cfg) =>
        {
          cfg.UseGoogleDrive(cfg =>
              {
              cfg.SettingsGroup = settingsGroup;
            });
        });

    private void ConfigureFileSystem(AbpBlobStoringOptions options, Dictionary<string, string> settings)
        => options.Containers.ConfigureDefault((containerConfig) =>
        {
          containerConfig.UseFileSystem(fsConfig =>
              {
              fsConfig.BasePath = settings.GetValueOrDefault("BasePath", string.Empty);
            });
        });

    private void ConfigureProxy(AbpBlobStoringOptions options, Dictionary<string, string> settings)
    {
      if (moduleOptions.IsProxy)
      {
        ConfigureFileSystem(options, settings);
      }
      else
      {
        string proxyIdStr = settings.GetValueOrDefault("ProxyId");
        if (Guid.TryParse(proxyIdStr, out var proxyId))
        {
          options.Containers.ConfigureDefault((containerConfig) =>
          {
            //containerConfig.UseProxyBlobProvider(cfg =>
            //{
            //    cfg.ProxyId = proxyId;
            //    cfg.SettingsGroup = storageProviderOptionsManager.SettingsGroup;
            //});
          });
        }
        else
        {
          throw new Exception("No proxy was configured for the Proxy Blob Provider");
        }
      }
    }
  }
}
