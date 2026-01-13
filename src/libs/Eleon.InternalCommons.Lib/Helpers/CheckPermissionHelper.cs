using Commons.Module.Constants.Permission;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
public static class CheckPermissionHelper
{
  public static async Task<List<string>> FilterBySource(this IPermissionDefinitionRecordRepository repository, List<string> permissions, string sourceId)
  {
    // IMPORTANT: IF NULL RETURN PERMISSIONS OF ALL SERIVCES
    if (sourceId == null) // if sourceId == null return all permissions
    {
      return permissions.ToList();
    }

    var dbSet = await repository.GetDbSetAsync();
    var records = await dbSet.Where(x => permissions.Contains(x.Name)).ToListAsync();
    var result = records.Where(x => x.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty) == sourceId).Select(x => x.Name).ToList();
    if (sourceId == string.Empty)
    {
      var allRecords = records.Select(x => x.Name).ToList();
      result.AddRange(permissions.Where(x => !allRecords.Contains(x)));
    }
    return result;
  }

  public static async Task FilterBySource(this IPermissionDefinitionRecordRepository repository, MultiplePermissionGrantResult grantResult, string sourceId)
  {
    var grantedPermissions = grantResult.Result.Where(x => x.Value == PermissionGrantResult.Granted).Select(x => x.Key).ToList();

    var servicePermissions = await repository.FilterBySource(grantedPermissions, sourceId);

    foreach (var permission in grantedPermissions)
    {
      if (!servicePermissions.Contains(permission))
      {
        grantResult.Result[permission] = PermissionGrantResult.Undefined;
      }
    }
  }

  public static async Task FilterBySource(this IPermissionDefinitionRecordRepository repository, MultiplePermissionValueProviderGrantInfo grantResult, string sourceId)
  {
    var grantedPermissions = grantResult.Result.Where(x => x.Value.IsGranted).Select(x => x.Key).ToList();

    var servicePermissions = await repository.FilterBySource(grantedPermissions, sourceId);

    foreach (var permission in grantedPermissions)
    {
      if (!servicePermissions.Contains(permission))
      {
        grantResult.Result[permission] = PermissionValueProviderGrantInfo.NonGranted;
      }
    }
  }

  public static string GetSourceId(IHttpContextAccessor httpContextAccessor)
  {
    if (httpContextAccessor.HttpContext == null)
    {
      return string.Empty;
    }

    var apiKeyName = httpContextAccessor.HttpContext.User.GetApiKeyName();

    // IMPORTANT: DON'T ADD NULL CHECK
    // if http context is not null but api key is null that means that this request was from user and he/she must receive permissions of all service
    return apiKeyName;
  }

  public const string AdminPermissionName = DefaultPermissions.AdministrationPermission;
}
