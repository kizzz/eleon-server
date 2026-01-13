using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.DocumentConversations;
public class DocumentChatMessageDto
{
  public string DocumentId { get; set; }
  public string DocumentObjectType { get; set; }
  public string LocalizationKey { get; set; }
  public string[] LocalizationParams { get; set; }
  public ChatMessageSeverity MessageSeverity { get; set; }
}
