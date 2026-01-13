using Commons.Module.Constants.Permission;

namespace VPortal.Permissions;

public static class VPortalPermissions
{
  public const string GroupName = "VPortal";

  public static class Dashboard
  {
    public const string DashboardGroup = GroupName + ".Dashboard";
    public const string Host = DashboardGroup + ".Host";
    public const string Tenant = DashboardGroup + ".Tenant";


  }

  public const string AdministrationPermission = DefaultPermissions.AdministrationPermission;

  //Add your own permission names. Example:
  //public const string MyPermission1 = GroupName + ".MyPermission1";
}
