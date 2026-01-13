using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.DocumentConversations;
using System;
using System.Threading.Tasks;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace VPortal.Collaboration.Feature.Module.DocumentConversations
{
  [Authorize]
  public class DocumentConversationAppService : CollaborationAppService, IDocumentConversationAppService
  {
    private readonly IVportalLogger<DocumentConversationAppService> logger;
    private readonly DocumentConversationDomainService domainService;

    public DocumentConversationAppService(
        IVportalLogger<DocumentConversationAppService> logger,
        DocumentConversationDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<DocumentConversationInfoDto> GetDocumentConversationInfo(string docType, string documentId)
    {
      DocumentConversationInfoDto result = null;
      try
      {
        var info = await domainService.GetDocumentConversationInfo(docType, documentId);
        result = ObjectMapper.Map<DocumentConversationInfo, DocumentConversationInfoDto>(info);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SendDocumentChatMessagesAsync(List<DocumentChatMessageDto> messages)
    {
      try
      {
        await domainService.SendDocumentMessages(messages.Select(m => new DocumentChatMessageEto
        {
          DocumentId = m.DocumentId,
          DocumentObjectType = m.DocumentObjectType,
          LocalizationKey = m.LocalizationKey,
          LocalizationParams = m.LocalizationParams,
          MessageSeverity = m.MessageSeverity
        }).ToList());
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
