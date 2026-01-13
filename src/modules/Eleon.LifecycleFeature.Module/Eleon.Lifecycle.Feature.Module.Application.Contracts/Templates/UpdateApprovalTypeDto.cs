using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  public class UpdateApprovalTypeDto
  {
    public Guid Id { get; set; }
    public LifecycleApprovalType NewApprovalType { get; set; }
  }
}
