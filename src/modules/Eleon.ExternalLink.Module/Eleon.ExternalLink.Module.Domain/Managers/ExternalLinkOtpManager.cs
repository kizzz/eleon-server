using Common.EventBus.Module;
using Common.Module.Constants;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity.Localization;

namespace VPortal.ExternalLink.Module.Managers
{
  public class ExternalLinkOtpManager : ITransientDependency
  {
    private readonly IDistributedEventBus sendOtpClient;
    private readonly IStringLocalizer<IdentityResource> stringLocalizer;
    private readonly IDistributedEventBus validateOtpClient;

    public ExternalLinkOtpManager(
        IDistributedEventBus sendOtpClient,
        IStringLocalizer<IdentityResource> stringLocalizer,
        IDistributedEventBus validateOtpClient)
    {
      this.sendOtpClient = sendOtpClient;
      this.stringLocalizer = stringLocalizer;
      this.validateOtpClient = validateOtpClient;
    }

    public async Task<OtpGenerationResultDto> SendOtpGenerationMessage(string externalLinkCode, ExternalLinkLoginType type, string key)
    {

      var recipients = new List<OtpRecepientEto>();
      if (type is ExternalLinkLoginType.Email)
      {
        recipients.Add(new()
        {
          // NotificationType = NotificationType.Email,
          Recipient = key,
        });
      }
      else if (type is ExternalLinkLoginType.Sms)
      {
        recipients.Add(new()
        {
          // NotificationType = NotificationType.Sms,
          Recipient = key,
        });
      }

      var request = new SendOtpMsg()
      {
        Key = GetOTPKey(externalLinkCode),
        Recipients = recipients,
        MessageText = stringLocalizer["OtpMessage"],
      };

      var response = await sendOtpClient.RequestAsync<OtpSentMsg>(request);

      string recipientsAsString = GetSecureRecipientString(recipients);
      if (response.Result == null)
      {
        throw new Exception($"Unable to send OTP code for {externalLinkCode} {recipientsAsString}");
      }
      else if (response.Result.Success)
      {
        response.Result.Message = recipientsAsString;
      }

      return response.Result;
    }

    public async Task<OtpValidationResultEto> SendOtpValidationMessage(string externalLinkCode, string otp)
    {
      var request = new ValidateOtpMsg
      {
        Key = GetOTPKey(externalLinkCode),
        Password = otp,
      };

      var response = await validateOtpClient.RequestAsync<OtpValidatedMsg>(request);
      if (response.Result == null)
      {
        throw new Exception($"Unable to validate OTP code for {externalLinkCode}");
      }

      return response.Result;
    }

    private static string GetSecureRecipientString(List<OtpRecepientEto> recipients)
        => recipients
        .Select(x => x.Recipient)
        .Select(x => x.Substring(0, 3) + new string('*', x.Length - 6) + x.Substring(x.Length - 3))
        .JoinAsString(", ");


    private static string GetOTPKey(string externalLinkCode) => $"PWD_SIGNIN;{externalLinkCode}";
  }
}
