using Volo.Abp.Reflection;

namespace VPortal.FileManager.Module.Permissions;

public class FileManagerPermissions
{
  public const string GroupName = "FileManager";
  public const string General = "Permission.FileManager.General";
  public const string Create = "Permission.FileManager.Create";
  public const string Share = "Permission.FileManager.Share";
  public const string Manage = "Permission.FileManager.Manage";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileManagerPermissions));
  }
}
