using abp_sdk.Middlewares;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EleonsoftSdk.Overrides;

public class EleonsoftTenantAuthorizationHandler : IAuthorizationHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public EleonsoftTenantAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Task HandleAsync(AuthorizationHandlerContext context)
  {
    var resolveResult = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<EleonsoftTenantResolveResultAccessor>();

    if (resolveResult?.ResolveException != null)
    {
      context.Fail(new AuthorizationFailureReason(this, "Tenant was not resolved"));
      throw resolveResult.ResolveException;
    }

    return Task.CompletedTask;
  }
}
