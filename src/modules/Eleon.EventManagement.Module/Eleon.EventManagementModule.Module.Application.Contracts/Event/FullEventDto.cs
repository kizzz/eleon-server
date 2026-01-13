
namespace EventManagementModule.Module.Application.Contracts.Event;
public class FullEventDto
{
  public Guid Id { get; set; }
  public Guid? TenantId { get; set; }
  public string Name { get; set; }
  public Guid QueueId { get; set; }
  public string Message { get; set; }
  public DateTime CreationTime { get; set; }
  public string Token { get; set; }
  public string Claims { get; set; }
}
