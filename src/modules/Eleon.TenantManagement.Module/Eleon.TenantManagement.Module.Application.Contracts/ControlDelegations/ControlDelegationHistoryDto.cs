using System;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class ControlDelegationHistoryDto
  {
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Date { get; set; }
  }
}
