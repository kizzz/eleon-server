using Common.Module.Constants;

namespace Messaging.Module.ETO
{
  public class BackgroundJobTextInfoEto
  {
    public BackgroundJobMessageType Type { get; set; } = BackgroundJobMessageType.Info;
    public string TextMessage { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
  }
}
