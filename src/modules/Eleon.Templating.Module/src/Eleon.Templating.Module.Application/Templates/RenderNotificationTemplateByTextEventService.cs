using Common.EventBus.Module;
using Common.Module.Constants;
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
  public class RenderNotificationTemplateByTextEventService : IDistributedEventHandler<RenderTemplateByTextMsg>, ITransientDependency
  {
    private readonly IVportalLogger<RenderNotificationTemplateByTextEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly TemplateManager templateManager;

    public RenderNotificationTemplateByTextEventService(IVportalLogger<RenderNotificationTemplateByTextEventService> logger, IResponseContext responseContext, TemplateManager templateManager)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.templateManager = templateManager;
    }

    public async Task HandleEventAsync(RenderTemplateByTextMsg eventData)
    {
      var response = new RenderTemplateResponse();
      try
      {
        var format = eventData.TemplateType.ToLowerInvariant() switch {

          "plaintext" => TextFormat.Plaintext,
          "scriban" => TextFormat.Scriban,
          _ => TextFormat.Plaintext
        };

        response.RenderedTemplate = await templateManager.ApplyTemplateByTextAsync(eventData.Text, format, eventData.Placeholders);
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
