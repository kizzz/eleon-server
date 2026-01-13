using System;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class UpdateControlDelegationRequestDto
  {
    public Guid DelegationId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string Reason { get; set; }
  }
}
