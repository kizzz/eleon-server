using System.ComponentModel.DataAnnotations;
using Common.Module.Constants;

namespace Eleon.Templating.Module.Templates;

public class CreateUpdateTemplateDto
{
  [Required]
  [StringLength(TemplateConstants.MaxNameLength)]
  public string Name { get; set; } = default!;

  [Required]
  public TemplateType Type { get; set; }

  [Required]
  public TextFormat Format { get; set; }

  [Required]
  [StringLength(TemplateConstants.MaxTemplateLength)]
  public string TemplateContent { get; set; } = default!;

  [StringLength(TemplateConstants.MaxRequiredPlaceholdersLength)]
  public string RequiredPlaceholders { get; set; } = string.Empty;
  public string TemplateId { get; set; } = string.Empty;

  public bool IsSystem { get; set; }
}




