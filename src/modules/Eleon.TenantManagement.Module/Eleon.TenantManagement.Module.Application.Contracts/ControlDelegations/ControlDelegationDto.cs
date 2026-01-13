using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class ControlDelegationDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DelegatedToUserId { get; set; }
    public DateTime DelegationStartDate { get; set; }
    public DateTime? DelegationEndDate { get; set; }
    public bool Active { get; set; }
    public string Reason { get; set; }
    public string UserName { get; set; }
    public string DelegatedToUserName { get; set; }
    public DateTime? LastLoginDate { get; set; }

    public List<ControlDelegationHistoryDto> DelegationHistory { get; set; }
  }
}
