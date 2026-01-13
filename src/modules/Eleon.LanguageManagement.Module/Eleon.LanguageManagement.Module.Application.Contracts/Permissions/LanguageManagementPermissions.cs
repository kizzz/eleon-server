using Common.Module.Constants;
using Volo.Abp.Reflection;

namespace VPortal.LanguageManagement.Module.Permissions;

public class LanguageManagementPermissions
{
  public const string GroupName = "LanguageManagement";

  public const string ManageLanguages = $"Permission.{nameof(FeaturePack.LanguageManagement)}.ManageLanguages";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(LanguageManagementPermissions));
  }
}
