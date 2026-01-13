using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Migrations.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;

namespace EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
public class VportalRolePermissionManagementProvider : RolePermissionManagementProvider, ITransientDependency
{
  private readonly IPermissionDefinitionRecordRepository _permissionDefinitionRecordRepository;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IIdentityRoleRepository _identityRoleRepository;
  private readonly IStaticPermissionDefinitionStore _staticStore;

  public VportalRolePermissionManagementProvider(
      IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
      IHttpContextAccessor httpContextAccessor,
      IUserRoleFinder userRoleFinder,
      IPermissionGrantRepository permissionGrantRepository,
      IIdentityRoleRepository identityRoleRepository,
      IGuidGenerator guidGenerator,
      ICurrentTenant currentTenant,
      IStaticPermissionDefinitionStore staticStore) : base(permissionGrantRepository, guidGenerator, currentTenant, userRoleFinder)
  {
    _permissionDefinitionRecordRepository = permissionDefinitionRecordRepository;
    _httpContextAccessor = httpContextAccessor;
    _identityRoleRepository = identityRoleRepository;
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

    if (providerName == "R" && Guid.TryParse(providerKey, out var roleId))
    {
      var role = await _identityRoleRepository.FindAsync(roleId);
      if (role != null && role.Name == MigrationConsts.AdminRoleNameDefaultValue)
      {
        foreach (var r in result.Result)
        {
          result.Result[r.Key] = new PermissionValueProviderGrantInfo(true, providerKey);
        }
      }
    }

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
