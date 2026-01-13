using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SharedModule.HttpApi.Helpers;
using System.Security.Principal;


namespace Eleoncore.SDK
{
  public class EleoncoreSdkTokenForwardingMiddleware : IMiddleware
  {
    private readonly IOptions<EleoncoreSdkTokenForwardingMiddlewareOptions> options;

    public EleoncoreSdkTokenForwardingMiddleware(IOptions<EleoncoreSdkTokenForwardingMiddlewareOptions> options)
    {
      this.options = options;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      // set token for current user into sdk token cache
      if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var esauthorization) && esauthorization.FirstOrDefault() != null)
      {
        var authParts = esauthorization.First()?.Split("Bearer");
        if (authParts?.Length == 2)
        {
          string token = authParts.Last();

          TokenHelperService.SetOidcToken(context.User.Identity?.FindUserId()?.ToString() ?? string.Empty, token);
        }
      }

      await next(context);
    }
  }
}
