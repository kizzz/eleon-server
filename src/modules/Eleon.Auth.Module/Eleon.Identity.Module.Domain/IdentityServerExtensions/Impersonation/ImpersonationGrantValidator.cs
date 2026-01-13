using Common.Module.Constants;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Migrations.Module;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.IdentityServerExtensions.Impersonation
{
  public class ImpersonationGrantValidator : IExtensionGrantValidator
  {
    private readonly ICurrentTenant currentTenant;
    private readonly ITokenValidator tokenValidator;
    private readonly ImpersonationDomainService impersonationDomainService;
    private readonly IdentityUserManager identityUserManager;
    private readonly ICurrentPrincipalAccessor currentPrincipalAccessor;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IVportalLogger<ImpersonationGrantValidator> logger;

    public ImpersonationGrantValidator(
        ICurrentTenant currentTenant,
        ITokenValidator tokenValidator,
        ImpersonationDomainService impersonationDomainService,
        IdentityUserManager identityUserManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IHttpContextAccessor httpContextAccessor,
        IVportalLogger<ImpersonationGrantValidator> logger)
    {
      this.currentTenant = currentTenant;
      this.tokenValidator = tokenValidator;
      this.impersonationDomainService = impersonationDomainService;
      this.identityUserManager = identityUserManager;
      this.currentPrincipalAccessor = currentPrincipalAccessor;
      this.httpContextAccessor = httpContextAccessor;
      this.logger = logger;
    }

    public string GrantType => VPortalExtensionGrantsConsts.Names.ImpresonationGrant;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
      try
      {
        var claims = await GetTokenClaims();

        Guid? actingAsTenantId = GetTenantId(claims);
        Guid actingAsUserId = GetUserId(claims);

        Guid? actorTenantId = GetImpersonatorTenantId(claims, actingAsTenantId);
        Guid actorUserId = GetImpersonatorUserId(claims) ?? actingAsUserId;

        Guid? tryingToImpersonateAsTenantId = GetImpersonatedTenantId(context.Request, actingAsTenantId);
        Guid tryingToImpersonateAsUserId = await GetImpersonatedUserId(context.Request, actorTenantId, tryingToImpersonateAsTenantId);

        await EnsureCanImpersonate(
            actorTenantId,
            actorUserId,
            actingAsTenantId,
            actingAsUserId,
            tryingToImpersonateAsTenantId,
            tryingToImpersonateAsUserId);

        using (currentTenant.Change(tryingToImpersonateAsTenantId))
        {
          context.Result = GetSuccessGrantValidationResult(
              actorTenantId,
              actorUserId,
              actingAsTenantId,
              actingAsUserId,
              tryingToImpersonateAsTenantId,
              tryingToImpersonateAsUserId);
        }
      }
      catch (Exception ex)
      {
        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
        logger.CaptureAndSuppress(ex);
      }

    }

    private GrantValidationResult GetSuccessGrantValidationResult(
        Guid? actorTenantId,
        Guid actorUserId,
        Guid? actingAsTenantId,
        Guid actingAsUserId,
        Guid? tryingToImpersonateAsTenantId,
        Guid tryingToImpersonateAsUserId)
    {
      var newClaims = new List<Claim>()
                    {
                        new Claim(VPortalExtensionGrantsConsts.Impresonation.ImpersonatorUserClaim, actorUserId.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.Impresonation.ImpersonatorTenantClaim, actorTenantId?.ToString() ?? "host"),
                    };

      if (tryingToImpersonateAsTenantId.HasValue)
      {
        newClaims.Add(new Claim(AbpClaimTypes.TenantId, tryingToImpersonateAsTenantId.ToString()));
      }

      return new GrantValidationResult(tryingToImpersonateAsUserId.ToString(), GrantType, newClaims, ExternalLoginProviderType.Local.ToString());
    }

    private async Task EnsureCanImpersonate(
        Guid? actorTenantId,
        Guid actorUserId,
        Guid? actingAsTenantId,
        Guid actingAsUserId,
        Guid? tryingToImpersonateAsTenantId,
        Guid tryingToImpersonateAsUserId)
    {
      using (currentTenant.Change(actingAsTenantId))
      {
        bool allowedToImpersonate = await impersonationDomainService.CheckIfCanImpersonate(
            actorTenantId,
            actorUserId,
            actingAsTenantId,
            actingAsUserId,
            tryingToImpersonateAsTenantId,
            tryingToImpersonateAsUserId);

        if (!allowedToImpersonate)
        {
          throw new Exception("You are not allowed to impersonate this user.");
        }
      }
    }

    private async Task<IEnumerable<Claim>> GetTokenClaims()
    {
      //string token = GetAccessTokenParameter(request.Raw);
      //var validatedToken = await tokenValidator.ValidateAccessTokenAsync(token, "VPortal");
      //if (validatedToken.IsError)
      //{
      //    throw InvalidTokenException();
      //}

      var identity = currentPrincipalAccessor.Principal.Identities.FirstOrDefault();
      if (identity == null)
      {
        throw InvalidTokenException();
      }

      return identity.Claims;
    }

    private async Task<Guid> GetImpersonatedUserId(ValidatedTokenRequest request, Guid? actorTenantId, Guid? tryingToImpersonateAsTenantId)
    {
      var impersonatedUser = GetImpersonatedUserParameter(request.Raw);
      if (Guid.TryParse(impersonatedUser, out var userId))
      {
        return userId;
      }

      if (actorTenantId == null && tryingToImpersonateAsTenantId != actorTenantId)
      {
        using (currentTenant.Change(tryingToImpersonateAsTenantId))
        {
          var admins = await identityUserManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);
          return admins.First().Id;
        }
      }

      throw new Exception("Impersonated user is invalid.");
    }

    private Guid? GetImpersonatedTenantId(ValidatedTokenRequest request, Guid? actorTenantId)
    {
      var impersonatedTenant = GetImpersonatedTenantParameter(request.Raw);
      if (impersonatedTenant == "host")
      {
        return null;
      }

      if (Guid.TryParse(impersonatedTenant, out var tenantId))
      {
        return tenantId;
      }

      return actorTenantId;
    }

    private string GetImpersonatedUserParameter(NameValueCollection parameters)
        => parameters.Get(VPortalExtensionGrantsConsts.Impresonation.ImpersonatedUserParameter)
            ?? throw new Exception("Impersonated user should be specified for impersonation grant.");

    private string GetImpersonatedTenantParameter(NameValueCollection parameters)
        => parameters.Get(VPortalExtensionGrantsConsts.Impresonation.ImpersonatedTenantParameter);

    private string GetAccessTokenParameter(NameValueCollection parameters)
        => parameters.Get(VPortalExtensionGrantsConsts.Impresonation.AccessTokenParameter)
            ?? throw InvalidTokenException();

    private static Guid? GetTenantId(IEnumerable<Claim> claims)
        => Guid.TryParse(claims.FirstOrDefault(x => x.Type == AbpClaimTypes.TenantId)?.Value, out var tenantClaim) ? tenantClaim : null;

    private static Guid GetUserId(IEnumerable<Claim> claims)
        => Guid.Parse(claims.First(x => x.Type == AbpClaimTypes.UserId).Value);

    private static Guid? GetImpersonatorUserId(IEnumerable<Claim> claims)
        => Guid.TryParse(claims.FirstOrDefault(x => x.Type == VPortalExtensionGrantsConsts.Impresonation.ImpersonatorUserClaim)?.Value, out var parsed) ? parsed : null;

    private static Guid? GetImpersonatorTenantId(IEnumerable<Claim> claims, Guid? actingAsTenantId)
    {
      var claim = claims.FirstOrDefault(x => x.Type == VPortalExtensionGrantsConsts.Impresonation.ImpersonatorTenantClaim)?.Value;
      if (claim == "host")
      {
        return null;
      }

      if (Guid.TryParse(claim, out var parsed))
      {
        return parsed;
      }

      return actingAsTenantId;
    }

    private static Exception InvalidTokenException() => new("A valid access token should be provided for impersonation grant.");
  }
}
