using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.EventServices;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class GetUserOtpSettingsEventSerivce :
      IDistributedEventHandler<GetUserOtpSettingsMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetUserOtpSettingsEventSerivce> logger;
    private readonly UserOtpSettingsDomainService userOtpSettingsDomainService;
    private readonly IResponseContext responseContext;

    public GetUserOtpSettingsEventSerivce(
        IVportalLogger<GetUserOtpSettingsEventSerivce> logger,
        UserOtpSettingsDomainService userOtpSettingsDomainService,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.userOtpSettingsDomainService = userOtpSettingsDomainService;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(GetUserOtpSettingsMsg eventData)
    {
      var response = new UserOtpSettingsGotMsg();
      try
      {
        var settings = await userOtpSettingsDomainService.GetUserOtpSettings(eventData.UserId);
        response.Settings = new UserOtpSettingsEto()
        {
          UserOtpType = settings.UserOtpType,
          OtpEmail = settings.OtpEmail,
          OtpPhoneNumber = settings.OtpPhoneNumber,
          UserId = settings.UserId,
        };
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
