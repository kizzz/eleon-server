using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.LanguageManagement.Module.Localization;

namespace VPortal.LanguageManagement.Module.Permissions;

public class LanguageManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(
        LanguageManagementPermissions.GroupName,
        L("Permission:LanguageManagement"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var manageLanguages = myGroup.AddPermission(
        LanguageManagementPermissions.ManageLanguages,
        L("Permission:ManageLanguages"));
    manageLanguages.Properties.Add("Order", 0);

  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<LanguageManagementResource>(name);
  }
}
