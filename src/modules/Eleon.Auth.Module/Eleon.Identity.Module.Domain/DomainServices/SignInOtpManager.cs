using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Keys;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace VPortal.Identity.Module.DomainServices
{
  public class SignInOtpManager : ITransientDependency
  {
    private readonly IDistributedEventBus eventBus;
    private readonly IStringLocalizer<IdentityResource> stringLocalizer;
    private readonly IConfiguration _configuration;
    private readonly ISessionAccessor _sessionAccessor;

    public SignInOtpManager(
        IDistributedEventBus eventBus,
        IStringLocalizer<IdentityResource> stringLocalizer,
        IConfiguration configuration,
        ISessionAccessor sessionAccessor)
    {
      this.eventBus = eventBus;
      this.stringLocalizer = stringLocalizer;
      _configuration = configuration;
      _sessionAccessor = sessionAccessor;
    }

    public async Task<OtpGenerationResultDto> SendOtpGenerationMessage(Volo.Abp.Identity.IdentityUser user)
    {
      var recipients = new List<OtpRecepientEto>();
      // var userOtpSettings = await GetUserOtpSettings(user.Id);
      // var userCustomSignInSettings = await GetUserSignInSettings(user.Id);

      //if(userCustomSignInSettings != null && userCustomSignInSettings.TwoFaNotificationType != null && userCustomSignInSettings.TwoFaNotificationType != TwoFaNotificationType.None)
      //{
      //    if (userCustomSignInSettings.TwoFaNotificationType == TwoFaNotificationType.Email)
      //    {
      //        userOtpSettings.OtpEmail = await userManager.GetEmailAsync(user);
      //        userOtpSettings.UserOtpType = UserOtpType.Email;
      //    }
      //    else if (userCustomSignInSettings.TwoFaNotificationType == TwoFaNotificationType.Sms)
      //    {
      //        userOtpSettings.OtpPhoneNumber = await userManager.GetPhoneNumberAsync(user);
      //        userOtpSettings.UserOtpType = UserOtpType.Sms;
      //    }
      //    else if (userCustomSignInSettings.TwoFaNotificationType == TwoFaNotificationType.Mixed)
      //    {
      //        userOtpSettings.OtpPhoneNumber = await userManager.GetPhoneNumberAsync(user);
      //        userOtpSettings.OtpEmail = await userManager.GetEmailAsync(user);
      //        userOtpSettings.UserOtpType = UserOtpType.Mixed;
      //    }
      //}
      //if (!string.IsNullOrEmpty(userOtpSettings.OtpEmail) && userOtpSettings.UserOtpType is UserOtpType.Email or UserOtpType.Mixed)
      //{
      //    recipients.Add(new()
      //    {
      //        NotificationType = NotificationType.Email,
      //        Recipient = userOtpSettings.OtpEmail,
      //    });
      //}
      //if (!string.IsNullOrEmpty(userOtpSettings.OtpPhoneNumber) &&  userOtpSettings.UserOtpType is UserOtpType.Sms or UserOtpType.Mixed)
      //{
      //    recipients.Add(new()
      //    {
      //        NotificationType = NotificationType.Sms,
      //        Recipient = userOtpSettings.OtpPhoneNumber,
      //    });
      //}

      var settingsResponse = await eventBus.RequestAsync<IdentitySettingsResponseMsg>(new GetIdentitySettingsRequestMsg());

      var settings = settingsResponse.Settings;

      var sendSetting = (settings.FirstOrDefault(x => x.Name == IdentitySettingsConsts.TwoFactorAuthenticationOption)?.Value ?? IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email);

      if (!string.IsNullOrEmpty(user.PhoneNumber) && (sendSetting == IdentitySettingsConsts.TwoFactorAuthenticationOptions.Mixed || sendSetting == IdentitySettingsConsts.TwoFactorAuthenticationOptions.Sms))
      {
        recipients.Add(new()
        {
          // NotificationType = NotificationType.Sms,
          Recipient = user.PhoneNumber,
        });
      }

      if (!string.IsNullOrEmpty(user.Email) && (sendSetting == IdentitySettingsConsts.TwoFactorAuthenticationOptions.Mixed || sendSetting == IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email || recipients.Count == 0))
      {
        recipients.Add(new()
        {
          // NotificationType = NotificationType.Message,
          Recipient = user.Email,
        });
      }

      var session = _sessionAccessor.Session;

      var request = new SendOtpMsg()
      {
        UserId = user.Id,
        Key = GetOTPKey(user.Id),
        Recipients = recipients,
        MessageText = stringLocalizer["OtpMessage"],
        Session = session,
      };

      var response = await eventBus.RequestAsync<OtpSentMsg>(request);

      string recipientsAsString = GetSecureRecipientString(recipients);
      if (response.Result == null)
      {
        throw new Exception($"Unable to send OTP code for {user.Id} {recipientsAsString}");
      }
      else if (response.Result.Success)
      {
        response.Result.Message = recipientsAsString;
      }

      return response.Result;
    }

    public async Task<OtpValidationResultEto> SendOtpValidationMessage(Guid userId, string otp)
    {
      var request = new ValidateOtpMsg()
      {
        Key = GetOTPKey(userId),
        Password = otp,
      };

      var response = await eventBus.RequestAsync<OtpValidatedMsg>(request);
      if (response.Result == null)
      {
        throw new Exception($"Unable to validate OTP code for {userId}");
      }

      return response.Result;
    }

    private static string GetSecureRecipientString(List<OtpRecepientEto> recipients)
        => recipients
        .Select(x => x.Recipient)
        .Select(x => x[..3] + new string('*', x.Length - 6) + x[^3..])
        .JoinAsString(", ");

    private static string GetOTPKey(Guid userId) => new SignInOtpKey(userId).ToString();

    private async Task<UserOtpSettingsEto> GetUserOtpSettings(Guid userId)
    {
      var request = new GetUserOtpSettingsMsg()
      {
        UserId = userId,
      };

      var response = await eventBus.RequestAsync<UserOtpSettingsGotMsg>(request);
      return response.Settings;
    }

    private async Task<GetUserSignInSettingGotMsg> GetUserSignInSettings(Guid userId)
    {
      var request = new GetUserSignInSettingMsg()
      {
        UserId = userId,
      };

      var response = await eventBus.RequestAsync<GetUserSignInSettingGotMsg>(request);
      return response;
    }
  }
}
