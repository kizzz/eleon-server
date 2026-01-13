using Volo.Abp.Domain.Entities.Auditing;

namespace EventManagementModule.Module.Domain.Shared.Entities;
public class QueueDefinitionEntity : FullAuditedEntity<Guid>
{
  public QueueDefinitionEntity() { }
  public QueueDefinitionEntity(Guid id) : base(id) { }

  public Guid? TenantId { get; set; }
  public string Name { get; set; }
  public int MessagesLimit { get; set; }

  public string Messages { get; set; }
}
