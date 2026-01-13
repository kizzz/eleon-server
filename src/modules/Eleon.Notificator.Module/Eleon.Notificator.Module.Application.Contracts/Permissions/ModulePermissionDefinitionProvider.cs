using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Notificator.Module.Localization;

namespace VPortal.Notificator.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    //var myGroup = context.AddGroup(NotificatorPermissions.GroupName, L("Permission:Notificator"));
    //myGroup.AddPermission(NotificatorPermissions.General, L("Permission:Notificator:General"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<NotificatorResource>(name);
  }
}
