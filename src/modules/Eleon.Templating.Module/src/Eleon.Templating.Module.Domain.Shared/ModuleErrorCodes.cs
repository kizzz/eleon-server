namespace Eleon.Templating.Module;

public static class ModuleErrorCodes
{
  public const string TemplateNameTypeAlreadyExists = "Templating:Template:001";
  public const string TemplateNotFound = "Templating:Template:002";
  public const string CannotDeleteSystemTemplate = "Templating:Template:003";
  public const string CannotModifySystemTemplateIsSystem = "Templating:Template:004";
  public const string InvalidFormatForTemplateType = "Templating:Template:005";
  public const string InvalidJsonTemplate = "Templating:Template:006";
  public const string MissingRequiredPlaceholder = "Templating:Template:007";
  public const string InvalidTemplateFormat = "Templating:Template:008";
}
