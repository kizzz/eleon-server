using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Otp.Module.Localization;

namespace VPortal.Otp.Module.Permissions;

public class OtpPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(OtpPermissions.GroupName, L("Permission:Otp"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<OtpResource>(name);
  }
}
