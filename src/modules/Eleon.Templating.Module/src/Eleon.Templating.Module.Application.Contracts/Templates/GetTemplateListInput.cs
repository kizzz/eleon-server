using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Volo.Abp.Application.Dtos;

namespace Eleon.Templating.Module.Templates;

public class GetTemplateListInput : PagedAndSortedResultRequestDto
{
  public string SearchQuery { get; set; }
  public TemplateType? Type { get; set; }
  public TextFormat? Format { get; set; }
  public bool? IsSystem { get; set; }
  public string? Filter { get; set; }
}




