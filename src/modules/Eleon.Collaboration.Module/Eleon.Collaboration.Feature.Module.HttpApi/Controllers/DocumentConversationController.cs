using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.DocumentConversations;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Collaboration.Feature.Module.DocumentConversations;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/DocumentConversations")]
  public class DocumentConversationController : ChatController, IDocumentConversationAppService
  {
    private readonly IDocumentConversationAppService appService;
    private readonly IVportalLogger<DocumentConversationController> _logger;

    public DocumentConversationController(
        IDocumentConversationAppService appService,
        IVportalLogger<DocumentConversationController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpGet("GetDocumentConversationInfo")]
    public async Task<DocumentConversationInfoDto> GetDocumentConversationInfo(string docType, string documentId)
    {

      var response = await appService.GetDocumentConversationInfo(docType, documentId);


      return response;
    }

    [HttpPost("SendDocumentChatMessages")]
    public async Task SendDocumentChatMessagesAsync(List<DocumentChatMessageDto> messages)
    {

      await appService.SendDocumentChatMessagesAsync(messages);

    }
  }
}
