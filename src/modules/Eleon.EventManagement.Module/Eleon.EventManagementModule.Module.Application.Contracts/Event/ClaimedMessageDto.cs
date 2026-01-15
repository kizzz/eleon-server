namespace EventManagementModule.Module.Application.Contracts.Event;

public class ClaimedMessageDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string? Token { get; set; }
  public string? Claims { get; set; }
  public int Attempts { get; set; }
  public DateTime CreatedUtc { get; set; }
  public string? MessageKey { get; set; }
  public string? TraceId { get; set; }
}
