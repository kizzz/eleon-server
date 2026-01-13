using Common.Module.Constants;
using Common.Module.Extensions;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftModuleCollector.Commons.Module.Messages;
using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftSdk.modules.Messager.Module.Abstractions;
using EleonsoftSdk.modules.Messager.Module.Statics;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using VPortal.Otp.Module.Entities;
using VPortal.Otp.Module.Localization;
using VPortal.Otp.Module.Repositories;

namespace VPortal.Otp.Module.DomainServices
{

  public class OtpDomainService : DomainService
  {
    private const int OTPDuration = 5 * 60;
    private const int OTPLength = 6;
    private const int OTPMaxAttemptsExceededDelay = 30 * 60;
    private const int OTPMaxFailedValidationAttempts = 3;

    private readonly int OTPRetryDelay = 30;
    private readonly int OTPMaxRetryAttempts = 5;

    private readonly bool NotifyImmidiately = true;

    private readonly IVportalLogger<OtpDomainService> logger;
    private readonly IStringLocalizer<OtpResource> L;
    private readonly IDistributedEventBus eventBus;
    private readonly IConfiguration configuration;
    private readonly IOtpRepository otpRepository;
    private readonly ISessionAccessor _sessionAccessor;
    private readonly IdentityUserManager _userManager;
    private readonly IJsonSerializer _jsonSerializer;

    public OtpDomainService(
        IVportalLogger<OtpDomainService> logger,
        IStringLocalizer<OtpResource> l,
        IDistributedEventBus massTransitPublisher,
        IConfiguration configuration,
        IOtpRepository otpRepository,
        ISessionAccessor sessionAccessor,
        IdentityUserManager userManager,
        IJsonSerializer jsonSerializer)
    {
      this.logger = logger;
      this.L = l;
      this.eventBus = massTransitPublisher;
      this.configuration = configuration;
      this.otpRepository = otpRepository;
      _sessionAccessor = sessionAccessor;
      _userManager = userManager;
      _jsonSerializer = jsonSerializer;
      OTPMaxRetryAttempts = configuration.GetValue("OTP:MaxRetryAttempts", 5);
      OTPRetryDelay = configuration.GetValue("OTP:ResendTime", 30);
      if (OTPRetryDelay <= 0)
      {
        OTPRetryDelay = 30;
      }
      NotifyImmidiately = configuration.GetValue("OTP:NotifyImmidiately", true);
    }

    public async Task<OtpGenerationResultDto> GenerateOtpAsync(string key, List<OtpRecepientEto> recipients, string message, Guid? userId = null, FullSessionInformation session = null)
    {
      OtpGenerationResultDto result = null;
      try
      {
        var (generateAllowed, retryAttempt, error) = await ValidateGeneration(key);
        if (generateAllowed)
        {
          string recipientsString = recipients.Select(x => x.Recipient).JoinAsString(", ");
          var otpEntity = await GenerateAndInsertOtp(key, recipientsString, retryAttempt);
          await SendNotificationAsync(recipients, otpEntity.Password, userId, session ?? _sessionAccessor.Session);
          var expirationUtc = otpEntity.CreationTime.AddSeconds(otpEntity.DurationS);
          result = new()
          {
            ExpirationUtcDate = expirationUtc,
            CreationUtcDate = otpEntity.CreationTime,
            DurationSeconds = OTPDuration,
            Success = true,
            Message = L["OtpValidForMinutes", otpEntity.Recipient, OTPDuration / 60],
          };
        }
        else
        {
          result = new()
          {
            Success = false,
            Message = error,
          };
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<OtpValidationResultEto> ValidateOtp(string key, string password)
    {

      OtpValidationResultEto result = null;
      try
      {
        var otp = await otpRepository.FindByKey(key);

        OtpValidationResultEto validationResult = null;
        if (configuration.GetValue<bool>("OTP:AllowAnyOTP", false))
        {
          validationResult = new(true, null);
        }
        else
        {
          if (otp.FailedValidationAttempts >= OTPMaxFailedValidationAttempts)
          {
            validationResult = new(false, L["Error:FailedAttemptsCount"]);
          }
          else if (otp == null || otp.Password != password)
          {
            validationResult = new(false, L["Error:Invalid"]);
          }
          else if (otp.IsExpired())
          {
            validationResult = new(false, L["Error:Expired"]);
          }
          else if (otp.IsUsed)
          {
            validationResult = new(false, L["Error:AlreadyUsed"]);
          }
          else
          {
            validationResult = new(true, null);
          }
        }

        if (validationResult.Valid)
        {
          otp.MarkAsUsed();
        }
        else if (otp != null)
        {
          otp.FailedValidationAttempts++;
        }

        await otpRepository.UpdateAsync(otp, true);
        result = validationResult;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task IgnoreLastOtp(string key)
    {
      try
      {
        var otp = await otpRepository.FindByKey(key);
        if (otp != null)
        {
          otp.IsIgnored = true;
          await otpRepository.UpdateAsync(otp);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<OtpEntity> GetOtpByRecipient(string recipient)
    {
      OtpEntity result = null;
      try
      {
        result = await otpRepository.GetOtpByRecipient(recipient);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task SendNotificationAsync(List<OtpRecepientEto> recipients, string password, Guid? userId, FullSessionInformation? session)
    {
      var notificatorRequestedBulkMsg = new NotificatorRequestedBulkMsg();

      var userName = string.Empty;
      if (userId.HasValue && userId != Guid.Empty)
      {
        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user != null)
        {
          userName = user.UserName;
        }
      }
      if (string.IsNullOrWhiteSpace(userName))
      {
        userName = recipients.Select(x => x.Recipient).JoinAsString(", ");
      }

      foreach (var recipient in recipients)
      {
        var id = GuidGenerator.Create();
        var notificationMessage = new SendInternalNotificationsMsg(new EleonsoftNotification
        {
          Id = id,
          Type = new TwoFactorNotificationType
          {
            UserName = userName,
            Session = session
          },
          Priority = EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationPriority.Normal,
          Message = password,
          Recipients = new List<RecipientEto>()
                    {
                        new RecipientEto()
                        {
                            Type = NotificatorRecepientType.Direct,
                            RecipientAddress = recipient.Recipient,
                            RefId = userId?.ToString(),
                        },
                    },
          RunImmidiate = true
        });

        await eventBus.PublishAsync(notificationMessage, onUnitOfWorkComplete: false);
      }
    }

    private async Task<OtpEntity> GenerateAndInsertOtp(string key, string recipient, int retryAttempt)
    {
      var otpEntity = new OtpEntity(GuidGenerator.Create())
      {
        Key = key,
        Recipient = recipient,
        Password = GeneratePassword(),
        DurationS = OTPDuration,
        RetryAttempt = retryAttempt,
        FailedValidationAttempts = 0,
        CreationTime = DateTime.UtcNow,
      };

      await otpRepository.InsertAsync(otpEntity);
      return otpEntity;
    }

    private async Task<(bool generateAllowed, int retryAttempt, string error)> ValidateGeneration(string key)
    {
      var lastOtp = await otpRepository.FindByKey(key);

      int retryAttempt = 0;
      string error = null;
      if (lastOtp != null)
      {
        bool regenerateAllowed = lastOtp.IsIgnored || lastOtp.IsUsed || DelayPassed(lastOtp.CreationTime, OTPMaxAttemptsExceededDelay);
        bool retryPossible = lastOtp.RetryPossible(OTPMaxRetryAttempts);
        bool retryAllowed = DelayPassed(lastOtp.CreationTime, OTPRetryDelay);
        bool isStub = AllowAnyOTP();
        if (regenerateAllowed || isStub)
        {
          retryAttempt = 0;
        }
        else if (retryPossible && retryAllowed)
        {
          retryAttempt = lastOtp.RetryAttempt + 1;
        }
        else if (retryPossible)
        {
          error = L.GetString("Error:RetryTooSoon", OTPRetryDelay);
        }
        else
        {
          error = L.GetString("Error:RegenerateTooSoon", OTPMaxAttemptsExceededDelay / 60);
        }
      }

      return (error.IsNullOrEmpty(), retryAttempt, error);
    }

    private bool AllowAnyOTP()
        => bool.TryParse(configuration["OTP:AllowAnyOTP"], out bool useStub) && useStub;

    private static bool DelayPassed(DateTime creationTime, int delaySeconds)
        => DateTime.Now > creationTime.AddSeconds(delaySeconds);

    private static string GeneratePassword()
    {
      const string possibleChars = "1234567890";
      StringBuilder res = new StringBuilder();
      for (int i = 0; i < OTPLength; i++)
      {
        int randIx = RandomNumberGenerator.GetInt32(possibleChars.Length - 1);
        res.Append(possibleChars[randIx]);
      }

      return res.ToString();
    }
  }
}
