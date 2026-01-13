using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace Messaging.Module.ETO
{
  public class DocumentChatMessageEto
  {
    public string DocumentId { get; set; }
    public string DocumentObjectType { get; set; }
    public string LocalizationKey { get; set; }
    public string[] LocalizationParams { get; set; }
    public ChatMessageSeverity MessageSeverity { get; set; }
  }
}
