namespace EventManagementModule.Module.Application.Contracts.Event;

public class AckRequestDto
{
  public Guid LockId { get; set; }
  public List<Guid> MessageIds { get; set; } = new();
}
