using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating HTTP context and accessors for testing.
/// </summary>
public static class HttpContextTestHelpers
{
    /// <summary>
    /// Creates a mock IHttpContextAccessor with a default HTTP context.
    /// </summary>
    /// <param name="apiKeyId">Optional API key ID to add as a claim.</param>
    /// <returns>A mock IHttpContextAccessor with configured HTTP context.</returns>
    public static IHttpContextAccessor CreateHttpContextAccessor(string apiKeyId = null)
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity();
        
        if (!string.IsNullOrWhiteSpace(apiKeyId))
        {
            identity.AddClaim(new Claim("client_key_id", apiKeyId));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, apiKeyId));
        }
        
        context.User = new ClaimsPrincipal(identity);
        accessor.HttpContext.Returns(context);
        
        return accessor;
    }
}

