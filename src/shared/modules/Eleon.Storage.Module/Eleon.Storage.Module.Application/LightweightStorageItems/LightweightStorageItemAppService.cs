using Common.Module.Keys;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Storage.Module.DomainServices;
using Storage.Module.LightweightStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using VPortal.Storage.Module;

namespace Storage.Module.LightweightStorageItems
{
  [Authorize]
  public class LightweightStorageItemAppService : StorageModuleAppService, ILightweightStorageItemAppService
  {
    private readonly IVportalLogger<LightweightStorageItemAppService> logger;
    private readonly LightweightStorageDomainService storageDomainService;
    private readonly IPermissionChecker permissionChecker;

    public LightweightStorageItemAppService(
        IVportalLogger<LightweightStorageItemAppService> logger,
        LightweightStorageDomainService storageDomainService,
        IPermissionChecker permissionChecker)
    {
      this.logger = logger;
      this.storageDomainService = storageDomainService;
      this.permissionChecker = permissionChecker;
    }

    public async Task<string> GetLightweightItem(string key)
    {
      string response = null;
      try
      {
        var item = await storageDomainService.GetLightweightItem(LightweightStorageKey.Parse(key));
        await EnsurePermissions([item]);
        response = item.Base64;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<string>> GetLightweightItems(List<string> keys)
    {
      List<string> response = null;
      try
      {
        var storageKeys = keys
            .Where(x => !string.IsNullOrEmpty(x))
            .SelectMany(x => x.Split(','))
            .Select(LightweightStorageKey.Parse)
            .ToList();

        var items = await storageDomainService.GetManyLightweightItems(storageKeys);
        await EnsurePermissions(items);
        response = items.Select(x => x?.Base64).ToList();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    private async Task EnsurePermissions(List<LightweightStorageItem> items)
    {
      var permissions = items.Where(x => x != null).SelectMany(x => x?.Permissions).Distinct().ToArray();
      if (permissions.IsNullOrEmpty())
      {
        return;
      }

      var grantResult = await permissionChecker.IsGrantedAsync(permissions);
      if (!grantResult.AllGranted)
      {
        throw new AbpAuthorizationException("You are not authorized to retreive this item.");
      }
    }
  }
}
