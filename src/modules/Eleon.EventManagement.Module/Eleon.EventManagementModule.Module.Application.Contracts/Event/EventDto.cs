

namespace EventManagementModule.Module.Application.Contracts.Event;
public class EventDto
{
  public Guid Id { get; set; }
  public Guid? TenantId { get; set; }
  public string Name { get; set; }
  public Guid QueueId { get; set; }
  public DateTime CreationTime { get; set; }
  public long Length { get; set; }
}
