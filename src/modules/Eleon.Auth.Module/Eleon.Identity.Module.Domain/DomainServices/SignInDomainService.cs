using Authorization.Module.ClientIsolation;
using Authorization.Module.TenantHostname;
using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using ExternalLogin.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using VPortal.Identity.Module.CustomCredentials;
using VPortal.Identity.Module.Localization;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace VPortal.Identity.Module.DomainServices
{

  public class SignInDomainService : DomainService
  {
    private readonly IVportalLogger<SignInDomainService> logger;
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> signInManager;
    private readonly IOptions<IdentityOptions> identityOptions;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IdentitySecurityLogManager identitySecurityLogManager;
    private readonly SignInOtpManager signInOtpManager;
    private readonly ClientIsolationValidator clientIsolationValidator;
    private readonly ExternalLoginManager externalLoginManager;
    private readonly TenantUrlResolver tenantUrlResolver;
    private readonly IConfiguration configuration;
    private readonly IDistributedEventBus eventBus;
    private readonly IStringLocalizer<IdentityResource> _localizer;

    public SignInDomainService(
        IVportalLogger<SignInDomainService> logger,
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        IOptions<IdentityOptions> identityOptions,
        IHttpContextAccessor httpContextAccessor,
        IdentitySecurityLogManager identitySecurityLogManager,
        SignInOtpManager signInOtpManager,
        ClientIsolationValidator clientIsolationValidator,
        ExternalLoginManager externalLoginManager,
        TenantUrlResolver tenantUrlResolver,
        IConfiguration configuration,
        IDistributedEventBus eventBus,
        IStringLocalizer<IdentityResource> localizer)
    {
      this.logger = logger;
      this.signInManager = signInManager;
      this.identityOptions = identityOptions;
      this.httpContextAccessor = httpContextAccessor;
      this.identitySecurityLogManager = identitySecurityLogManager;
      this.signInOtpManager = signInOtpManager;
      this.clientIsolationValidator = clientIsolationValidator;
      this.externalLoginManager = externalLoginManager;
      this.tenantUrlResolver = tenantUrlResolver;
      this.configuration = configuration;
      this.eventBus = eventBus;
      _localizer = localizer;
    }

    public async Task<CustomSignInResult> SignInWithOtpAsync(string otp = null, bool isRegistrationProcess = false)
    {
      try
      {
        if (!await HasExternalLogin())
        {
          return new CustomSignInResult(SignInResult.Failed)
          {
            ErrorMessage = _localizer["Error:ExternalLoginInfo:NotFound"],
          };
        }

        var loginInfo = await GetExternalLoginInfo();

        var user = await externalLoginManager.GetOrCreateUser(loginInfo);

        //var clientIsolationValidationResult = await clientIsolationValidator.ValidateClientIsolation(httpContextAccessor.HttpContext, user.Id);
        //if (clientIsolationValidationResult.ValidationResult == ClientIsolationValidationResult.MissingClientCert)
        //{
        //    var isSecureUrl = await tenantUrlResolver.IsSecureHost(httpContextAccessor.HttpContext, CurrentTenant.Id);
        //    return new CustomSignInResult(SignInResult.Failed)
        //    {
        //        RequireSecureApi = !isSecureUrl,
        //    };
        //}

        if (await externalLoginManager.IsUserLockedOut(user))
        {
          return new CustomSignInResult(SignInResult.LockedOut);
        }

        var twoAuthEnableInSettings = await GetTwoFactorEnabledIdentitySetting();

        bool requireTwoFactor = twoAuthEnableInSettings && loginInfo.LoginProvider == ExternalLoginProviderType.Local.ToString() && !isRegistrationProcess;

        if (requireTwoFactor && string.IsNullOrEmpty(otp))
        {
          var result = await GenerateOTP(user);

          await identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
          {
            Identity = loginInfo.LoginProvider,
            Action = ExternalLoginSecurityLogActions.RequestedTwoFactor,
            UserName = user.UserName,
          });

          return result;
        }
        else
        {
          var loginResult = requireTwoFactor ? await SignInWithOtp(user, otp) : new CustomSignInResult(SignInResult.Success);

          if (!loginResult.SignInResult.Succeeded)
          {
            await identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
              Identity = loginInfo.LoginProvider,
              Action = ExternalLoginSecurityLogActions.TwoFactorFailedAttempt,
              UserName = user.UserName,
            });
          }
          else
          {
            await externalLoginManager.PerformTokenFederation(loginInfo, []);

            await identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
              Identity = loginInfo.LoginProvider,
              Action = ExternalLoginSecurityLogActions.LoginWithExternalProvider,
              UserName = user.UserName,
            });
          }

          return loginResult;
        }
      }
      catch (Exception ex)
      {
        logger.Log.LogWarning("Failed to login with otp: {error}", ex.Message);
        logger.Capture(ex);
      }
      finally
      {
      }

      return null;
    }

    public async Task<bool> HasExternalLogin()
    {
      var loginInfo = await signInManager.GetExternalLoginInfoAsync();
      return loginInfo != null;
    }

    public async Task ClearExternalLogin()
    {
      httpContextAccessor.HttpContext.Response.Cookies.Delete(IdentityConstants.ExternalScheme, new CookieOptions()
      {
        Path = "/",
        HttpOnly = true,
      });
    }

    private async Task<CustomSignInResult> GenerateOTP(Volo.Abp.Identity.IdentityUser user)
    {
      var otpGenerationResult = await signInOtpManager.SendOtpGenerationMessage(user);
      if (otpGenerationResult.Success)
      {
        return new CustomSignInResult(SignInResult.TwoFactorRequired)
        {
          SuccessMessage = otpGenerationResult.Message,
        };
      }

      return new CustomSignInResult(SignInResult.Failed)
      {
        ErrorMessage = otpGenerationResult.Message,
      };
    }

    private async Task<CustomSignInResult> SignInWithOtp(Volo.Abp.Identity.IdentityUser user, string otp)
    {
      try
      {
        var otpValidationResult = await signInOtpManager.SendOtpValidationMessage(user.Id, otp);
        if (!otpValidationResult.Valid)
        {
          return new CustomSignInResult(SignInResult.TwoFactorRequired)
          {
            ErrorMessage = otpValidationResult.ErrorMessage,
          };
        }

        return new CustomSignInResult(SignInResult.Success);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
        return new CustomSignInResult(SignInResult.Failed)
        {
          ErrorMessage = "Failed to validate otp"
        };
      }
    }

    public async Task<ExternalLoginInfo> GetExternalLoginInfo()
    {
      if (identityOptions is Volo.Abp.Options.AbpDynamicOptionsManager<IdentityOptions>)
      {
        await identityOptions.SetAsync();
      }

      var loginInfo = await signInManager.GetExternalLoginInfoAsync();
      if (loginInfo == null)
      {
        throw new UserFriendlyException("External login info is not available!");
      }

      return loginInfo;
    }

    private async Task<bool> GetTwoFactorEnabledIdentitySetting()
    {
      bool result = false;
      try
      {
        var request = new GetIdentitySettingsForRegistrationMsg();
        var response = await eventBus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(request);
        result = response.EnableTwoAuth;
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
  }
}
