using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:StorageModule"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<StorageModuleResource>(name);
  }
}
