using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Otp.Module.DomainServices;

namespace VPortal.Otp.Module.EventServices
{
  public class SendOtpEventService :
      IDistributedEventHandler<SendOtpMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SendOtpEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly OtpDomainService domainService;

    public SendOtpEventService(
        IVportalLogger<SendOtpEventService> logger,
        IResponseContext responseContext,
        OtpDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(SendOtpMsg eventData)
    {
      var msg = eventData;
      OtpGenerationResultDto result = null;
      try
      {
        result = await domainService.GenerateOtpAsync(msg.Key, msg.Recipients, msg.MessageText, msg.UserId, msg.Session);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        var response = new OtpSentMsg()
        {
          Result = result,
        };

        await responseContext.RespondAsync(response);
      }

    }
  }
}
