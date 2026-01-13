using Microsoft.AspNetCore.Http;
using EleonsoftProxy.Api;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using TenantSettings.Module.Cache;

namespace Eleoncore.Module.ContentSecurity
{
  public class EleoncoreContentSecurityMiddleware : IMiddleware, ITransientDependency
  {
    private const string ContentSecurityPolicyHeaderName = "Content-Security-Policy";
    private readonly ICurrentTenant currentTenant;
    private readonly EleoncoreSdkTenantSettingService _eleoncoreSdkTenantSettingService;

    public EleoncoreContentSecurityMiddleware(ICurrentTenant currentTenant, EleoncoreSdkTenantSettingService eleoncoreSdkTenantSettingService)
    {
      this.currentTenant = currentTenant;
      _eleoncoreSdkTenantSettingService = eleoncoreSdkTenantSettingService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      if (!currentTenant.IsAvailable)
      {
        await next(context);
        return;
      }
      Guid tenantId = currentTenant.Id == null ? Guid.Empty : currentTenant.Id.Value;
      var settings = await _eleoncoreSdkTenantSettingService.GetTenantSettingsAsync(tenantId);
      var allowedHosts = settings.TenantContentSecurityHosts;
      string formattedHosts = allowedHosts.Select(x => x.EnsureEndsWith('/')).JoinAsString(" ");
      if (formattedHosts.Trim().Length == 0)
      {
        formattedHosts = "none";
      }

      string policy = string.Join(" ", "frame-ancestors", formattedHosts) + ";";

      var response = context.Response;
      response.Headers[ContentSecurityPolicyHeaderName] = "frame-ancestors *;";

      await next(context);
    }
  }
}
