using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.GatewayManagement.Module.Localization;

namespace VPortal.GatewayManagement.Module.Permissions;

public class GatewayManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(GatewayManagementPermissions.GroupName, L("Permission:GatewayManagement"));
    myGroup.Properties.Add("CategoryName", "SitesManagement::PermissionGroupCategory:Administration");

    var gateway = myGroup.AddPermission(
        GatewayManagementPermissions.Gateway,
        L("Permission:GatewayManagement:Gateway"),
        multiTenancySide: Volo.Abp.MultiTenancy.MultiTenancySides.Host);
    gateway.Properties.Add("Order", 0);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<GatewayManagementResource>(name);
  }
}
