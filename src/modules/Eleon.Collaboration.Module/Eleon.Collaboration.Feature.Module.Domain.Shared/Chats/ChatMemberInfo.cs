using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class ChatMemberInfo
  {
    public string Name { get; set; }
    public string UserName { get; set; }
    public Guid Id { get; set; }
    public string Picture { get; set; }
    public ChatMemberRole Role { get; set; }
  }
}
