using Volo.Abp.Reflection;

namespace Eleon.Templating.Module.Permissions;

public class ModulePermissions
{
  public const string GroupName = "Module";

  public const string Templates = GroupName + ".Templates";
  public const string TemplatesCreate = Templates + ".Create";
  public const string TemplatesUpdate = Templates + ".Update";
  public const string TemplatesDelete = Templates + ".Delete";
  public const string TemplatesGet = Templates + ".Get";
  public const string TemplatesGetList = Templates + ".GetList";
  public const string TemplatesApply = Templates + ".Apply";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ModulePermissions));
  }
}
