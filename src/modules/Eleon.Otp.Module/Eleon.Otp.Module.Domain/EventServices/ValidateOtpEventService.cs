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
  public class ValidateOtpEventService :
      IDistributedEventHandler<ValidateOtpMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ValidateOtpEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly OtpDomainService domainService;

    public ValidateOtpEventService(
        IVportalLogger<ValidateOtpEventService> logger,
        IResponseContext responseContext,
        OtpDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(ValidateOtpMsg eventData)
    {
      var msg = eventData;
      OtpValidationResultEto result = null;
      try
      {
        result = await domainService.ValidateOtp(msg.Key, msg.Password);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        var response = new OtpValidatedMsg()
        {
          Result = result,
        };
        await responseContext.RespondAsync(response);
      }

    }
  }
}
