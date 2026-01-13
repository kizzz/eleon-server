using System;
using System.Collections.Generic;
using System.Text;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class FullStatesGroupTemplateDto : StatesGroupTemplateDto
  {
    public List<FullStateTemplateDto> States { get; set; }
  }
}
