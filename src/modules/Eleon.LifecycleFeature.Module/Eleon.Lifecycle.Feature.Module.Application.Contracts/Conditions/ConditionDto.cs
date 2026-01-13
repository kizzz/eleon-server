using System;
using System.Collections.Generic;
using Common.Module.Constants;

namespace VPortal.Lifecycle.Feature.Module.Conditions
{
  public class ConditionDto
  {
    public Guid Id { get; set; }
    public LifecycleConditionTargetType ConditionType { get; set; }
    public Guid RefId { get; set; }
    public virtual List<RuleDto> Rules { get; set; }

    public ConditionDto() { }
  }
}
