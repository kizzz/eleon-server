using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace Messaging.Module.ETO
{
  public class ChatRoomEto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RefId { get; set; }
    public ChatRoomType Type { get; set; }
    public DateTime CreationTime { get; set; }
    public ChatRoomStatus Status { get; set; }
    public bool IsPublic { get; set; } = false;
  }
}
