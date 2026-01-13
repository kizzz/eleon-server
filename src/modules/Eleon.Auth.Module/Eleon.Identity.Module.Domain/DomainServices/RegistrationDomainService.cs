using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Identity.Module.CustomCredentials;

namespace VPortal.Identity.Module.DomainServices
{

  public class RegistrationDomainService : DomainService
  {
    private readonly IVportalLogger<RegistrationDomainService> logger;
    private readonly IDistributedEventBus eventBus;
    private readonly IdentityUserManager identityUserManager;
    private readonly ICurrentTenant currentTenant;
    private readonly SignInDomainService signInDomainService;
    private readonly IOptions<IdentityOptions> identityOptions;

    public RegistrationDomainService(
        IVportalLogger<RegistrationDomainService> logger,
        IDistributedEventBus eventBus,
        IdentityUserManager identityUserManager,
        ICurrentTenant currentTenant,
        SignInDomainService signInDomainService,
        IOptions<IdentityOptions> identityOptions)
    {
      this.logger = logger;
      this.eventBus = eventBus;
      this.identityUserManager = identityUserManager;
      this.currentTenant = currentTenant;
      this.signInDomainService = signInDomainService;
      this.identityOptions = identityOptions;
    }

    public async Task<GetIdentitySettingsForRegistrationGotMsg> GetIdentitySettingsForRegistration()
    {
      GetIdentitySettingsForRegistrationGotMsg result = new GetIdentitySettingsForRegistrationGotMsg();
      try
      {
        var request = new GetIdentitySettingsForRegistrationMsg();
        result = await eventBus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(request);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CustomSignInResult> CreateUser(string username, string name = null, string surname = null, string email = null, string phoneNumber = null, string password = null)
    {
      CustomSignInResult result = null;
      try
      {
        var settings = await GetIdentitySettingsForRegistration();
        if (settings == null)
        {
          throw new Exception("Settings are empty");
        }

        if (!settings.EnableSelfRegistration)
        {
          throw new Exception("You don't have permission to create new user");
        }

        var existingUser = await identityUserManager.FindByNameAsync(username);
        if (existingUser != null)
        {
          throw new Exception("Data is already in use, and registration is no longer possible");
        }

        await identityOptions.SetAsync();
        Volo.Abp.Identity.IdentityUser newUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), username, email, currentTenant.Id);
        newUser.Name = name;
        newUser.Surname = surname;
        newUser.SetPhoneNumber(phoneNumber, false);

        if (settings.EnablePassword && string.IsNullOrWhiteSpace(password))
        {
          throw new Exception("You don't have permission to create user without password");
        }

        if (!settings.EnablePassword && !string.IsNullOrWhiteSpace(password))
        {
          throw new Exception("You don't have permission to create user with password");
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
          await identityUserManager.CreateAsync(newUser, password, true);
        }
        else
        {
          await identityUserManager.CreateAsync(newUser);
        }

        result = new CustomSignInResult(SignInResult.Success)
        {
          UserEmail = email,
          UserPhone = phoneNumber,
          UserName = name,
          IsNewUser = true,
          TwoFactorRequired = settings.EnableTwoAuth,
        };
      }
      catch (Exception ex)
      {
        result = new CustomSignInResult(SignInResult.Failed)
        {
          ErrorMessage = ex.Message,
          UserEmail = email,
          UserPhone = phoneNumber,
          UserName = name,
          IsNewUser = true,
        };

        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CustomSignInResult> Resend(string email)
    {
      CustomSignInResult result = new CustomSignInResult(SignInResult.Failed);
      try
      {

        var existingUser = await identityUserManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
          existingUser = await identityUserManager.FindByNameAsync(email);
          if (existingUser == null)
            throw new Exception("User With this login not found");
        }

        var request = new GetOtpByRecipientMsg(email);
        var response = await eventBus.RequestAsync<GetOtpByRecipientGotMsg>(request);
        if (response != null)
        {
          if (!response.IsExpired)
          {
            result = new CustomSignInResult(SignInResult.Failed)
            {
              ErrorMessage = "Registration:MessageAlreadySended",
            };
          }
          else
          {

            var sendCode = await SendCode(email);
            if (sendCode != null && sendCode.SignInResult.Succeeded)
            {
              result = new CustomSignInResult(SignInResult.Success)
              {
                SuccessMessage = "Registration:MessageSend",
              };
            }
            else if (sendCode != null)
            {
              result = new CustomSignInResult(SignInResult.Failed)
              {
                ErrorMessage = sendCode.ErrorMessage,
              };
            }
          }
        }

        result.UserEmail = email;
        result.IsUserPhoneConfirmed = existingUser.PhoneNumberConfirmed;
        result.IsUserEmailConfirmed = existingUser.EmailConfirmed;
        result.UserPhone = existingUser.PhoneNumber;
        result.UserName = existingUser.UserName;
      }
      catch (Exception ex)
      {
        result = new CustomSignInResult(SignInResult.Failed)
        {
          ErrorMessage = ex.Message,
        };
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CustomSignInResult> UserEmailPhoneVerification(string email, string otpValue = null)
    {
      CustomSignInResult result = null;
      try
      {
        var settings = await GetIdentitySettingsForRegistration();
        if (settings == null)
        {
          throw new Exception("Settings are empty");
        }

        if (!settings.EnableSelfRegistration)
        {
          throw new Exception("You don't have permission to create new user");
        }

        var existingUser = await identityUserManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
          existingUser = await identityUserManager.FindByNameAsync(email);
          if (existingUser == null)
            throw new Exception("User With this login not found");
        }

        //need confirm
        if (settings.RequireConfirmedEmail && !existingUser.EmailConfirmed)
        {
          existingUser.SetEmailConfirmed(true);
        }
        else if (settings.RequireConfirmedPhone && !existingUser.PhoneNumberConfirmed)
        {
          existingUser.SetPhoneNumberConfirmed(true);
        }

        result = new CustomSignInResult(SignInResult.Success);
        result.UserEmail = email;
        result.IsUserPhoneConfirmed = existingUser.PhoneNumberConfirmed;
        result.IsUserEmailConfirmed = existingUser.EmailConfirmed;
        result.UserPhone = existingUser.PhoneNumber;
        result.UserName = existingUser.UserName;
      }
      catch (Exception ex)
      {
        result = new CustomSignInResult(SignInResult.Failed)
        {
          ErrorMessage = ex.Message,
        };
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CustomSignInResult> SendCode(string email)
    {
      CustomSignInResult result = new CustomSignInResult(SignInResult.Success);
      try
      {
        var settings = await GetIdentitySettingsForRegistration();
        if (settings == null)
        {
          throw new Exception("Settings are empty");
        }

        if (!settings.EnableSelfRegistration)
        {
          throw new Exception("You don't have permission to create new user");
        }

        var existingUser = await identityUserManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
          existingUser = await identityUserManager.FindByNameAsync(email);
          if (existingUser == null)
            throw new Exception("User With this login not found");
        }

        //GenerateOrp
        //var otp = await signInDomainService.SignInWithExternalLoginInfo(isGenerateOtp: true);

        //if(otp != null)
        //{
        //    result = new CustomSignInResult(SignInResult.Success)
        //    { Recipient = settings.RequireConfirmedEmail && !existingUser.EmailConfirmed ? otp.SuccessMessage : GetSecureRecipientString(existingUser.PhoneNumber) };
        //}

        result.UserEmail = email;
        result.IsUserPhoneConfirmed = existingUser.PhoneNumberConfirmed;
        result.IsUserEmailConfirmed = existingUser.EmailConfirmed;
        result.UserPhone = existingUser.PhoneNumber;
        result.UserName = existingUser.UserName;
      }
      catch (Exception ex)
      {
        result = new CustomSignInResult(SignInResult.Failed)
        {
          ErrorMessage = ex.Message,
        };
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

      return result;
    }

    private string GetSecureRecipientString(string recipient)
    {
      if (recipient.Length <= 6)
      {
        return recipient;
      }

      return recipient[..3] + new string('*', recipient.Length - 6) + recipient[^3..];
    }
  }
}
