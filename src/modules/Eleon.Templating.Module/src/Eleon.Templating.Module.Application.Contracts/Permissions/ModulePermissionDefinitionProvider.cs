using Eleon.Templating.Module.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Eleon.Templating.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(ModulePermissions.GroupName, L("Permission:Module"));

    var templatesPermission = myGroup.AddPermission(ModulePermissions.Templates, L("Permission:Templates"));
    templatesPermission.AddChild(ModulePermissions.TemplatesCreate, L("Permission:Templates.Create"));
    templatesPermission.AddChild(ModulePermissions.TemplatesUpdate, L("Permission:Templates.Update"));
    templatesPermission.AddChild(ModulePermissions.TemplatesDelete, L("Permission:Templates.Delete"));
    templatesPermission.AddChild(ModulePermissions.TemplatesGet, L("Permission:Templates.Get"));
    templatesPermission.AddChild(ModulePermissions.TemplatesGetList, L("Permission:Templates.GetList"));
    templatesPermission.AddChild(ModulePermissions.TemplatesApply, L("Permission:Templates.Apply"));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<TemplatingResource>(name);
  }
}
