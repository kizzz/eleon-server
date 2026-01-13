using Microsoft.AspNetCore.Builder;

namespace Authorization.Module.ContentSecurity
{
  public static class ApplicationBuilderExtensions
  {
    public static void UseContentSecurity(this IApplicationBuilder app)
    {
      app.UseMiddleware<ContentSecurityMiddleware>();
    }
  }
}
