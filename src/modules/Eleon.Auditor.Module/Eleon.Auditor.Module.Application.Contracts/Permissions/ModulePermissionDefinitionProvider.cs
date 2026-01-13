using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Auditor.Module.Localization;

namespace VPortal.Auditor.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:Auditor.Module"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<ModuleResource>(name);
  }
}
