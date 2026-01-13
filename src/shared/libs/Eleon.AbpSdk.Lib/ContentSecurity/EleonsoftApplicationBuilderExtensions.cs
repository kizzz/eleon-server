using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Eleoncore.Module.ContentSecurity
{
  public static class EleonsoftApplicationBuilderExtensions
  {
    public static void UseEleonsoftContentSecurity(this IApplicationBuilder app)
    {
      app.UseMiddleware<EleonsoftContentSecurityMiddleware>();
    }


    public static IServiceCollection AddEleonsoftContentSecurity(this IServiceCollection services)
    {
      services.AddTransient<EleonsoftContentSecurityMiddleware>();

      return services;
    }
  }
}
