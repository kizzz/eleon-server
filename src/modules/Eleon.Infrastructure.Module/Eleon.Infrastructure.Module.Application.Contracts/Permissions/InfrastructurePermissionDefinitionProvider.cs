using Infrastructure.Module.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace VPortal.Infrastructure.Module.Permissions;

public class InfrastructurePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var group = context.AddGroup(
        InfarastructurePermissions.GroupName,
        L("Permission:InfrastructureModule"));
    group.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");

    var viewSecurityLogs = group.AddPermission(
        InfarastructurePermissions.ViewSecurityLogs,
        L("Permission:InfrastructureModule:ViewSecurityLogs"));
    viewSecurityLogs.Properties.Add("Order", 0);

    var sendNotification = group.AddPermission(
        InfarastructurePermissions.SendNotification,
        L("Permission:InfrastructureModule:SendNotification"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<InfrastructureResource>(name);
  }
}
