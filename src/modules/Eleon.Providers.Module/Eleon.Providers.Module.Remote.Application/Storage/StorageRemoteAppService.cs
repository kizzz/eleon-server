using Logging.Module;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;
using VPortal.Storage.Remote.Application.Contracts.Storage;

namespace VPortal.Storage.Remote.Application.BlobProviders
{
  public class StorageRemoteAppService : StorageRemoteModuleAppService, IStorageRemoteAppService
  {
    private readonly IVportalLogger<StorageRemoteAppService> logger;
    private readonly StorageProviderOptionsManager optionsManager;
    private readonly StorageDomainService storageDomainService;

    public StorageRemoteAppService(
        IVportalLogger<StorageRemoteAppService> logger,
        StorageProviderOptionsManager optionsManager,
        StorageDomainService storageDomainService)
    {
      this.logger = logger;
      this.optionsManager = optionsManager;
      this.storageDomainService = storageDomainService;
    }

    public async Task<bool> Delete(string settingsGroup, string blobName)
    {

      bool result = false;
      try
      {
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          result = await storageDomainService.Delete(settingsGroup, blobName);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> Exists(string settingsGroup, string blobName)
    {

      bool result = false;
      try
      {
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          result = await storageDomainService.Exists(settingsGroup, blobName);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<string> GetBase64(string settingsGroup, string blobName)
    {

      string result = null;
      try
      {
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          var data = await storageDomainService.GetBytes(settingsGroup, blobName);
          result = Convert.ToBase64String(data);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> Save(SaveBase64Request request)
    {

      bool result = false;
      try
      {
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(request.SettingsGroup))
        {
          var data = Convert.FromBase64String(request.DataBase64);
          result = await storageDomainService.Save(request.SettingsGroup, request.BlobName, data);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
