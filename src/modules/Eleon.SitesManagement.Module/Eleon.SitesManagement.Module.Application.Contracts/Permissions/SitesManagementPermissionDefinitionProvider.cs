using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module.Permissions;

public class SitesManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(
        SitesManagementPermissions.GroupName,
        L("Permission:SitesManagement"));
    myGroup.Properties.Add("CategoryName", "SitesManagement::PermissionGroupCategory:Administration");

  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<SitesManagementResource>(name);
  }
}


