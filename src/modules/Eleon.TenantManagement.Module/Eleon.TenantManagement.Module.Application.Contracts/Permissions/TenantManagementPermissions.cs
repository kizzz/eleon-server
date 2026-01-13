using Commons.Module.Constants.Permission;
using Volo.Abp.Reflection;

namespace VPortal.TenantManagement.Module.Permissions;

public class TenantManagementPermissions
{
  public const string GroupName = "TenantManagement";

  public const string SuspendedAdmin = "SuspendedAdmin";
  public const string SuspendedUser = "SuspendedUser";

  public const string General = GroupName + ".General";

  public const string Site = GroupName + ".Site";
  public const string SiteManager = Site + ".Manager";


  public const string TenantSettingsManagement = DefaultPermissions.TenantSettingsManagement;

  public static class Dashboard
  {
    public const string VPortalGroup = "VPortal";
    public const string DashboardGroup = VPortalGroup + ".Dashboard";
    public const string Host = DashboardGroup + ".Host";
    public const string Tenant = DashboardGroup + ".Tenant";
  }

  public const string AdministrationPermission = DefaultPermissions.AdministrationPermission;

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(TenantManagementPermissions));
  }
}
