using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions
{
  public class SendTextMessageRequestDto
  {
    public Guid ChatId { get; set; }
    public string Message { get; set; }
    public ChatMessageSeverity Severity { get; set; } = ChatMessageSeverity.None;
  }
}
