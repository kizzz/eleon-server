using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.ApplicationConfiguration.Module.Localization;

namespace VPortal.ApplicationConfiguration.Module.Permissions;

public class ApplicationConfigurationPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ApplicationConfigurationPermissions.GroupName, L("Permission:ApplicationConfiguration"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<ApplicationConfigurationResource>(name);
  }
}
