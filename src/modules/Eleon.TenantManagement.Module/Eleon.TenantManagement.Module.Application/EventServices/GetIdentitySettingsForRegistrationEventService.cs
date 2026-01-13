using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.Settings;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class GetIdentitySettingsForRegistrationEventService :
      IDistributedEventHandler<GetIdentitySettingsForRegistrationMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetIdentitySettingsForRegistrationEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly IdentitySettingAppService identitySettingAppService;

    public GetIdentitySettingsForRegistrationEventService(
        IVportalLogger<GetIdentitySettingsForRegistrationEventService> logger,
        IResponseContext responseContext,
        IdentitySettingAppService identitySettingAppService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.identitySettingAppService = identitySettingAppService;
    }

    public async Task HandleEventAsync(GetIdentitySettingsForRegistrationMsg eventData)
    {
      var message = new GetIdentitySettingsForRegistrationGotMsg();
      try
      {
        var result = await identitySettingAppService.GetIdentitySettings();
        if (result != null)
        {
          message.EnablePassword = result.FirstOrDefault(x => x.Name == "PasswordEnable")?.Value.ToLower() == "true";
          message.EnableTwoAuth = result.FirstOrDefault(x => x.Name == "TwoFactorAuthenticationEnable")?.Value.ToLower() == "true";
          message.TwoAuthOption = result.FirstOrDefault(x => x.Name == "TwoFactorAuthenticationOption")?.Value;
          message.SmsProviderOption = result.FirstOrDefault(x => x.Name == "SmsProviderOption")?.Value;
          message.AllowChangeEmail = result.FirstOrDefault(x => x.Name == "Abp.Identity.User.IsEmailUpdateEnabled")?.Value.ToLower() == "true";
          message.AllowChangeUserName = result.FirstOrDefault(x => x.Name == "Abp.Identity.User.IsUserNameUpdateEnabled")?.Value.ToLower() == "true";
          message.EnableSelfRegistration = result.FirstOrDefault(x => x.Name == "SelfRegistrationEnable")?.Value.ToLower() == "true";
          message.RequireConfirmedEmail = result.FirstOrDefault(x => x.Name == "Abp.Identity.SignIn.RequireConfirmedEmail")?.Value.ToLower() == "true";
          message.RequireConfirmedPhone = result.FirstOrDefault(x => x.Name == "Abp.Identity.SignIn.RequireConfirmedPhoneNumber")?.Value.ToLower() == "true";

          message.PasswordRequireDigit = result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequireDigit")?.Value.ToLower() == "true";
          message.PasswordRequiredLength = int.Parse(result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequiredLength")?.Value);
          message.PasswordRequiredUniqueChars = int.Parse(result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequiredUniqueChars")?.Value);
          message.PasswordRequireNonAlphanumeric = result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequireNonAlphanumeric")?.Value.ToLower() == "true";
          message.PasswordRequireLowercase = result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequireLowercase")?.Value.ToLower() == "true";
          message.PasswordRequireUppercase = result.FirstOrDefault(x => x.Name == "Abp.Identity.Password.RequireUppercase")?.Value.ToLower() == "true";
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(message);
      }

    }
  }
}
