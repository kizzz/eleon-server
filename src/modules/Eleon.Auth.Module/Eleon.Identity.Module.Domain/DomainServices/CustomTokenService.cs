using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using VPortal.Identity.Module.SignIn;

namespace VPortal.Identity.Module.DomainServices;

public class CustomTokenService : ITransientDependency
{
  private readonly ITokenService _tokenService;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IClientStore _clientStore;
  private readonly IResourceStore _resourceStore;
  private readonly IdentityUserManager _userManager;
  private readonly SignInManager _signInManager;
  private readonly IConfiguration _configuration;

  public CustomTokenService(
      ITokenService tokenService,
      IRefreshTokenService refreshTokenService,
      IClientStore clientStore,
      IResourceStore resourceStore,
      IdentityUserManager userManager,
      SignInManager signInManager,
      IConfiguration configuration)
  {
    _tokenService = tokenService;
    _refreshTokenService = refreshTokenService;
    _clientStore = clientStore;
    _resourceStore = resourceStore;
    _userManager = userManager;
    _signInManager = signInManager;
    _configuration = configuration;
  }

  public async Task<TokenResonse> CreateTokensByUserIdAsync(string clientId, string userId)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user == null)
    {
      throw new Exception("User was not found");
    }
    var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
    var client = await _clientStore.FindClientByIdAsync(clientId);
    if (client == null)
    {
      throw new Exception("Client was not found");
    }
    return await CreateTestTokenAsync(client, user);
  }

  public async Task<TokenResonse> CreateTokensByUserNameAsync(string clientId, string userName)
  {
    var user = await _userManager.FindByNameAsync(userName);
    if (user == null)
    {
      throw new Exception("User was not found");
    }
    var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
    var client = await _clientStore.FindClientByIdAsync(clientId);
    if (client == null)
    {
      throw new Exception("Client was not found");
    }
    return await CreateTestTokenAsync(client, user);
  }

  private async Task<TokenResonse> CreateTestTokenAsync(Client client, IdentityUser user)
  {
    ArgumentNullException.ThrowIfNull(client, nameof(client));
    ArgumentNullException.ThrowIfNull(user, nameof(user));

    var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(client.AllowedScopes);

    ((ClaimsIdentity)userPrincipal.Identity).AddClaims([new Claim("idp", "Local"), new Claim("auth_time", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())]);

    var validatedRequest = new ValidatedRequest();
    validatedRequest.SetClient(client);

    var tokenRequest = new TokenCreationRequest
    {
      Subject = userPrincipal,
      ValidatedResources = new ResourceValidationResult
      {
        Resources = resources,
        ParsedScopes = client.AllowedScopes.Select(x => new ParsedScopeValue(x)).ToList(),
      },
      ValidatedRequest = validatedRequest,
    };

    var accessToken = await _tokenService.CreateAccessTokenAsync(tokenRequest); // uses IClaimsService that uses IProfileService to fill user claims
    var jwt = await _tokenService.CreateSecurityTokenAsync(accessToken);

    var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(tokenRequest.Subject, accessToken, client); // stores refresh token into database

    return new TokenResonse
    {
      AccessToken = jwt,
      TokenType = "Bearer",
      ExpiresIn = client.AccessTokenLifetime,
      RefreshToken = refreshToken,
    };
  }

  public class TokenResonse
  {
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
  }
}
