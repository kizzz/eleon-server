using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
//using VPortal.Identity.Module.Localization;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.Identity.Module.Permissions;

public class IdentityPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {

  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<TenantManagementResource>(name);
  }

  public override void PostDefine(IPermissionDefinitionContext context)
  {
    var abpIdentityGroup = context.GetGroupOrNull(Volo.Abp.Identity.IdentityPermissions.GroupName);
    if (abpIdentityGroup == null)
    {
      abpIdentityGroup = context.AddGroup(Volo.Abp.Identity.IdentityPermissions.GroupName, L("Permissions:Identity"));
    }
    abpIdentityGroup.DisplayName = L("Permissions:Group:UserManagement");
    abpIdentityGroup.Properties.TryAdd("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
    abpIdentityGroup.AddPermission(IdentityPermissions.Session.Revoke, L("Permissions:Session:Revoke"));

    abpIdentityGroup.AddPermission(IdentityPermissions.ApiKey.Default, L("Permissions:ApiKey:Default"));

    abpIdentityGroup.AddPermission(IdentityPermissions.Management.SettingsManagement, L("Permissions:IdentitySettingsManagement"));
  }
}
