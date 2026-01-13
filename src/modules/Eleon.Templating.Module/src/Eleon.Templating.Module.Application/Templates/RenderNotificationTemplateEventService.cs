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
using Templating.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.GatewayManagement.Module.Vpn
{
  public class RenderNotificationTemplateEventService : IDistributedEventHandler<RenderNotificationTemplateMsg>, ITransientDependency
  {
    private readonly IVportalLogger<RenderNotificationTemplateEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly TemplateManager templateManager;

    public RenderNotificationTemplateEventService(IVportalLogger<RenderNotificationTemplateEventService> logger, IResponseContext responseContext, TemplateManager templateManager)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.templateManager = templateManager;
    }

    public async Task HandleEventAsync(RenderNotificationTemplateMsg eventData)
    {
      var response = new RenderNotificationTemplateResponse();
      try
      {
        response.RenderedTemplate = await templateManager.ApplyTemplateAsync(eventData.TemplateName, TemplateType.Notification, eventData.Placeholders);
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
