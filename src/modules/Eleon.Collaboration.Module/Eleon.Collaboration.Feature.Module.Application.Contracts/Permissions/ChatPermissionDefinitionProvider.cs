using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Collaboration.Feature.Module.Localization;

namespace VPortal.Collaboration.Feature.Module.Permissions;

public class ChatPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    // var myGroup = context.AddGroup(ChatPermissions.GroupName, L("Permission:Chat"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<CollaborationResource>(name);
  }
}
