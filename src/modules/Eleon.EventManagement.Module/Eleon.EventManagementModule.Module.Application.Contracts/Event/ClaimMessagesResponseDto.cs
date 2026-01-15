namespace EventManagementModule.Module.Application.Contracts.Event;

public class ClaimMessagesResponseDto
{
  public Guid LockId { get; set; }
  public string QueueStatus { get; set; } = string.Empty;
  public int PendingCount { get; set; }
  public List<ClaimedMessageDto> Messages { get; set; } = new();
}
