using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace EventManagementModule.Module.Domain.Shared.Entities;

public class EventQueueMessageEntity : Entity<Guid>, IMultiTenant
{
  public Guid? TenantId { get; set; }
  public Guid QueueId { get; set; }
  public byte Lane { get; set; }
  public long EnqueueSeq { get; set; }
  public string Name { get; set; } = string.Empty;
  public byte Status { get; set; }
  public Guid? LockId { get; set; }
  public DateTime? LockedUntilUtc { get; set; }
  public int Attempts { get; set; }
  public DateTime? VisibleAfterUtc { get; set; }
  public DateTime CreatedUtc { get; set; }
  public string? MessageKey { get; set; }
  public string? TraceId { get; set; }
  public string? LastError { get; set; }

  public EventQueueMessageBodyEntity? Body { get; set; }

  public EventQueueMessageEntity()
  {
  }

  public EventQueueMessageEntity(Guid id) : base(id)
  {
  }
}
