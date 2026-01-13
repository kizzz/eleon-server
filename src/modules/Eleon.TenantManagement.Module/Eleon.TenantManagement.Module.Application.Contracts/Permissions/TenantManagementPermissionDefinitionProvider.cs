using NUglify.Helpers;
using Serilog;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module.Permissions;

public class TenantManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(
        TenantManagementPermissions.GroupName,
        L("Permission:TenantManagement"));
    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");


    var suspendedAdmin = myGroup.AddPermission(
        TenantManagementPermissions.SuspendedAdmin,
        L("Permission:TenantManagement:SuspendedAdmin"),
        multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Tenant);
    suspendedAdmin.Properties.Add("Order", 0);

    var suspendedUser = myGroup.AddPermission(
        TenantManagementPermissions.SuspendedUser,
        L("Permission:TenantManagement:SuspendedUser"),
        multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Tenant);
    suspendedUser.Properties.Add("Order", 1);

    var general = myGroup.AddPermission(
        TenantManagementPermissions.General,
        L("Permission:TenantManagement:General"));
    general.Properties.Add("Order", 2);

    var hostAdministration = myGroup.AddPermission("Permission.HostAdministration", L("Permission:TenantManagement:HostAdministration"),
        Volo.Abp.MultiTenancy.MultiTenancySides.Both);
    hostAdministration.Properties.Add("Order", 3);

    var site = myGroup.AddPermission(
        TenantManagementPermissions.Site,
        L("Permission:TenantManagement:Site"),
        Volo.Abp.MultiTenancy.MultiTenancySides.Tenant);
    site.Properties.Add("Order", 4);
    var siteManager = site.AddChild(
        TenantManagementPermissions.SiteManager,
        L("Permission:TenantManagement:SiteManager"),
        Volo.Abp.MultiTenancy.MultiTenancySides.Tenant);
    siteManager.Properties.Add("Order", 5);

    var settings = myGroup.AddPermission(TenantManagementPermissions.TenantSettingsManagement, L("Permission:TenantManagement:TenantSettingsManagement"));
    settings.Properties.Add("Order", 6);


    var vportalGroup = context.AddGroup(TenantManagementPermissions.Dashboard.VPortalGroup);
    vportalGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var hostDashboard = vportalGroup.AddPermission(
        TenantManagementPermissions.Dashboard.Host,
        L("Permission:Dashboard:Host"),
        MultiTenancySides.Host);
    hostDashboard.Properties.Add("Order", 0);

    var tenantDashboard = vportalGroup.AddPermission(
        TenantManagementPermissions.Dashboard.Tenant,
        L("Permission:Dashboard:Tenant"),
        MultiTenancySides.Tenant);
    tenantDashboard.Properties.Add("Order", 1);

    var administration = vportalGroup.AddPermission(
        TenantManagementPermissions.AdministrationPermission, // system managed permission
        L("Administration"));
    administration.Properties.Add("Order", 2);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<TenantManagementResource>(name);
  }

  public override void PostDefine(IPermissionDefinitionContext context)
  {
    var settingsGroup = context.GetGroupOrNull(SettingManagementPermissions.GroupName);
    if (settingsGroup != null)
    {
      settingsGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
    }

    var featuresGroup = context.GetGroupOrNull(FeatureManagementPermissions.GroupName);
    if (featuresGroup != null)
    {
      featuresGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
    }

    var abpTenantsGroup = context.GetGroupOrNull(Volo.Abp.TenantManagement.TenantManagementPermissions.GroupName);
    if (abpTenantsGroup != null)
    {
      abpTenantsGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
      abpTenantsGroup.Permissions.ForEach(p =>
      {
        p.MultiTenancySide = Volo.Abp.MultiTenancy.MultiTenancySides.Both;
        abpTenantsGroup.Permissions.ForEach(p1 =>
              {
            p1.MultiTenancySide = Volo.Abp.MultiTenancy.MultiTenancySides.Both;
          });
      });
    }
  }
}
