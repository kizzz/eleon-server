using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class FullStateTemplateDto : StateTemplateDto
  {
    public List<StateActorTemplateDto> Actors { get; set; }
  }
}
