using Common.Module.Constants;
using IdentityModel;
using IdentityServer4;
using Logging.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using VPortal.Identity.Module.CustomCredentials;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module.DomainServices
{

  public class LocalSignInDomainService : DomainService
  {
    private readonly IVportalLogger<LocalSignInDomainService> logger;
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> signInManager;
    private readonly IdentityUserManager userManager;
    private readonly ExternalLoginManager externalLoginManager;
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly IIdentitySecurityLogRepository identitySecurityLogRepository;
    private readonly RegistrationDomainService registrationDomainService;
    private readonly SignInDomainService signInDomainService;
    private readonly SignInOtpManager signInOtpManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalSignInDomainService(
        IVportalLogger<LocalSignInDomainService> logger,
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        ExternalLoginManager externalLoginManager,
        TenantSettingsCacheService tenantSettingsCache,
        IdentityUserManager userManager,
        IIdentitySecurityLogRepository identitySecurityLogRepository,
        RegistrationDomainService registrationDomainService,
        SignInDomainService signInDomainService,
        SignInOtpManager signInOtpManager,
        IHttpContextAccessor httpContextAccessor)
    {
      this.logger = logger;
      this.signInManager = signInManager;
      this.userManager = userManager;
      this.externalLoginManager = externalLoginManager;
      this.tenantSettingsCache = tenantSettingsCache;
      this.identitySecurityLogRepository = identitySecurityLogRepository;
      this.registrationDomainService = registrationDomainService;
      this.signInDomainService = signInDomainService;
      this.signInOtpManager = signInOtpManager;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CustomSignInResult> SignInAsync(string login, string password = null)
    {
      CustomSignInResult result = null;
      try
      {
        var user = await userManager.FindByNameAsync(login);
        if (user == null)
        {
          result = new CustomSignInResult(SignInResult.Failed);
        }
        else if (await externalLoginManager.IsUserLockedOut(user))
        {
          result = new CustomSignInResult(SignInResult.LockedOut);
        }
        else
        {
          var settings = await registrationDomainService.GetIdentitySettingsForRegistration();
          if (settings == null)
          {
            return new CustomSignInResult(SignInResult.Failed)
            {
              ErrorMessage = "Settings are empty",
            };
          }

          if (settings.EnablePassword && password.IsNullOrWhiteSpace())
          {
            return new CustomSignInResult(SignInResult.Failed)
            {
              ErrorMessage = "Password is required",
            };
          }

          if (settings.EnablePassword && !password.IsNullOrWhiteSpace())
          {
            var validation = await userManager.CheckPasswordAsync(user, password);
            if (!validation)
            {
              return new CustomSignInResult(SignInResult.Failed)
              {
                ErrorMessage = "Password is invalid"
              };
            }
          }

          if (settings.EnableTwoAuth)
          {
            result = new CustomSignInResult(SignInResult.TwoFactorRequired)
            {
              UserEmail = user.Email,
              UserPhone = user.PhoneNumber,
              IsUserEmailConfirmed = user.EmailConfirmed,
              IsUserPhoneConfirmed = user.PhoneNumberConfirmed,
              IsNewUser = await IsNewUserAsync(user.Id),
            };

            string providerName = ExternalLoginProviderType.Local.ToString();

            if (!await signInDomainService.HasExternalLogin())
            {
              var otpResult = await signInOtpManager.SendOtpGenerationMessage(user);

              if (otpResult.Success)
              {
                result.SuccessMessage = otpResult.Message;
              }
              else
              {
                result.ErrorMessage = otpResult.Message;
              }
            }
            else
            {
              var loginInfo = await signInDomainService.GetExternalLoginInfo();
              providerName = loginInfo.LoginProvider;

              if (providerName == ExternalLoginProviderType.Local.ToString())
              {
                var otpResult = await signInOtpManager.SendOtpGenerationMessage(user);
                if (otpResult.Success)
                {
                  result.SuccessMessage = otpResult.Message;
                }
                else
                {
                  result.ErrorMessage = otpResult.Message;
                }
              }
            }

            await externalLoginManager.SignInHttpContext(user.Id, user.Email, providerName);
          }
          else
          {
            var additionalClaims = new List<System.Security.Claims.Claim>
                        {
                            new Claim("idp", "local"),
                            new Claim("amr", "local"),
                            new Claim("auth_time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                            new Claim(JwtClaimTypes.SessionId, ParseSessionHelper.GenerateSessionId(_httpContextAccessor.HttpContext)),
                        };
            await signInManager.SignInWithClaimsAsync(user, null, additionalClaims);

            result = new CustomSignInResult(SignInResult.Success)
            {
              UserEmail = user.Email,
              UserPhone = user.PhoneNumber,
              IsUserEmailConfirmed = user.EmailConfirmed,
              IsUserPhoneConfirmed = user.PhoneNumberConfirmed,
              IsNewUser = await IsNewUserAsync(user.Id),
            };
          }
        }
      }
      catch (Exception ex)
      {
        logger.Log.LogWarning("Failed to login with credentials: {error}", ex.Message);
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    private async Task<bool> IsNewUserAsync(Guid userId)
    {
      var log = await identitySecurityLogRepository
          .GetListAsync(userId: userId, action: IdentitySecurityLogActionConsts.LoginSucceeded, identity: IdentitySecurityLogIdentityConsts.Identity);
      return log == null || (log != null && log.Count == 0);
    }
  }
}
