using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.Localization;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace VPortal.Identity.Module.DomainServices
{
  public class PasswordRestoreDomainService : DomainService
  {
    private readonly IVportalLogger<PasswordRestoreDomainService> logger;
    private readonly IDistributedEventBus requestClient;
    private readonly IStringLocalizer<IdentityResource> localizer;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IDistributedEventBus eventBus;
    private readonly IdentityUserManager identityUserManager;

    public PasswordRestoreDomainService(
        IVportalLogger<PasswordRestoreDomainService> logger,
        IDistributedEventBus requestClient,
        IStringLocalizer<IdentityResource> localizer,
        IDistributedEventBus massTransitPublisher,
        IHttpContextAccessor httpContextAccessor,
        IDistributedEventBus eventBus,
        IdentityUserManager identityUserManager)
    {
      this.logger = logger;
      this.requestClient = requestClient;
      this.localizer = localizer;
      this.massTransitPublisher = massTransitPublisher;
      this.httpContextAccessor = httpContextAccessor;
      this.eventBus = eventBus;
      this.identityUserManager = identityUserManager;
    }

    public async Task<string> SendRestoreRequest(string email, string username)
    {
      var result = string.Empty;
      try
      {
        var user = await identityUserManager.FindByEmailAsync(email);
        bool userMatched =
            user != null
            && string.Equals(user.UserName, username, StringComparison.InvariantCultureIgnoreCase);
        if (userMatched)
        {
          await SendRestoreLinkToUser(user);
        }
      }
      catch (Exception ex)
      {
        result = ex.Message;
        logger.CaptureAndSuppress(ex);
      }


      return result;
    }

    public async Task<bool> ValidateRestoreCode(string code)
    {
      bool valid = false;
      try
      {
        if (!string.IsNullOrWhiteSpace(code))
        {
          var message = new GetExternalLinkPublicParamsMsg
          {
            LinkCode = code
          };
          var link = await requestClient.RequestAsync<SendExternalLinkPublicParamsMsg>(message);
          valid = link.IsSuccess;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return valid;
    }

    public async Task<bool> ChangePassword(string code, string password)
    {
      bool result = false;
      try
      {
        var message = new GetExternalLinkPrivateParamsMsg();
        message.LinkCode = code;
        var link = await requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(message);
        if (link.IsSuccess && link.PrivateParams.NonEmpty())
        {
          var user = await identityUserManager.FindByIdAsync(link.PrivateParams);
          if (user != null)
          {
            var resetToken = await identityUserManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await identityUserManager.ResetPasswordAsync(user, resetToken, password);
            await identityUserManager.UpdateAsync(user);
            await identityUserManager.UpdateSecurityStampAsync(user);
            result = true;
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task SendRestoreLinkToUser(IdentityUser user)
    {
      try
      {
        string url = $"https://{httpContextAccessor.HttpContext.Request.Host.Value}/auth/Account/RestorePassword?code={{link}}";

        var checkIfLastLinkActive = new CheckLastLinkActiveMsg
        {
          ObjectType = "PasswordRestoreAttempt",
          PrivateParams = user.Id.ToString()
        };
        var checkIfLastLinkActiveResponse = await requestClient.RequestAsync<ActionCompletedMsg>(checkIfLastLinkActive);
        if (!checkIfLastLinkActiveResponse.Success)
        {
          throw new UserFriendlyException("You can request another password reset email in 5 minutes.");
        }

        var linkEntity = new ExternalLinkEto
        {
          DocumentType = "PasswordRestoreAttempt",
          ExternalLinkUrl = url,
          PrivateParams = user.Id.ToString(),
          ExpirationDateTime = DateTime.Now.AddMinutes(5),
        };

        var message = new CreateExternalLinkMsg
        {
          NewExternalLinkEto = linkEntity
        };
        var response = await requestClient.RequestAsync<SendExternalLinkCreatedMsg>(message);
        var link = response.ExternalLinkCreated;


        await SendLinkNotification(user, link.FullLink);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
    }

    private async Task SendLinkNotification(IdentityUser user, string link)
    {
      var userOtpSettings = await GetUserOtpSettings(user.Id);

      string notificationText = localizer["RepeatPassword:LinkMessage", link];
      var notifications = new List<EleonsoftNotification>
            {
                new EleonsoftNotification()
                {
                    Message = notificationText,
                    Type = new MessageNotificationType(),
                    Recipients = new List<RecipientEto>()
                    {
                        new ()
                        {
                            Type = NotificatorRecepientType.Direct,
                            RecipientAddress = user.Email,
                            RefId = user.Id.ToString(),
                        },
                    },
                    RunImmidiate = true
                },
            };
      //if (userOtpSettings.UserOtpType is UserOtpType.Email or UserOtpType.Mixed)
      //{
      //    notifications.Add(new NotificationEto()
      //    {
      //        Data = notificationText,
      //        Type = NotificationType.Email,
      //        Recipients = new List<NotificatorRecepientEto>()
      //        {
      //            new ()
      //            {
      //                Type = NotificatorRecepientType.Direct,
      //                RecipientAddress = userOtpSettings.OtpEmail,
      //                RefId = user.Id.ToString(),
      //            },
      //        },
      //    });
      //}
      //else if (userOtpSettings.UserOtpType is UserOtpType.Sms or UserOtpType.Mixed)
      //{
      //    notifications.Add(new NotificationEto()
      //    {
      //        Data = notificationText,
      //        Type = NotificationType.Sms,
      //        Recipients = new List<NotificatorRecepientEto>()
      //        {
      //            new ()
      //            {
      //                Type = NotificatorRecepientType.Direct,
      //                RecipientAddress = userOtpSettings.OtpPhoneNumber,
      //                RefId = user.Id.ToString(),
      //            },
      //        },
      //    });
      //}

      var request = new NotificatorRequestedBulkMsg()
      {
        Messages = notifications.Select(n => new NotificatorRequestedMsg()
        {
          Notification = n,
        }).ToList(),
      };

      await massTransitPublisher.PublishAsync(request);
    }

    private async Task<UserOtpSettingsEto> GetUserOtpSettings(Guid userId)
    {
      var request = new GetUserOtpSettingsMsg()
      {
        UserId = userId,
      };

      var response = await eventBus.RequestAsync<UserOtpSettingsGotMsg>(request);
      return response.Settings;
    }
  }
}
