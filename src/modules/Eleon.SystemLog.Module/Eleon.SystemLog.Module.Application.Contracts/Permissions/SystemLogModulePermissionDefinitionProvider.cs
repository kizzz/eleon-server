using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.DocMessageLog.Module.Localization;

namespace VPortal.DocMessageLog.Module.Permissions;

public class SystemLogModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(SystemLogModulePermissions.GroupName, L("Permission:DocMessageLog"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    myGroup.AddPermission(
        SystemLogModulePermissions.General,
        L("Permission:DocMessageLog:General"));

    myGroup.AddPermission(
        SystemLogModulePermissions.ViewSecurityLogs,
        L("Permission:DocMessageLog:ViewSecurityLogs"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<SystemLogResource>(name);
  }
}
