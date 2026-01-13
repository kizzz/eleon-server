using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomPermissionManager : PermissionManager, IPermissionManager, ISingletonDependency
  {
    protected new IPermissionGrantRepository PermissionGrantRepository { get; }

    protected new IPermissionDefinitionManager PermissionDefinitionManager { get; }

    protected new ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

    protected new IGuidGenerator GuidGenerator { get; }

    protected new ICurrentTenant CurrentTenant { get; }

    protected new IReadOnlyList<IPermissionManagementProvider> ManagementProviders => _lazyProviders.Value;

    protected new PermissionManagementOptions Options { get; }

    protected new IDistributedCache<PermissionGrantCacheItem> Cache { get; }

    private readonly Lazy<List<IPermissionManagementProvider>> _lazyProviders;
    private readonly IPermissionChecker _permissionChecker;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomPermissionManager(
        IPermissionDefinitionManager permissionDefinitionManager,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager,
        IPermissionGrantRepository permissionGrantRepository,
        IServiceProvider serviceProvider,
        IGuidGenerator guidGenerator,
        IOptions<PermissionManagementOptions> options,
        ICurrentTenant currentTenant,
        IDistributedCache<PermissionGrantCacheItem> cache,
        IPermissionChecker permissionChecker,
        IHttpContextAccessor httpContextAccessor) : base(permissionDefinitionManager, simpleStateCheckerManager, permissionGrantRepository,
            serviceProvider, guidGenerator, options, currentTenant, cache)
    {
      GuidGenerator = guidGenerator;
      CurrentTenant = currentTenant;
      Cache = cache;
      _permissionChecker = permissionChecker;
      _httpContextAccessor = httpContextAccessor;
      SimpleStateCheckerManager = simpleStateCheckerManager;
      PermissionGrantRepository = permissionGrantRepository;
      PermissionDefinitionManager = permissionDefinitionManager;
      Options = options.Value;

      _lazyProviders = new Lazy<List<IPermissionManagementProvider>>(
          () => Options
              .ManagementProviders
              .Select(c => serviceProvider.GetRequiredService(c) as IPermissionManagementProvider)
              .ToList(),
          true
      );
    }

    public new virtual async Task<PermissionWithGrantedProviders> GetAsync(string permissionName, string providerName, string providerKey)
    {
      var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
      if (permission == null)
      {
        return new PermissionWithGrantedProviders(permissionName, false);
      }

      return await GetInternalAsync(
          permission,
          providerName,
          providerKey
      );
    }

    public new virtual async Task<MultiplePermissionWithGrantedProviders> GetAsync(
        string[] permissionNames,
        string providerName,
        string providerKey)
    {
      var permissions = new List<PermissionDefinition>();
      var undefinedPermissions = new List<string>();

      foreach (var permissionName in permissionNames)
      {
        var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
        if (permission != null)
        {
          permissions.Add(permission);
        }
        else
        {
          undefinedPermissions.Add(permissionName);
        }
      }

      if (!permissions.Any())
      {
        return new MultiplePermissionWithGrantedProviders(undefinedPermissions.ToArray());
      }

      var result = await GetInternalAsync(
          permissions.ToArray(),
          providerName,
          providerKey
      );

      foreach (var undefinedPermission in undefinedPermissions)
      {
        result.Result.Add(new PermissionWithGrantedProviders(undefinedPermission, false));
      }

      return result;
    }

    public new virtual async Task<List<PermissionWithGrantedProviders>> GetAllAsync(string providerName, string providerKey)
    {
      var permissionDefinitions = (await PermissionDefinitionManager.GetPermissionsAsync()).ToArray();

      var multiplePermissionWithGrantedProviders = await GetInternalAsync(permissionDefinitions, providerName, providerKey);

      return multiplePermissionWithGrantedProviders.Result;

    }

    public new virtual async Task SetAsync(string permissionName, string providerName, string providerKey, bool isGranted)
    {
      var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
      if (permission == null)
      {
        /* Silently ignore undefined permissions,
           maybe they were removed from dynamic permission definition store */
        return;
      }

      if (!permission.IsEnabled || !await SimpleStateCheckerManager.IsEnabledAsync(permission))
      {
        //TODO: BusinessException
        //throw new ApplicationException($"The permission named '{permission.Name}' is disabled!");
      }

      if (permission.Providers.Any() && !permission.Providers.Contains(providerName))
      {
        //TODO: BusinessException
        throw new ApplicationException($"The permission named '{permission.Name}' has not compatible with the provider named '{providerName}'");
      }

      if (!permission.MultiTenancySide.HasFlag(CurrentTenant.GetMultiTenancySide()))
      {
        //TODO: BusinessException
        throw new ApplicationException($"The permission named '{permission.Name}' has multitenancy side '{permission.MultiTenancySide}' which is not compatible with the current multitenancy side '{CurrentTenant.GetMultiTenancySide()}'");
      }

      var currentGrantInfo = await GetInternalAsync(permission, providerName, providerKey);
      if (currentGrantInfo.IsGranted == isGranted)
      {
        return;
      }

      var provider = ManagementProviders.FirstOrDefault(m => m.Name == providerName);
      if (provider == null)
      {
        //TODO: BusinessException
        throw new AbpException("Unknown permission management provider: " + providerName);
      }

      await provider.SetAsync(permissionName, providerKey, isGranted);
    }


    public new virtual async Task DeleteAsync(string providerName, string providerKey)
    {
      var permissionGrants = await PermissionGrantRepository.GetListAsync(providerName, providerKey);
      foreach (var permissionGrant in permissionGrants)
      {
        await PermissionGrantRepository.DeleteAsync(permissionGrant);
      }
    }

    protected new virtual async Task<PermissionWithGrantedProviders> GetInternalAsync(
        PermissionDefinition permission,
        string providerName,
        string providerKey)
    {
      var multiplePermissionWithGrantedProviders = await GetInternalAsync(
          new[] { permission },
          providerName,
          providerKey
      );

      return multiplePermissionWithGrantedProviders.Result.First();
    }

    protected new virtual async Task<MultiplePermissionWithGrantedProviders> GetInternalAsync(
        PermissionDefinition[] permissions,
        string providerName,
        string providerKey)
    {
      var apiKey = CheckPermissionHelper.GetSourceId(_httpContextAccessor);

      //if (apiKey != null)
      //{
      //    permissions = permissions.Where(x => (x.Properties.GetOrDefault(PermissionConstants.SourceIdPropertyName)?.ToString() ?? string.Empty) == apiKey).ToArray();
      //}

      if (permissions.Length == 0)
      {
        return new MultiplePermissionWithGrantedProviders();
      }

      var permissionNames = permissions.Select(x => x.Name).ToArray();
      var multiplePermissionWithGrantedProviders = new MultiplePermissionWithGrantedProviders(permissionNames);

      var neededCheckPermissions = new List<PermissionDefinition>();

      foreach (var permission in permissions
                                  .Where(x => x.IsEnabled)
                                  .Where(x => x.MultiTenancySide.HasFlag(CurrentTenant.GetMultiTenancySide()))
                                  .Where(x => !x.Providers.Any() || x.Providers.Contains(providerName)))
      {
        if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
        {
          neededCheckPermissions.Add(permission);
        }
      }

      if (!neededCheckPermissions.Any())
      {
        return multiplePermissionWithGrantedProviders;
      }

      foreach (var provider in ManagementProviders)
      {
        permissionNames = neededCheckPermissions.Select(x => x.Name).Distinct().ToArray();
        var multiplePermissionValueProviderGrantInfo = await provider.CheckAsync(permissionNames, providerName, providerKey);

        foreach (var providerResultDict in multiplePermissionValueProviderGrantInfo.Result)
        {
          if (providerResultDict.Value.IsGranted)
          {
            var permissionWithGrantedProvider = multiplePermissionWithGrantedProviders.Result
                .First(x => x.Name == providerResultDict.Key);

            permissionWithGrantedProvider.IsGranted = true;
            permissionWithGrantedProvider.Providers.Add(new PermissionValueProviderInfo(provider.Name, providerResultDict.Value.ProviderKey));
          }
        }
      }

      return multiplePermissionWithGrantedProviders;
    }
  }
}
