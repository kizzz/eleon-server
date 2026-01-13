using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eleon.Templating.Module.Templates;

public class Template : AuditedAggregateRoot<Guid>
{
  public string Name { get; set; } = default!;
  public TemplateType Type { get; set; }
  public TextFormat Format { get; set; }
  public string TemplateId { get; set; }
  public string TemplateContent { get; set; } = default!;
  public string RequiredPlaceholders { get; set; } = string.Empty;
  public bool IsSystem { get; set; }

  // EF Core constructor
  protected Template()
  {
  }

  public Template(
      Guid id)
      : base(id)
  {
  }


  public void Update(
      string name,
      TemplateType type,
      TextFormat format,
      string templateContent,
      string requiredPlaceholders,
      string templateId)
  {
    Name = name;
    Type = type;
    Format = format;
    TemplateContent = templateContent;
    RequiredPlaceholders = requiredPlaceholders;
    TemplateId = templateId;
  }
}




