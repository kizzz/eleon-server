using Volo.Abp.Reflection;

namespace VPortal.SitesManagement.Module.Permissions;

public class SitesManagementPermissions
{
  public const string GroupName = "SitesManagement";


  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(SitesManagementPermissions));
  }
}


