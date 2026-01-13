using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace VPortal.Google.Module.Configuration
{
  public class GoogleKeyProvider : ITransientDependency
  {
    private readonly IFeatureChecker featureChecker;
    private readonly IConfiguration configuration;
    private readonly ICurrentTenant currentTenant;

    public GoogleKeyProvider(
        IFeatureChecker featureChecker,
        IConfiguration configuration,
        ICurrentTenant currentTenant)
    {
      this.featureChecker = featureChecker;
      this.configuration = configuration;
      this.currentTenant = currentTenant;
    }

    public async Task<string> GetMapsKey()
        => await GetFeature("Google.Maps.Key") ?? configuration["Google:Maps:DefaultKey"];
    public async Task<string> GetDriveKey()
        => await GetFeature("Google.Drive.Key") ?? configuration["Google:Drive:DefaultKey"];

    public async Task<string> GetOptimizationKey()
    {
      if (currentTenant.Id == null)
      {
        return await File.ReadAllTextAsync(configuration["Google:Optimization:DefaultKeyPath"]);
      }
      else
      {
        return await GetFeature("Google.Optimization.Key");
      }
    }

    private async Task<string> GetFeature(string featureName)
    {
      var featuretemp = await featureChecker.GetOrNullAsync("Google.Maps.Key");
      var feature = await featureChecker.GetOrNullAsync(featureName);
      if (string.IsNullOrWhiteSpace(feature))
      {
        return null;
      }

      return feature;
    }
  }
}
