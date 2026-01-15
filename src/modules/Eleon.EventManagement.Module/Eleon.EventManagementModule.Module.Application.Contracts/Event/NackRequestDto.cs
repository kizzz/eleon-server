namespace EventManagementModule.Module.Application.Contracts.Event;

public class NackRequestDto
{
  public Guid LockId { get; set; }
  public Guid MessageId { get; set; }
  public int MaxAttempts { get; set; } = 10;
  public int DelaySeconds { get; set; } = 5;
  public string Error { get; set; } = string.Empty;
}
