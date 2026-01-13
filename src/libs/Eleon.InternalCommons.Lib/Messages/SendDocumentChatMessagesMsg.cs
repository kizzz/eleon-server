using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendDocumentChatMessagesMsg : VportalEvent
  {
    public List<DocumentChatMessageEto> Messages { get; set; } = new List<DocumentChatMessageEto>();

  }
}
