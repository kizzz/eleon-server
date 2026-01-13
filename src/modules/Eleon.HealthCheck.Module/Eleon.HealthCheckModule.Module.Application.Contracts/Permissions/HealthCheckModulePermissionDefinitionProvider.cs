using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.HealthCheckModule.Module.Localization;

namespace VPortal.HealthCheckModule.Permissions;

public class HealthCheckModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:HealthCheckModule"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<HealthCheckModuleResource>(name);
  }
}
