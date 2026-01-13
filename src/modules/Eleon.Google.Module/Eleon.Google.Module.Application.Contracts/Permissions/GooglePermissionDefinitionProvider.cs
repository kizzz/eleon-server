using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Google.Module.Localization;

namespace VPortal.Google.Module.Permissions;

public class GooglePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(GooglePermissions.GroupName, L("Permission:Google"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<GoogleResource>(name);
  }
}
