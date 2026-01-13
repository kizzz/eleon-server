using Common.Module.Extensions;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace Authorization.Module.Permissions
{
  public class FeaturePermissionHelper : ITransientDependency
  {
    private readonly IFeatureChecker featureChecker;
    private readonly ICurrentTenant currentTenant;
    private readonly IConfiguration configuration;

    public FeaturePermissionHelper(
        IFeatureChecker featureChecker,
        ICurrentTenant currentTenant,
        IConfiguration configuration)
    {
      this.featureChecker = featureChecker;
      this.currentTenant = currentTenant;
      this.configuration = configuration;
    }

    public async Task<List<PermissionDefinition>> FilterPermissionsByFeatures(List<PermissionDefinition> permissionDefinitions)
    {
      List<PermissionDefinition> allowed = null;
      if (currentTenant.Id == null)
      {
        allowed = permissionDefinitions;
      }
      else
      {
        allowed = new();
        foreach (var pd in permissionDefinitions)
        {
          string featureName = pd.Properties.GetValueOrDefault("FeatureName", null)?.ToString();
          if (featureName.IsNullOrEmpty() || await featureChecker.IsEnabledAsync(featureName!))
          {
            allowed.Add(pd);
          }
        }
      }

      if (bool.TryParse(configuration["TenantManagement:EnableMultiTenancy"], out bool isMultiTenant) && !isMultiTenant)
      {
        allowed.RemoveAll(permission => permission.Name.StartsWith("AbpTenantManagement."));
      }

      if (bool.TryParse(configuration["ElsaWorkflows:Enable"], out bool enableElsa) && !enableElsa)
      {
        allowed.RemoveAll(permission => permission.Name.StartsWith("CoreInfrastructure.Module.ElsaWorkflows"));
      }

      return allowed;
    }
  }
}
