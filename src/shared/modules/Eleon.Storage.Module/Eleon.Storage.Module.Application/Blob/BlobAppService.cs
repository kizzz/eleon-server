using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.Storage.Module.Storage.Module.Application.Contracts.Blob;
using VPortal.Storage.Module;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;

namespace ModuleCollector.Storage.Module.Storage.Module.Application.Blob;

[Authorize]
public class BlobAppService : StorageModuleAppService, IBlobAppService
{
  private readonly IVportalLogger<BlobAppService> logger;
  private readonly StorageProviderOptionsManager optionsManager;
  private readonly StorageDomainService storageDomainService;

  public BlobAppService(
      IVportalLogger<BlobAppService> logger,
      StorageProviderOptionsManager optionsManager,
      StorageDomainService storageDomainService
      )
  {
    this.logger = logger;
    this.optionsManager = optionsManager;
    this.storageDomainService = storageDomainService;
  }

  public async Task<string> GetAsync(string settingsGroup, string blobName)
  {
    string response = null;
    try
    {
      using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
      {
        var bytes = await storageDomainService.GetBytes(settingsGroup, blobName);
        response = Convert.ToBase64String(bytes);
      }
    }
    catch (Exception ex)
    {
      logger.CaptureAndSuppress(ex);
    }

    return response;
  }

  public async Task<bool> SaveAsync(SaveBlobRequestDto request)
  {
    var response = false;
    try
    {
      using (StorageProviderOptionsManager.SetAmbientSettingsGroup(request.SettingGroup))
      {
        var bytes = Convert.FromBase64String(request.FileBase64);
        await storageDomainService.Save(request.SettingGroup, request.BlobName, bytes);
        response = true;
      }
    }
    catch (Exception ex)
    {
      logger.CaptureAndSuppress(ex);
    }

    return response;
  }

  public async Task<bool> RemoveAsync(string settingsGroup, string blobName)
  {
    var response = false;
    try
    {
      using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
      {
        await storageDomainService.Delete(settingsGroup, blobName);
        response = true;
      }
    }
    catch (Exception ex)
    {
      logger.CaptureAndSuppress(ex);
    }

    return response;
  }
}
