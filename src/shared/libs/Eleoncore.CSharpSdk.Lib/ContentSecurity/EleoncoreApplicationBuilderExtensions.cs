using Microsoft.AspNetCore.Builder;

namespace Eleoncore.Module.ContentSecurity
{
  public static class EleoncoreApplicationBuilderExtensions
  {
    public static void UseEleoncoreContentSecurity(this IApplicationBuilder app)
    {
      app.UseMiddleware<EleoncoreContentSecurityMiddleware>();
    }
  }
}
