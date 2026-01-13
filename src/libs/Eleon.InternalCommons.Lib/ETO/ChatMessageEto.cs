using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace Messaging.Module.ETO
{
  public class ChatMessageEto
  {
    public DateTime CreationTime { get; set; }
    public Guid Id { get; set; }
    public string Sender { get; set; }
    public ChatMessageSenderType SenderType { get; set; }
    public ChatMessageType Type { get; set; }
    public ChatMessageSeverity Severity { get; set; }
    public string Content { get; set; }
    public Guid ChatRoomId { get; set; }
    public bool Unread { get; set; }
    public bool Outcoming { get; set; }
    public ChatMemberInfoEto SenderInfo { get; set; }
  }
}
