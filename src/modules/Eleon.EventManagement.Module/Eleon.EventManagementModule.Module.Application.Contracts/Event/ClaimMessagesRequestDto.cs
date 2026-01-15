namespace EventManagementModule.Module.Application.Contracts.Event;

public class ClaimMessagesRequestDto
{
  public string QueueName { get; set; } = string.Empty;
  public int MaxCount { get; set; } = 100;
  public int LockSeconds { get; set; } = 60;
}
