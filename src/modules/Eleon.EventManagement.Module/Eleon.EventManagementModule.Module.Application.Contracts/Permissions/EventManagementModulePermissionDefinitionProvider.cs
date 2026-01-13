using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.EventManagementModule.Module.Localization;

namespace VPortal.EventManagementModule.Permissions;

public class EventManagementModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:EventManagementModule"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<EventManagementModuleResource>(name);
  }
}
