using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Otp.Module.DomainServices;
using VPortal.Otp.Module.Entities;

namespace VPortal.Otp.Module.EventServices
{
  public class GetOtpByRecipientEventService : IDistributedEventHandler<GetOtpByRecipientMsg>, ITransientDependency
  {
    private readonly IVportalLogger<GetOtpByRecipientEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly OtpDomainService domainService;

    public GetOtpByRecipientEventService(
        IVportalLogger<GetOtpByRecipientEventService> logger,
        IResponseContext responseContext,
        OtpDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(GetOtpByRecipientMsg eventData)
    {
      var msg = eventData;
      OtpEntity result = null;
      try
      {
        result = await domainService.GetOtpByRecipient(msg.Recipient);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        var response = new GetOtpByRecipientGotMsg()
        {
          IsExpired = result.IsExpired(),
          Recipient = result.Recipient,
        };
        await responseContext.RespondAsync(response);
      }

    }
  }
}
