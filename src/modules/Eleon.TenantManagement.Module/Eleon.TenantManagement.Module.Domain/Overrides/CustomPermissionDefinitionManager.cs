using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomPermissionDefinitionManager : PermissionDefinitionManager, IPermissionDefinitionManager, ITransientDependency
  {
    public CustomPermissionDefinitionManager(IStaticPermissionDefinitionStore staticStore, IDynamicPermissionDefinitionStore dynamicStore) : base(staticStore, dynamicStore)
    {
      StaticStore = staticStore;
      DynamicStore = dynamicStore;
    }

    public IStaticPermissionDefinitionStore StaticStore { get; }
    public IDynamicPermissionDefinitionStore DynamicStore { get; }

    public new async Task<IReadOnlyList<PermissionGroupDefinition>> GetGroupsAsync()
    {
      var staticGroups = await StaticStore.GetGroupsAsync();
      var staticGroupDictionary = staticGroups.ToDictionary(g => g.Name, g => g);

      var dynamicGroups = (await DynamicStore.GetGroupsAsync()).ToList();

      foreach (var dynamicGroup in dynamicGroups)
      {
        if (staticGroupDictionary.TryGetValue(dynamicGroup.Name, out var staticGroup))
        {
          // Merge permissions from the static group into the dynamic group
          foreach (var staticPermission in staticGroup.Permissions)
          {
            var dynamicPermission = dynamicGroup.GetPermissionOrNull(staticPermission.Name);

            if (dynamicPermission == null)
            {
              // Add the static permission to the dynamic group if it doesn't exist
              dynamicGroup.AddPermission(
                  staticPermission.Name,
                  staticPermission.DisplayName,
                  staticPermission.MultiTenancySide,
                  staticPermission.IsEnabled
              );
            }
            else
            {
              // Merge child permissions
              MergeChildPermissions(dynamicPermission, staticPermission);
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

    private void MergeChildPermissions(PermissionDefinition dynamicPermission, PermissionDefinition staticPermission)
    {
      foreach (var staticChild in staticPermission.Children)
      {
        var dynamicChild = dynamicPermission.Children.FirstOrDefault(c => c.Name == staticChild.Name);

        if (dynamicChild == null)
        {
          // Add static child to dynamic if it doesn't exist
          dynamicPermission.AddChild(
              staticChild.Name,
              staticChild.DisplayName,
              staticChild.MultiTenancySide,
              staticChild.IsEnabled
          );
        }
        else
        {
          // Recursively merge child permissions
          MergeChildPermissions(dynamicChild, staticChild);
        }
      }
    }
  }
}
