using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;

namespace EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
public class VportalUserPermissionManagementProvider : UserPermissionManagementProvider, ITransientDependency
{
  private readonly IPermissionDefinitionRecordRepository _permissionDefinitionRecordRepository;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IStaticPermissionDefinitionStore _staticStore;

  public VportalUserPermissionManagementProvider(
      IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
      IHttpContextAccessor httpContextAccessor,
      IPermissionGrantRepository permissionGrantRepository,
      IGuidGenerator guidGenerator,
      ICurrentTenant currentTenant,
      IStaticPermissionDefinitionStore staticStore) : base(permissionGrantRepository, guidGenerator, currentTenant)
  {
    _permissionDefinitionRecordRepository = permissionDefinitionRecordRepository;
    _httpContextAccessor = httpContextAccessor;
    _staticStore = staticStore;
  }

  public override async Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
  {
    var multiplePermissionValueProviderGrantInfo = await CheckAsync(new[] { name }, providerName, providerKey);

    return multiplePermissionValueProviderGrantInfo.Result.First().Value;
  }

  public override async Task<MultiplePermissionValueProviderGrantInfo> CheckAsync(string[] names, string providerName, string providerKey)
  {
    var result = await base.CheckAsync(names, providerName, providerKey);

    await _permissionDefinitionRecordRepository.FilterBySource(result, CheckPermissionHelper.GetSourceId(_httpContextAccessor));

    // grant administration permission if any admin permission is granted
    if (names.Any(x => x == CheckPermissionHelper.AdminPermissionName))
    {
      var permissions = await _staticStore.GetPermissionsAsync(); // all admin permissions must be granted in static store
      var grantedAdminPermissions = await base.CheckAsync(permissions.Select(x => x.Name).ToArray(), providerName, providerKey);
      if (grantedAdminPermissions.Result.Any(x => x.Value.IsGranted))
      {
        result.Result[CheckPermissionHelper.AdminPermissionName] = new PermissionValueProviderGrantInfo(true, providerKey);
      }
    }

    return result;
  }
}
