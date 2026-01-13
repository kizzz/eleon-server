using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using VPortal.Localization;

namespace VPortal.Permissions;

public class VPortalPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(VPortalPermissions.GroupName);
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var hostDashboard = myGroup.AddPermission(
        VPortalPermissions.Dashboard.Host,
        L("Permission:Dashboard:Host"),
        MultiTenancySides.Host);
    hostDashboard.Properties.Add("Order", 0);

    var tenantDashboard = myGroup.AddPermission(
        VPortalPermissions.Dashboard.Tenant,
        L("Permission:Dashboard:Tenant"),
        MultiTenancySides.Tenant);
    tenantDashboard.Properties.Add("Order", 1);

    var administration = myGroup.AddPermission(
        VPortalPermissions.AdministrationPermission, // system managed permission
        L("Administration"));
    administration.Properties.Add("Order", 2);

    //Define your own permissions here. Example:
    //myGroup.AddPermission(VPortalPermissions.MyPermission1, L("Permission:MyPermission1"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<VPortalResource>(name);
  }
}
