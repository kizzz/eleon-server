using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace VPortal.Collaboration.Feature.Module.EventServices
{
  internal class SendDocumentChatMessagesConsumer :
      IDistributedEventHandler<SendDocumentChatMessagesMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SendDocumentChatMessagesConsumer> logger;
    private readonly IResponseContext responseContext;
    private readonly DocumentConversationDomainService documentConversationDomainService;

    public SendDocumentChatMessagesConsumer(
        IVportalLogger<SendDocumentChatMessagesConsumer> logger,
        IResponseContext responseContext,
        DocumentConversationDomainService documentConversationDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.documentConversationDomainService = documentConversationDomainService;
    }

    public async Task HandleEventAsync(SendDocumentChatMessagesMsg eventData)
    {
      var msg = eventData;
      var response = new ActionCompletedMsg();
      try
      {
        if (!msg.Messages.IsNullOrEmpty())
        {
          await documentConversationDomainService.SendDocumentMessages(msg.Messages);
          response.Success = true;
        }
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
