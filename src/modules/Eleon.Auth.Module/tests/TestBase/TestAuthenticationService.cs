using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

internal sealed class TestAuthenticationService : IAuthenticationService
{
    public AuthenticateResult AuthenticateResult { get; set; } = AuthenticateResult.NoResult();
    public string SignedInScheme { get; private set; }
    public string SignedOutScheme { get; private set; }
    public ClaimsPrincipal SignedInPrincipal { get; private set; }
    public AuthenticationProperties SignedInProperties { get; private set; }

    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
    {
        return Task.FromResult(AuthenticateResult);
    }

    public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        return Task.CompletedTask;
    }

    public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        return Task.CompletedTask;
    }

    public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        SignedInScheme = scheme;
        SignedInPrincipal = principal;
        SignedInProperties = properties;
        return Task.CompletedTask;
    }

    public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
    {
        SignedOutScheme = scheme;
        return Task.CompletedTask;
    }
}
