using Common.Module.Constants;
using ExternalLogin.Module;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Migrations.Module;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using VPortal.Identity.Module.ExternalProviders;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module.DomainServices
{
  public class ExternalLoginManager : ITransientDependency
  {
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> signInManager;
    private readonly IdentityUserManager userManager;
    private readonly IGuidGenerator guidGenerator;
    private readonly ICurrentTenant currentTenant;
    private readonly IOptions<IdentityOptions> identityOptions;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly IdentitySecurityLogManager identitySecurityLogManager;

    public ExternalLoginManager(
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        IdentityUserManager userManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IOptions<IdentityOptions> identityOptions,
        IHttpContextAccessor httpContextAccessor,
        TenantSettingsCacheService tenantSettingsCache,
        IdentitySecurityLogManager identitySecurityLogManager)
    {
      this.signInManager = signInManager;
      this.userManager = userManager;
      this.guidGenerator = guidGenerator;
      this.currentTenant = currentTenant;
      this.identityOptions = identityOptions;
      this.httpContextAccessor = httpContextAccessor;
      this.tenantSettingsCache = tenantSettingsCache;
      this.identitySecurityLogManager = identitySecurityLogManager;
    }

    /// <summary>
    /// Gets user form ExternalLoginInfo by email, and replace external auth with local auth.
    /// </summary>
    /// <param name="loginInfo">External login information (usually got from login context).</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PerformTokenFederation(ExternalLoginInfo loginInfo, List<Claim> additionalClaims)
    {
      var user = await GetOrCreateUser(loginInfo);

      await httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

      await SignInWithClaims(user, loginInfo, additionalClaims);
    }

    public async Task<Volo.Abp.Identity.IdentityUser> GetOrCreateUser(ExternalLoginInfo loginInfo)
    {
      var user = await userManager.FindByEmailAsync(GetEmail(loginInfo));
      if (user == null)
      {
        user = await CreateExternalUserAsync(loginInfo);
      }
      else if (loginInfo.LoginProvider != "Local")
      {
        if (await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
        {
          CheckIdentityErrors(await userManager.AddLoginAsync(user, loginInfo));
        }
      }

      return user;
    }

    public async Task<bool> IsUserLockedOut(Volo.Abp.Identity.IdentityUser user)
    {
      if (currentTenant.Id == null)
      {
        return false;
      }

      if (await userManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue))
      {
        return false;
      }

      var settings = await tenantSettingsCache.GetTenantSettings(currentTenant.Id);
      return !settings.IsActive;
    }

    public async Task SignInHttpContext(Guid userId, string userEmail, string loginProvider)
    {
      var simpleIdentity = new ClaimsIdentity(IdentityConstants.ExternalScheme, ClaimTypes.NameIdentifier, ClaimTypes.Role);
      simpleIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userEmail));
      simpleIdentity.AddClaim(new Claim(AbpClaimTypes.UserId, userId.ToString()));
      if (currentTenant.Id.HasValue)
      {
        simpleIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, currentTenant.Id.ToString()));
      }

      {
        var sid = ParseSessionHelper.GenerateSessionId(httpContextAccessor.HttpContext);
        simpleIdentity.AddClaim(new Claim(JwtClaimTypes.SessionId, sid));
        simpleIdentity.AddClaim(new Claim("idp", loginProvider));
        simpleIdentity.AddClaim(new Claim("amr", "external"));
        simpleIdentity.AddClaim(new Claim("auth_time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));
      }

      var userPrincipal = new ClaimsPrincipal([simpleIdentity]);
      var authItems = new Dictionary<string, string>() { ["LoginProvider"] = loginProvider };
      var authenticationProps = new AuthenticationProperties(authItems)
      {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
        RedirectUri = null,
      };

      await httpContextAccessor.HttpContext.SignInAsync(
          IdentityConstants.ExternalScheme,
          userPrincipal,
          authenticationProps);
      httpContextAccessor.HttpContext.Response.StatusCode = 200;
      httpContextAccessor.HttpContext.Response.Headers.Remove("Location");
    }

    private async Task SignInWithClaims(Volo.Abp.Identity.IdentityUser user, ExternalLoginInfo loginInfo, List<Claim> additionalClaims)
    {
      // var props = GetAuthenticationProperties(loginInfo);
      var claims = GetAdditionalClaims(loginInfo).Concat(additionalClaims).ToList();
      await signInManager.SignInWithClaimsAsync(user, true, claims); // props
    }

    private AuthenticationProperties GetAuthenticationProperties(ExternalLoginInfo loginInfo)
    {
      var tokens = new List<AuthenticationToken>() { loginInfo.AuthenticationTokens.FirstOrDefault(x => x.Name == "id_token") };

      var props = new AuthenticationProperties();

      if (tokens != null)
      {
        props.StoreTokens(tokens);
      }

      return props;
    }

    private List<Claim> GetAdditionalClaims(ExternalLoginInfo loginInfo)
    {
      var claims = new List<Claim>();
      var sid = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
      if (sid != null)
      {
        claims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
      }
      else
      {
        claims.Add(new Claim(JwtClaimTypes.SessionId, ParseSessionHelper.GenerateSessionId(httpContextAccessor.HttpContext)));
      }

      claims.Add(new Claim("idp", loginInfo.LoginProvider));
      claims.Add(new Claim("amr", "external"));
      claims.Add(new Claim("auth_time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));

      return claims;
    }

    private async Task<Volo.Abp.Identity.IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
      await identityOptions.SetAsync();

      var user = ExternalProviderHelper.CreateUserFromExternalData(guidGenerator.Create(), currentTenant.Id, info);

      CheckIdentityErrors(await userManager.CreateAsync(user));
      CheckIdentityErrors(await userManager.SetEmailAsync(user, user.Email));
      CheckIdentityErrors(await userManager.AddLoginAsync(user, info));
      CheckIdentityErrors(await userManager.AddDefaultRolesAsync(user));

      user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
      user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);

      var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
      if (!phoneNumber.IsNullOrWhiteSpace())
      {
        var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);
        user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
      }

      await userManager.UpdateAsync(user);

      await AddAdminRoleIfNeeded(user, GetProviderType(info));

      await identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = info.LoginProvider,
        Action = ExternalLoginSecurityLogActions.ProvisionExternalUser,
        UserName = user.UserName,
      });

      return user;
    }

    private async Task AddAdminRoleIfNeeded(Volo.Abp.Identity.IdentityUser user, ExternalLoginProviderType providerType)
    {
      if (user.Email.IsNullOrWhiteSpace() || providerType == ExternalLoginProviderType.None)
      {
        return;
      }

      var settings = await tenantSettingsCache.GetTenantSettings(currentTenant.Id);
      string adminEmail = settings.LoginProviders.FirstOrDefault(x => x.Type == providerType)?.AdminIdentifier;
      if (adminEmail.IsNullOrWhiteSpace())
      {
        return;
      }

      if (user.Email == adminEmail)
      {
        await userManager.AddToRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);
      }
    }

    private ExternalLoginProviderType GetProviderType(ExternalLoginInfo info)
        => Enum.TryParse<ExternalLoginProviderType>(info.LoginProvider, out var parsed) ? parsed : ExternalLoginProviderType.None;

    private void CheckIdentityErrors(IdentityResult identityResult)
    {
      if (!identityResult.Succeeded)
      {
        throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
      }
    }

    private string GetEmail(ExternalLoginInfo loginInfo)
    {
      var email =
          loginInfo.Principal.FindFirstValue(AbpClaimTypes.Email)
          ?? loginInfo.Principal.Identity.Name;

      if (email.IsNullOrWhiteSpace())
      {
        throw new UserFriendlyException("Cannot retreive user email from the external provider!");
      }

      return email;
    }
  }
}
