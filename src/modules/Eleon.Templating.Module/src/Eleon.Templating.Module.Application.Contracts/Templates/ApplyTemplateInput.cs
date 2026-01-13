using System.ComponentModel.DataAnnotations;

namespace Eleon.Templating.Module.Templates;

public class ApplyTemplateInput
{
  [Required]
  public string TemplateName { get; set; }

  public TemplateType TemplateType { get; set; }

  public Dictionary<string, string> Placeholders { get; set; } = new();
}




