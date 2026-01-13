using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.Entities
{
  public class ControlDelegationHistoryEntity : CreationAuditedEntity<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Date { get; set; }
  }
}
