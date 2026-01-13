using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.ExternalLink.Module.Localization;

namespace VPortal.ExternalLink.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    //var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:ExternalLink"));
    //myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
    //var general = myGroup.AddPermission(
    //    ModulePermissions.General,
    //    L("Permission:ExternalLink:General"));
    //general.Properties.Add("Order", 0);

    //var create = myGroup.AddPermission(
    //    ModulePermissions.Create,
    //    L("Permission:ExternalLink:Create"));
    //create.Properties.Add("Order", 1);

    //var share = myGroup.AddPermission(
    //    ModulePermissions.Share,
    //    L("Permission:ExternalLink:Share"));
    //share.Properties.Add("Order", 2);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<ModuleResource>(name);
  }
}
