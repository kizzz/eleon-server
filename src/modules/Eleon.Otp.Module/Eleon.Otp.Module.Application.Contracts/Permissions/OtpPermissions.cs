using Volo.Abp.Reflection;

namespace VPortal.Otp.Module.Permissions;

public class OtpPermissions
{
  public const string GroupName = "Otp";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(OtpPermissions));
  }
}
