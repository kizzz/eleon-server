using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EventManagementModule.Module.Domain.Shared.Entities;

public class QueueEntity : FullAuditedEntity<Guid>, IMultiTenant
{
  public virtual Guid? TenantId { get; set; }
  public virtual Guid QueueDefinitionId { get; set; }
  public virtual string Name { get; set; }
  public virtual string DisplayName { get; set; }
  public virtual string Forwarding { get; set; }
  public virtual int MessagesLimit { get; set; }
  public virtual List<EventEntity> Messages { get; set; }

  public int Count { get; set; }

  public virtual Guid? Head { get; set; }
  public virtual Guid? Tail { get; set; }


  /// <summary>
  /// Empty constructor for deserialization purposes.
  /// </summary>
  public QueueEntity()
  {
  }

  public QueueEntity(Guid id) : base(id)
  {
  }
}
