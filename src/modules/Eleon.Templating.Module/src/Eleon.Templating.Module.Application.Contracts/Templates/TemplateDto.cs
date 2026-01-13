using Eleon.Templating.Module.Templates;

namespace Eleon.Templating.Module.Templates;

public class TemplateDto : MinimalTemplateDto
{
  public string TemplateContent { get; set; } = default!;
  public string RequiredPlaceholders { get; set; } = string.Empty;
  public DateTime CreationTime { get; set; }
  public DateTime? LastModificationTime { get; set; }
}




