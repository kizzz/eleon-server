using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.SystemServicesModule.Module.Localization;

namespace VPortal.SystemServicesModule.Permissions;

public class SystemServicesModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:SystemServicesModule"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<SystemServicesModuleResource>(name);
  }
}

