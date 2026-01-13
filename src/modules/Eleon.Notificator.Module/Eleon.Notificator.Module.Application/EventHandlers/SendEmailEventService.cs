using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace VPortal.Notificator.Module.EventServices
{
  public class SendEmailEventService :
      IDistributedEventHandler<SendEmailMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SendEmailEventService> logger;
    private readonly EmailNotificator emailNotificator;
    private readonly IResponseContext responseContext;

    public SendEmailEventService(
        IVportalLogger<SendEmailEventService> logger,
        EmailNotificator emailNotificator
,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.emailNotificator = emailNotificator;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(SendEmailMsg eventData)
    {

      var response = new SendEmailGotMsg();
      try
      {
        await emailNotificator.SendEmailAsync(eventData.Subject, eventData.Body, [eventData.TargetEmailAddress], true);
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
        response.ErrorMsg = e.Message;
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }
    }
  }

}
