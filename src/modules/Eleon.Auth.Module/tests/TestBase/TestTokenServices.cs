using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

public sealed class TestTokenService : ITokenService
{
    public Token AccessToken { get; set; } = new Token("access_token");
    public string SecurityToken { get; set; } = "jwt-token";
    public TokenCreationRequest LastAccessTokenRequest { get; private set; }

    public Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
    {
        LastAccessTokenRequest = request;
        return Task.FromResult(AccessToken);
    }

    public Task<string> CreateSecurityTokenAsync(Token token)
    {
        return Task.FromResult(SecurityToken);
    }

    public Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
    {
        return Task.FromResult(new Token("identity_token"));
    }
}

public sealed class TestRefreshTokenService : IRefreshTokenService
{
    public string RefreshToken { get; set; } = "refresh";

    public Task<string> CreateRefreshTokenAsync(ClaimsPrincipal subject, Token accessToken, Client client)
    {
        return Task.FromResult(RefreshToken);
    }

    public Task<TokenValidationResult> ValidateRefreshTokenAsync(string refreshTokenHandle, Client client)
    {
        return Task.FromResult(new TokenValidationResult { IsError = false });
    }

    public Task<string> UpdateRefreshTokenAsync(string refreshTokenHandle, RefreshToken refreshToken, Client client)
    {
        return Task.FromResult(refreshTokenHandle);
    }
}
