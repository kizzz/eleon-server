using Common.Module.Extensions;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  [Authorize]
  [ExposeServices(typeof(IPermissionAppService))]
  [Volo.Abp.DependencyInjection.Dependency(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, ReplaceServices = true)]
  public class FeaturePermissionAppService : PermissionAppService
  {
    private readonly FeaturePermissionManager featurePermissionManager;
    private readonly VportalPermissionHelper permissionHelper;
    private readonly IVportalLogger<FeaturePermissionAppService> logger;

    public FeaturePermissionAppService(
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        IOptions<PermissionManagementOptions> options,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager,
        FeaturePermissionManager featurePermissionManager,
        VportalPermissionHelper permissionHelper,
        IVportalLogger<FeaturePermissionAppService> logger)
        : base(permissionManager, permissionDefinitionManager, options, simpleStateCheckerManager)
    {
      this.featurePermissionManager = featurePermissionManager;
      this.permissionHelper = permissionHelper;
      this.logger = logger;
    }

    public override async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
    {
      GetPermissionListResultDto result = null;
      try
      {
        await permissionHelper.EnsureAdmin();

        var baseResult = await base.GetAsync(providerName, providerKey);

        var granted = baseResult.Groups
            .SelectMany(g => g.Permissions.Select(p => p.Name))
            .ToList();

        var withoutSuspended = granted
            .Where(x => !VportalSpecialPermissions.IsSpecialPermission(x))
            .ToList();

        var filtered = await featurePermissionManager.FilterPermissionsByFeatures(withoutSuspended);

        foreach (var g in baseResult.Groups)
        {
          g.Permissions = g.Permissions
              .Where(p => filtered.Contains(p.Name))
              .ToList();
        }

        baseResult.Groups = baseResult.Groups
            .Where(x => !x.Permissions.IsNullOrEmpty())
            .ToList();

        result = baseResult;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }


      return result;
    }

    public override async Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {
      try
      {
        await permissionHelper.EnsureAdmin();

        var tryingToGrant = input.Permissions.Select(x => x.Name).ToList();

        var filtered = await featurePermissionManager.FilterPermissionsByFeatures(tryingToGrant);
        var withoutSuspended = filtered
            .Where(x => !VportalSpecialPermissions.IsSpecialPermission(x))
            .ToList();

        if (tryingToGrant.Count != withoutSuspended.Count)
        {
          input.Permissions = input.Permissions.Where(x => withoutSuspended.Contains(x.Name)).ToArray();
        }

        await base.UpdateAsync(providerName, providerKey, input);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
