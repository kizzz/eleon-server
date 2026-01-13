using Common.EventBus.Module;
using Commons.Module.Messages.Templating;
using Eleon.Templating.Module.Templates;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.GatewayManagement.Module.Vpn
{
  public class BulkRenderNotificationTemplateEventService : IDistributedEventHandler<BulkRenderNotificationTemplateMsg>, ITransientDependency
  {
    private readonly IVportalLogger<BulkRenderNotificationTemplateEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly TemplateManager templateManager;

    public BulkRenderNotificationTemplateEventService(IVportalLogger<BulkRenderNotificationTemplateEventService> logger, IResponseContext responseContext, TemplateManager templateManager)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.templateManager = templateManager;
    }

    public async Task HandleEventAsync(BulkRenderNotificationTemplateMsg eventData)
    {
      var response = new BulkRenderNotificationTemplateResponse();
      try
      {
        foreach (var template in eventData.TemplatePlaceholders)
        {
          response.RenderedTemplates.Add(await templateManager.ApplyTemplateAsync(eventData.TemplateKey, TemplateType.Notification, template));
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
