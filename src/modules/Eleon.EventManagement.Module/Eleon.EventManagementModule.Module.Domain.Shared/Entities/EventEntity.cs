using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EventManagementModule.Module.Domain.Shared.Entities;

[DisableAuditing]
public class EventEntity : CreationAuditedEntity<Guid>, IMultiTenant, IHasExtraProperties
{
  public Guid? TenantId { get; set; }
  public string Name { get; set; }
  public string Message { get; set; }
  public Guid QueueId { get; set; }

  public Guid? Next { get; set; }
  public Guid? Prev { get; set; }

  public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; } = new ExtraPropertyDictionary();

  public EventEntity()
  {
  }

  public EventEntity(Guid id) : base(id)
  {
  }
}
