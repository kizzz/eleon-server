using System;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class CreateControlDelegationRequestDto
  {
    public Guid DelegatedToUserId { get; set; }
    public DateTime DelegationStartDate { get; set; }
    public DateTime? DelegationEndDate { get; set; }
    public string Reason { get; set; }
  }
}
