using Common.Module.Constants;
using Eleon.Templating.Module.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Templating.Module.Templates;

public class MinimalTemplateDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = default!;
  public TemplateType Type { get; set; }
  public TextFormat Format { get; set; }
  public string TemplateId { get; set; }
  public bool IsSystem { get; set; }
}
