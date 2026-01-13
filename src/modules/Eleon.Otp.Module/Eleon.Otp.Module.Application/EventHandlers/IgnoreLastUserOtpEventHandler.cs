using Common.Module.Keys;
using Eleon.InternalCommons.Lib.Messages.Otp;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Otp.Module.DomainServices;

namespace Eleon.Otp.Module.Eleon.Otp.Module.Application.EventHandlers;
public class IgnoreLastUserOtpEventHandler : IDistributedEventHandler<IgnoreLastUserOtpRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<IgnoreLastUserOtpEventHandler> _logger;
  private readonly OtpDomainService _otpDomainService;

  public IgnoreLastUserOtpEventHandler(IVportalLogger<IgnoreLastUserOtpEventHandler> logger, OtpDomainService otpDomainService)
  {
    _logger = logger;
    _otpDomainService = otpDomainService;
  }

  public async Task HandleEventAsync(IgnoreLastUserOtpRequestMsg eventData)
  {

    try
    {
      var key = new SignInOtpKey(eventData.UserId).ToString();
      await _otpDomainService.IgnoreLastOtp(key);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }
}
