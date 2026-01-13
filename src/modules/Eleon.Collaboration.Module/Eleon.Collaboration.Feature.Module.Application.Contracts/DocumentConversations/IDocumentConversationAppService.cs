using Common.Module.Constants;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.DocumentConversations;
using System.Threading.Tasks;

namespace VPortal.Collaboration.Feature.Module.DocumentConversations
{
  public interface IDocumentConversationAppService
  {
    Task<DocumentConversationInfoDto> GetDocumentConversationInfo(string docType, string documentId);
    Task SendDocumentChatMessagesAsync(List<DocumentChatMessageDto> messages);
  }
}
