using Logging.Module;
using SharedModule.modules.Blob.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;

namespace VPortal.FileManager.Module.Managers
{
  public class FileEditManager : DomainService
  {
    private readonly IVportalLogger<FileEditManager> logger;
    private readonly StorageDomainService storageDomainService;
    private readonly StorageProviderOptionsManager storageProviderOptionsManager;

    public FileEditManager(
        IVportalLogger<FileEditManager> logger,
        StorageDomainService storageDomainService,
        StorageProviderOptionsManager storageProviderOptionsManager)
    {
      this.logger = logger;
      this.storageDomainService = storageDomainService;
      this.storageProviderOptionsManager = storageProviderOptionsManager;
    }


    public async Task<(string, string)> Upload(string name, byte[] data)
    {

      (string, string) result = (null, null);
      try
      {
        var settingsGroup = storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          await storageDomainService.Save(settingsGroup, name, data);
          // Note: File ID and Web URL are now published via GoogleDriveBlobSavedEvent
          // To get these values, subscribe to the event or use a different mechanism
          // For now, returning null/null to maintain API contract
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task DeleteFile(string externalFileId)
    {
      try
      {
        var settingsGroup = storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          await storageDomainService.Delete(settingsGroup, externalFileId);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

    }

    public async Task<byte[]> DownloadFile(string externalFileId)
    {
      byte[] result = null;
      try
      {
        var settingsGroup = storageProviderOptionsManager.GetExplicitProviderTypeSettingsGroup(StorageTypes.GoogleDrive);
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          result = await storageDomainService.GetBytes(settingsGroup, externalFileId);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
