using Authorization.Module.Permissions;
using Common.EventBus.Module;
using Commons.Module.Messages.Permissions;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using IdentityModel;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using System.Data;
using TenantSettings.Module.Cache;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace VPortal.TenantManagement.Module.Permissions
{
  [UnitOfWork]
  public class VPortalRolePermissionValueProvider : RolePermissionValueProvider
  {
    private readonly IPermissionGrantRepository permissionGrantRepository;
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly IVportalLogger<VPortalRolePermissionValueProvider> logger;
    private readonly FeaturePermissionHelper featurePermissionHelper;
    private readonly IUserRoleFinder userRoleFinder;
    private readonly IDistributedEventBus _eventBus;
    private readonly ICurrentTenant currentTenant;
    private readonly ICurrentUser currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionDefinitionRecordRepository _permissionDefinitionRecordRepository;
    private readonly IStaticPermissionDefinitionStore _staticStore;

    public VPortalRolePermissionValueProvider(
        IPermissionGrantRepository permissionGrantRepository,
        TenantSettingsCacheService tenantSettingsCache,
        IVportalLogger<VPortalRolePermissionValueProvider> logger,
        FeaturePermissionHelper featurePermissionHelper,
        IUserRoleFinder userRoleFinder,
        IPermissionStore permissionStore,
        IDistributedEventBus requestClient,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IHttpContextAccessor httpContextAccessor,
        IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
        IStaticPermissionDefinitionStore staticStore)
        : base(permissionStore)
    {
      this.permissionGrantRepository = permissionGrantRepository;
      this.tenantSettingsCache = tenantSettingsCache;
      this.logger = logger;
      this.featurePermissionHelper = featurePermissionHelper;
      this.userRoleFinder = userRoleFinder;
      this._eventBus = requestClient;
      this.currentTenant = currentTenant;
      this.currentUser = currentUser;
      _httpContextAccessor = httpContextAccessor;
      _permissionDefinitionRecordRepository = permissionDefinitionRecordRepository;
      _staticStore = staticStore;
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
      try
      {
        var multiplePermissions = await CheckAsync(
            new PermissionValuesCheckContext(
                new List<PermissionDefinition>()
                {
                            context.Permission,
                },
                context.Principal));

        return multiplePermissions.Result.GetValueOrDefault(context.Permission.Name, PermissionGrantResult.Undefined);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {

      try
      {
        bool isAdmin = IsUserAdmin(context);
        bool isSuspended = await IsCurrentTenantSuspended();

        List<string> grantedPermissions;
        if (isSuspended)
        {
          grantedPermissions = isAdmin
              ? GetSuspendedAdminPermissions(context.Permissions)
              : GetSuspendedUserPermissions();
        }
        else
        {
          grantedPermissions = isAdmin
              ? GetAdminPermissions(context.Permissions)
              : await GetUserPermissions(context);
        }

        var withDefinitions = context.Permissions
            .Where(x => grantedPermissions.Any(y => y == x.Name))
            .ToList();

        var withoutDefinitions = grantedPermissions
            .Where(x => context.Permissions.All(y => y.Name != x))
            .ToList();

        var filteredPermissions =
            (await featurePermissionHelper.FilterPermissionsByFeatures(withDefinitions))
            .Select(x => x.Name)
            .Concat(withoutDefinitions);

        var prohibitedPermissions = new List<string>();
        if (!isSuspended)
        {
          filteredPermissions = filteredPermissions
              .Where(x => !VportalSpecialPermissions.IsSpecialPermission(x))
              .ToList();
          prohibitedPermissions.AddRange(VportalSpecialPermissions.GetAll());
        }

        var result = new MultiplePermissionGrantResult();

        foreach (var granted in filteredPermissions.Distinct())
        {
          result.Result.Add(granted, PermissionGrantResult.Granted);
        }

        foreach (var prohibited in prohibitedPermissions.Distinct())
        {
          result.Result.Add(prohibited, PermissionGrantResult.Prohibited);
        }

        await _permissionDefinitionRecordRepository.FilterBySource(result, string.Empty);

        // grant administration permission if any admin permission is granted
        if (context.Permissions.Any(x => x.Name == CheckPermissionHelper.AdminPermissionName))
        {
          var permissions = await _staticStore.GetPermissionsAsync(); // all admin permissions must be granted in static store
          var grantedAdminPermissions = await base.CheckAsync(new PermissionValuesCheckContext(permissions.ToList(), context.Principal));
          if (grantedAdminPermissions.Result.Any(x => x.Value == PermissionGrantResult.Granted))
          {
            result.Result[CheckPermissionHelper.AdminPermissionName] = PermissionGrantResult.Granted;
          }
        }

        return result;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    private List<string> GetAdminPermissions(List<PermissionDefinition> permissionDefinitions)
        => permissionDefinitions
            .Select(x => x.Name)
            .ToList();

    private List<string> GetSuspendedAdminPermissions(List<PermissionDefinition> permissionDefinitions)
        => permissionDefinitions
            .Select(x => x.Name)
            .Where(x => x.StartsWith("Permission.Account"))
            .Concat([VportalSpecialPermissions.SuspendedAdmin])
            .ToList();

    private List<string> GetSuspendedUserPermissions()
        => [VportalSpecialPermissions.SuspendedUser];

    private async Task<List<string>> GetUserPermissions(PermissionValuesCheckContext context)
    {
      if (!currentUser.Id.HasValue)
      {
        return [];
      }

      var response = await _eventBus.RequestAsync<GetGrantedRolePermissionsResponseMsg>(new GetGrantedRolePermissionsMsg { UserId = currentUser.Id.Value });

      return response.Permissions;
    }

    private bool IsUserAdmin(PermissionValuesCheckContext context)
    {
      var identity = context.Principal.Identities.FirstOrDefault();
      var isAdminRole = identity?.Claims.Any(c => (c.Type == JwtClaimTypes.Role || c.Type == AbpClaimTypes.Role) && c.Value == MigrationConsts.AdminRoleNameDefaultValue) == true;
      return isAdminRole;
    }

    private async Task<bool> IsCurrentTenantSuspended()
    {
      var inactiveTenants = await tenantSettingsCache.GetInactiveTenants();
      return currentTenant.Id.HasValue && inactiveTenants.Contains(currentTenant.Id.Value);
    }
  }
}
