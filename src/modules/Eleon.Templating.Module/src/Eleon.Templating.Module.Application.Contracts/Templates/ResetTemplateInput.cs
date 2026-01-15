using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using Volo.Abp.Application.Dtos;

namespace Eleon.Templating.Module.Templates;

public class ResetTemplateInput 
{
  public string Name { get; set; }
  public TemplateType Type {get; set; }
}




