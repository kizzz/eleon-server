

using System.Collections.Immutable;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomFeatureDefinitionManager : FeatureDefinitionManager, IFeatureDefinitionManager, ITransientDependency
  {
    public CustomFeatureDefinitionManager(IStaticFeatureDefinitionStore staticStore, IDynamicFeatureDefinitionStore dynamicStore) : base(staticStore, dynamicStore)
    {
      StaticStore = staticStore;
      DynamicStore = dynamicStore;
    }

    public new IStaticFeatureDefinitionStore StaticStore { get; }
    public new IDynamicFeatureDefinitionStore DynamicStore { get; }

    public new async Task<IReadOnlyList<FeatureGroupDefinition>> GetGroupsAsync()
    {
      var staticGroups = await StaticStore.GetGroupsAsync();
      var staticGroupDictionary = staticGroups.ToDictionary(g => g.Name, g => g);

      var dynamicGroups = (await DynamicStore.GetGroupsAsync()).ToList();

      foreach (var dynamicGroup in dynamicGroups)
      {
        if (staticGroupDictionary.TryGetValue(dynamicGroup.Name, out var staticGroup))
        {
          // Merge permissions from the static group into the dynamic group
          foreach (var staticFeature in staticGroup.Features)
          {
            var dynamicFeature = dynamicGroup.Features.FirstOrDefault(f => f.Name == staticFeature.Name);

            if (dynamicFeature == null)
            {
              // Add the static permission to the dynamic group if it doesn't exist
              dynamicGroup.AddFeature(
                  staticFeature.Name,
                  displayName: staticFeature.DisplayName
              );
            }
            else
            {
              // Merge child permissions
              MergeChildFeatures(dynamicFeature, staticFeature);
            }
          }

          // Optionally, merge properties if required
          foreach (var staticProperty in staticGroup.Properties)
          {
            if (!dynamicGroup.Properties.ContainsKey(staticProperty.Key))
            {
              dynamicGroup.Properties[staticProperty.Key] = staticProperty.Value;
            }
          }
        }
      }

      // Add any static groups that were not in the dynamic groups
      foreach (var staticGroup in staticGroups)
      {
        if (!dynamicGroups.Any(g => g.Name == staticGroup.Name))
        {
          dynamicGroups.Add(staticGroup);
        }
      }

      return dynamicGroups.ToImmutableList();
    }

    private void MergeChildFeatures(FeatureDefinition dynamicFeature, FeatureDefinition staticFeature)
    {
      foreach (var staticChild in staticFeature.Children)
      {
        var dynamicChild = dynamicFeature.Children.FirstOrDefault(c => c.Name == staticChild.Name);

        if (dynamicChild == null)
        {
          // Add static child to dynamic if it doesn't exist
          dynamicFeature.CreateChild(
              staticChild.Name,
              displayName: staticChild.DisplayName
          );
        }
        else
        {
          // Recursively merge child permissions
          MergeChildFeatures(dynamicChild, staticChild);
        }
      }
    }
  }
}
