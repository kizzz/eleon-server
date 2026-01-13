using Authorization.Module.TenantHostname;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Authorization.Module.ClientIsolation
{
  public class ClientIsolationMiddleware : IMiddleware, ITransientDependency
  {
    private readonly ILogger<ClientIsolationMiddleware> logger;
    private readonly TenantUrlResolver tenantUrlResolver;
    private readonly IConfiguration configuration;

    public ClientIsolationMiddleware(
        ILogger<ClientIsolationMiddleware> logger,
        TenantUrlResolver tenantUrlResolver,
        IConfiguration configuration)
    {
      this.logger = logger;
      this.tenantUrlResolver = tenantUrlResolver;
      this.configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var validator = context.RequestServices.GetRequiredService<ClientIsolationValidator>();
      var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
      var currentTenant = context.RequestServices.GetRequiredService<ICurrentTenant>();

      var validationResult = await validator.ValidateClientIsolation(context, currentUser.Id);

      bool isSecureRequest = await tenantUrlResolver.IsSecureHost(context, currentTenant.Id);
      if (validationResult.ValidationResult
          is ClientIsolationValidationResult.CertIsolationDisabled
          && isSecureRequest)
      {
        logger.LogDebug($"Redirecting user {validationResult.Ip} to non-secure API.");
        await SendRedirectToNonSecure(context, currentTenant.Id);
      }
      else if (validationResult.Valid)
      {
        await next(context);
      }
      else if (validationResult.ValidationResult == ClientIsolationValidationResult.InvalidIp)
      {
        logger.LogDebug($"Rejecting user {validationResult.Ip} because of IP isolation validation failure.");
        SendIpError(context);
      }
      else if (isSecureRequest)
      {
        logger.LogDebug($"Redirect user {validationResult.Ip} to the error page as he failed to provide cert via secure API.");
        await SendRedirectToCertificateError(context, currentTenant.Id);
      }
      else
      {
        logger.LogDebug($"Redirect user {validationResult.Ip} to secure API.");
        await SendRedirectToSecure(context, currentTenant.Id);
      }
    }

    private void SendIpError(HttpContext context)
    {
      context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }

    private async Task SendRedirectToCertificateError(HttpContext context, Guid? tenantId)
    {
      var currentUrl = await tenantUrlResolver.GetBaseTenantUrl(context, tenantId);
      string errorUrl = $"{currentUrl}/srv/core/certificate-error";
      SetCustomRedirect(context, errorUrl);
    }

    private async Task SendRedirectToSecure(HttpContext context, Guid? tenantId)
    {
      var secureBaseUrl = await tenantUrlResolver.GetBaseTenantUrl(context, tenantId, security: true);
      SetCustomRedirect(context, secureBaseUrl);
    }

    private async Task SendRedirectToNonSecure(HttpContext context, Guid? tenantId)
    {
      var nonSecureBaseUrl = await tenantUrlResolver.GetBaseTenantUrl(context, tenantId, security: false);
      SetCustomRedirect(context, nonSecureBaseUrl);
    }

    private void SetCustomRedirect(HttpContext context, string targetUrl)
    {
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      context.Response.Headers["Location"] = targetUrl;
    }
  }
}
