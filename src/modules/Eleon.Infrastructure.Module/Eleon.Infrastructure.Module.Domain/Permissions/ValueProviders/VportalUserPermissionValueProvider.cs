using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace Authorization.Module.Permissions
{
  public class VportalUserPermissionValueProvider : UserPermissionValueProvider
  {
    private readonly IVportalLogger<VportalUserPermissionValueProvider> logger;
    private readonly FeaturePermissionHelper featurePermissionHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionDefinitionRecordRepository _permissionDefinitionRecordRepository;
    private readonly IStaticPermissionDefinitionStore _staticStore;

    public VportalUserPermissionValueProvider(
        IVportalLogger<VportalUserPermissionValueProvider> logger,
        IPermissionStore permissionStore,
        FeaturePermissionHelper featurePermissionHelper,
        IHttpContextAccessor httpContextAccessor,
        IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
        IStaticPermissionDefinitionStore staticStore)
        : base(permissionStore)
    {
      this.logger = logger;
      this.featurePermissionHelper = featurePermissionHelper;
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
        var baseResult = (await base.CheckAsync(context)).Result;

        var definitions = context.Permissions
            .Where(x => baseResult.Any(y => y.Key == x.Name))
            .ToList();

        var filteredPermissions = await featurePermissionHelper.FilterPermissionsByFeatures(definitions);

        var result = new MultiplePermissionGrantResult();
        foreach (var filtered in filteredPermissions)
        {
          result.Result.Add(filtered.Name, baseResult[filtered.Name]);
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
  }
}
