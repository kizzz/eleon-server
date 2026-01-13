using Common.Module.Constants;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.BackgroundJobs.Module.Localization;

namespace VPortal.BackgroundJobs.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(ModulePermissions.GroupName, L($"Permission:{"BackgroundJob"}"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var child = myGroup.AddPermission(
        $"Permission.{"BackgroundJob"}.General",
        displayName: LocalizableString.Create<ModuleResource>($"Permission:{"BackgroundJob"}:General"));
    child.Properties.Add("Order", 0);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<ModuleResource>(name);
  }
}
