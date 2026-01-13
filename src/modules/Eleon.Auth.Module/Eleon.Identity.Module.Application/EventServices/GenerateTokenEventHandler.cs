using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.Messages.ApiKey;
using EleonsoftSdk.modules.Helpers.Module;
using ExternalLogin.Module;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Logging.Module;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Entities;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.EventServices;
public class GenerateTokenEventHandler : IDistributedEventHandler<GenerateTokenRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<GenerateTokenEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly IClientStore _clientStore;
  private readonly IResourceStore _resourceStore;
  private readonly ITokenService _tokenService;
  private readonly ApiKeyValidator _apiKeyValidator;
  private readonly IdentitySecurityLogManager _identitySecurityLogManager;
  private readonly IConfiguration _configuration;

  public GenerateTokenEventHandler(
      IVportalLogger<GenerateTokenEventHandler> logger,
      IResponseContext responseContext,
      IClientStore clientStore,
      IResourceStore resourceStore,
      ITokenService tokenService,
      ApiKeyValidator apiKeyValidator,
      IdentitySecurityLogManager identitySecurityLogManager,
      IConfiguration configuration)
  {
    _logger = logger;
    _responseContext = responseContext;
    _clientStore = clientStore;
    _resourceStore = resourceStore;
    _tokenService = tokenService;
    _apiKeyValidator = apiKeyValidator;
    _identitySecurityLogManager = identitySecurityLogManager;
    _configuration = configuration;
  }
  public async Task HandleEventAsync(GenerateTokenRequestMsg eventData)
  {

    var response = new GenerateTokenResponeMsg
    {
      IsSuccess = false,
      Error = "Failed to generate token.",
      ErrorDescription = "An error occurred while processing the request."
    };
    try
    {
      var client = await _clientStore.FindClientByIdAsync(eventData.ClientId);

      if (client == null || (client.RequireClientSecret && client.ClientSecrets.All(s => s.Value != eventData.ClientSecret.ToSha256())))
      {
        response.Error = "Invalid client credentials.";
        response.ErrorDescription = "The provided client ID or secret is invalid.";
        await _responseContext.RespondAsync(response);
        return;
      }

      var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(client.AllowedScopes);

      var key = await _apiKeyValidator.ValidateApiRequest(eventData.ApiKey, eventData.Nonce, eventData.Timestamp, eventData.Signature);

      if (key == null || !key.IsValid || key.ApiKey == null)
      {
        response.Error = "Invalid API key.";
        response.ErrorDescription = key.ErrorMessage ?? "The provided API key is invalid.";
        await _responseContext.RespondAsync(response);

        await SafeLogAsync(new IdentitySecurityLogContext
        {
          Identity = "Eleonauth:ApiKey",
          Action = "Failed to vaildate api key",
          ClientId = client.ClientId,
          UserName = key?.ApiKey?.Name,
        });

        return;
      }

      await SafeLogAsync(new IdentitySecurityLogContext
      {
        Identity = "Eleonauth:ApiKey",
        Action = "Api key successfully granted",
        ClientId = client.ClientId,
        UserName = key?.ApiKey?.Name,
      });

      var accessToken = new Token(IdentityModel.OidcConstants.TokenTypes.AccessToken)
      {
        CreationTime = DateTime.UtcNow,
        Lifetime = client.AccessTokenLifetime,
        Claims = new List<Claim>()
                {
                    new Claim("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyId, key.ApiKey.Id.ToString()),
                    new Claim("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim, key.ApiKey.Type.ToString()),
                    new Claim("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyRefIdClaim, key.ApiKey.RefId),
                    new Claim("client_" + VPortalExtensionGrantsConsts.ApiKey.ApiKeyNameClaim, key.ApiKey.Name),
                    new Claim("client_id", client.ClientId),
                    new Claim("scope", string.Join(" ", client.AllowedScopes)),
                    new Claim("iss", _configuration["App:Authority"]),
                },
        ClientId = client.ClientId,
        Audiences = client.AllowedScopes.ToList(),
      };

      var accessTokenResult = await _tokenService.CreateSecurityTokenAsync(accessToken);

      response.IsSuccess = true;
      response.AccessToken = accessTokenResult;
      response.ExpiresIn = client.AccessTokenLifetime;
      response.Error = string.Empty;
      response.ErrorDescription = string.Empty;

      await _responseContext.RespondAsync(response);

      await _identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = "ApiKey:" + key.ApiKey.Type.ToString(),
        Action = ExternalLoginSecurityLogActions.RequestedAccessTokenByApiKey,
        ClientId = client.ClientId,
        UserName = key.ApiKey.Name,
        ExtraProperties =
                {
                    ["ApiKeyName"] = key.ApiKey.Name,
                    ["ApiKeyId"] = key.ApiKey.Id,
                    ["ApiKeyType"] = key.ApiKey.Type.ToString(),
                    ["ApiKeyRefId"] = key.ApiKey.RefId.ToString(),
                    ["ApiKeyExpiresAt"] = key.ApiKey.ExpiresAt?.ToString("O") ?? "-",
                    ["ApiKeyInvalidated"] = key.ApiKey.Invalidated.ToString(),
                },
      });
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      throw;
    }
    finally
    {
    }
  }

  private async Task SafeLogAsync(IdentitySecurityLogContext context)
  {
    try
    {
      await _identitySecurityLogManager.SaveAsync(context);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }
}
