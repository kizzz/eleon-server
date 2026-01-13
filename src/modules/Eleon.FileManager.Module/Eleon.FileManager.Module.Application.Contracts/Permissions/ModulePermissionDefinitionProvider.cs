using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.FileManager.Module.Localization;

namespace VPortal.FileManager.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(FileManagerPermissions.GroupName, L("Permission:FileManager"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var general = myGroup.AddPermission(
        FileManagerPermissions.General,
        L("Permission:FileManager:General"));
    general.Properties.Add("Order", 0);

    var create = myGroup.AddPermission(
        FileManagerPermissions.Create,
        L("Permission:FileManager:Create"));
    create.Properties.Add("Order", 1);

    var share = myGroup.AddPermission(
        FileManagerPermissions.Share,
        L("Permission:FileManager:Share"));
    share.Properties.Add("Order", 2);

    var manage = myGroup.AddPermission(
        FileManagerPermissions.Manage,
        L("Permission:FileManager:Manage"));
    manage.Properties.Add("Order", 3);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<ModuleResource>(name);
  }
}
