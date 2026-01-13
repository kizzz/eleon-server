using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace Messaging.Module.ETO
{
  public class ChatMemberInfoEto
  {
    public string Name { get; set; }
    public Guid Id { get; set; }
    public string Picture { get; set; }
    public ChatMemberRole Role { get; set; }
  }
}
