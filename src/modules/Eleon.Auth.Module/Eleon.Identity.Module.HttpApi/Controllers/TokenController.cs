using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.SignIn;
using static VPortal.Identity.Module.DomainServices.CustomTokenService;

namespace VPortal.Controllers
{
  [Area(VPortal.Identity.Module.IdentityRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = VPortal.Identity.Module.IdentityRemoteServiceConsts.RemoteServiceName)]
  [Route("api/test/token")]
  public class TokenController : Controller
  {
    private readonly CustomTokenService _tokenCreationService;
    private readonly bool _enableTestTokens;

    public TokenController(CustomTokenService tokenCreationService, IConfiguration configuration)
    {
      _tokenCreationService = tokenCreationService;
      _enableTestTokens = configuration.GetValue<bool>("DebugSettings:EnableTestToken", false);
    }

    [HttpPost("GetByUserId")]
    public async Task<TokenResonse> GetByUserIdAsync(Guid userId)
    {
      if (_enableTestTokens != true)
      {
        throw new Exception("Test tokens are disabled. Please enable them in the configuration.");
      }

      return await _tokenCreationService.CreateTokensByUserIdAsync("VPortal_App", userId.ToString());
    }

    [HttpPost("GetByUserName")]
    public async Task<TokenResonse> GetByUserNameAsync(string userName)
    {
      if (_enableTestTokens != true)
      {
        throw new Exception("Test tokens are disabled. Please enable them in the configuration.");
      }

      return await _tokenCreationService.CreateTokensByUserNameAsync("VPortal_App", userName);
    }
  }
}



public class CustomTokenGenerator : ITransientDependency
{
  private readonly ITokenService _tokenService;
  private readonly IRefreshTokenService _refreshTokenService;
  private readonly IClientStore _clientStore;
  private readonly IResourceStore _resourceStore;
  private readonly IdentityUserManager _userManager;
  private readonly SignInManager _signInManager;
  private readonly IConfiguration _configuration;

  public CustomTokenGenerator(
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


  public async Task<TokenResonse> GenerateTokensAsync(string clientId, string userId)
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
      RefreshToken = refreshToken
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
