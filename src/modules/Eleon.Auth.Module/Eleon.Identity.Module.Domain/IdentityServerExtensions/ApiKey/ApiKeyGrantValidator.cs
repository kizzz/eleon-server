using Common.Module.Constants;
using Common.Module.Extensions;
using Common.Module.Helpers;
using Common.Module.Keys;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Threading;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.IdentityServerExtensions.ApiKey
{
  public class ApiKeyGrantValidator : IExtensionGrantValidator
  {
    private readonly ICurrentTenant currentTenant;
    private readonly IVportalLogger<ApiKeyGrantValidator> logger;
    private readonly ApiKeyValidator _apiKeyValidator;
    private readonly IConfiguration _configuration;
    private readonly bool _isEnabled;
    private readonly IdentitySecurityLogManager _identitySecurityLogManager;

    public ApiKeyGrantValidator(
        ICurrentTenant currentTenant,
        IVportalLogger<ApiKeyGrantValidator> logger,
        ApiKeyValidator apiKeyValidator,
        IConfiguration configuration,
        IdentitySecurityLogManager identitySecurityLogManager)
    {
      this.currentTenant = currentTenant;
      this.logger = logger;
      _apiKeyValidator = apiKeyValidator;
      _configuration = configuration;
      _isEnabled = configuration.GetValue("ApiKeyGrant:Enabled", false);
      _identitySecurityLogManager = identitySecurityLogManager;
    }

    public string GrantType => VPortalExtensionGrantsConsts.Names.ApiKeyGrant;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
      try
      {
        if (!_isEnabled)
        {
          throw new Exception("Api key grant is not enabled in the configuration.");
        }

        var apiKey = context.Request.Raw.Get(VPortalExtensionGrantsConsts.ApiKey.ApiKeyParameter) ?? throw new Exception("Api key parameter is required for this grant.");
        var nonce = context.Request.Raw.Get(VPortalExtensionGrantsConsts.ApiKey.NonceParameter) ?? throw new Exception("Nonce parameter is required for this grant.");
        var signature = context.Request.Raw.Get(VPortalExtensionGrantsConsts.ApiKey.SignatureParameter) ?? throw new Exception("Signature parameter is required for this grant.");

        var successfullyParsed = DateTimeOffset.TryParse(context.Request.Raw.Get(VPortalExtensionGrantsConsts.ApiKey.TimestampParameter), out var timestamp);
        if (!successfullyParsed)
        {
          throw new Exception("Timestamp parameter is required for this grant.");
        }

        var result = await _apiKeyValidator.ValidateApiRequest(apiKey, nonce, timestamp.UtcDateTime, signature);

        if (result == null || !result.IsValid || result.ApiKey == null)
        {
          try
          {
            await _identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
              Identity = "Eleonauth:ApiKey",
              Action = "Failed to vaildate api key",
              ClientId = context.Request.Client.ClientId,
              UserName = result?.ApiKey?.Name,
              ExtraProperties =
                            {
                                { "ApiKeyId", result?.ApiKey?.Id.ToString() },
                                { "ApiKeyType", result?.ApiKey?.Type.ToString() },
                                { "ApiKeyRefId", result?.ApiKey?.RefId.ToString() },
                            },
            });
          }
          catch (Exception ex)
          {
            logger.Log.LogError(ex, "Error occurred while saving API key grant log.");
          }
        }
        else
        {
          try
          {
            await _identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
              Identity = "Eleonauth:ApiKey",
              Action = "Api key successfully granted",
              ClientId = context.Request.Client.ClientId,
              UserName = result?.ApiKey?.Name,
              ExtraProperties =
                            {
                                { "ApiKeyId", result?.ApiKey?.Id.ToString() },
                                { "ApiKeyType", result?.ApiKey?.Type.ToString() },
                                { "ApiKeyRefId", result?.ApiKey?.RefId.ToString() },
                            },
            });
          }
          catch (Exception ex)
          {
            logger.Log.LogError(ex, "Error occurred while saving API key grant log.");
          }
        }

        if (result == null || !result.IsValid)
        {
          throw new Exception("Api key is not valid. " + (result?.ErrorMessage ?? ""));
        }

        using (currentTenant.Change(result.ApiKey.TenantId))
        {
          var claims = new List<Claim>()
                    {
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyId, result.ApiKey.Id.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim, result.ApiKey.Type.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyRefIdClaim, result.ApiKey.RefId.ToString()),
                        new Claim(VPortalExtensionGrantsConsts.ApiKey.ApiKeyNameClaim, result.ApiKey.Name ?? string.Empty),
                    };

          if (result.ApiKey.TenantId.HasValue)
          {
            claims.Add(new Claim(AbpClaimTypes.TenantId, result.ApiKey.TenantId.Value.ToString()));
          }

          foreach (var claim in claims)
          {
            context.Request.ClientClaims.Add(claim);
          }

          context.Result = new GrantValidationResult();
        }
      }
      catch (Exception ex)
      {
        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
        logger.CaptureAndSuppress(ex);
        return;
      }

    }
  }
}
