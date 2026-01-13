using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Lifecycle.Feature.Module.Localization;

namespace VPortal.Lifecycle.Feature.Module.Permissions;

public class EventManagementModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(LifecyclePermissions.GroupName, L("Permission:LifecycleFeatureModule"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    myGroup.AddPermission(LifecyclePermissions.General, L("Permission:LifecycleFeatureModule:General"));
    myGroup.AddPermission(LifecyclePermissions.LifecycleManager, L("Permission:LifecycleFeatureModule:LifecycleManager"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<LifecycleFeatureModuleResource>(name);
  }
}
