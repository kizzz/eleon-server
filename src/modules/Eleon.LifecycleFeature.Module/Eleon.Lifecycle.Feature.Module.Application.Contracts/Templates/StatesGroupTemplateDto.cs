using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class StatesGroupTemplateDto
  {
    public Guid? Id { get; set; }
    public string DocumentObjectType { get; set; }
    public string GroupName { get; set; }
    public bool IsActive { get; set; }
  }
}
