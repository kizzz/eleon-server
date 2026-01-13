using Common.Module.Constants;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace Authorization.Module.Permissions
{
  public class ApiKeyPermissionValueProvider : PermissionValueProvider
  {
    public override string Name => ApiKeyConstants.ApiKeyPermissionProviderName;

    private readonly IVportalLogger<ApiKeyPermissionValueProvider> logger;
    private readonly IPermissionDefinitionRecordRepository _permissionDefinitionRecordRepository;
    private readonly IStaticPermissionDefinitionStore _staticStore;

    public ApiKeyPermissionValueProvider(
        IVportalLogger<ApiKeyPermissionValueProvider> logger,
        IPermissionStore permissionStore,
        IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
        IStaticPermissionDefinitionStore staticStore) // todo use permission store
        : base(permissionStore)
    {
      this.logger = logger;
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
        logger.CaptureAndSuppress(e);
        return PermissionGrantResult.Undefined;
      }
      finally
      {
      }
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
      MultiplePermissionGrantResult result = null;
      try
      {
        var apiKeyId = context.Principal.GetApiKeyId();

        var permissionNames = context.Permissions.Select(p => p.Name).ToArray();

        if (string.IsNullOrEmpty(apiKeyId))
        {
          return new MultiplePermissionGrantResult(permissionNames);
        }

        result = await PermissionStore.IsGrantedAsync(permissionNames, Name, apiKeyId);

        await _permissionDefinitionRecordRepository.FilterBySource(result, string.Empty);

        // grant administration permission if any admin permission is granted
        if (context.Permissions.Any(x => x.Name == CheckPermissionHelper.AdminPermissionName))
        {
          var permissions = await _staticStore.GetPermissionsAsync(); // all admin permissions must be granted in static store
          var grantedAdminPermissions = await PermissionStore.IsGrantedAsync(permissions.Select(x => x.Name).ToArray(), Name, apiKeyId);
          if (grantedAdminPermissions.Result.Any(x => x.Value == PermissionGrantResult.Granted))
          {
            result.Result[CheckPermissionHelper.AdminPermissionName] = PermissionGrantResult.Granted;
          }
        }

        return result;
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
        return new MultiplePermissionGrantResult(context.Permissions.Select(p => p.Name).ToArray());
      }
      finally
      {
      }
    }
  }
}
