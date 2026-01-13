using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;
using VPortal.Google.Module.Localization;

namespace VPortal.Google.Module.Features
{
  public class GoogleFeatureDefinitionProvider : FeatureDefinitionProvider
  {
    public override void Define(IFeatureDefinitionContext context)
    {
      var googleGroup = context.AddGroup($"Google");
      var googleMapsKeyFeature = googleGroup.AddFeature(
          $"Google.Maps.Key",
          defaultValue: string.Empty,
          displayName: LocalizableString.Create<GoogleResource>($"Features:Google.Maps.Key"),
          valueType: new FreeTextStringValueType());
      var googleDirveKeyFeature = googleGroup.AddFeature(
          $"Google.Drive.Key",
          defaultValue: string.Empty,
          displayName: LocalizableString.Create<GoogleResource>($"Features:Google.Drive.Key"),
          valueType: new FreeTextStringValueType());
      var googleOptimizationKeyFeature = googleGroup.AddFeature(
          $"Google.Optimization.Key",
          defaultValue: string.Empty,
          displayName: LocalizableString.Create<GoogleResource>($"Features:Google.Optimization.Key"),
          valueType: new FreeTextStringValueType());
    }
  }
}
