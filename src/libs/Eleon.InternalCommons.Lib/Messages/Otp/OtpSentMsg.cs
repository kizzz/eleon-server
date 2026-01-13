using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class OtpSentMsg : VportalEvent
  {
    public OtpGenerationResultDto Result { get; set; }
  }

  public class OtpGenerationResultDto
  {
    public int DurationSeconds { get; set; }
    public DateTime CreationUtcDate { get; set; }
    public DateTime ExpirationUtcDate { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
  }
}
