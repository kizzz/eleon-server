using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using VPortal.Identity.Module.AuthenticationSchemes;

namespace ExternalLogin.Module
{
  public static class ExternalLoginServiceExtensions
  {
    public static void AddExternalLogin(this IServiceCollection services, IConfiguration configuration)
    {
      services.Replace(ServiceDescriptor.Singleton<IAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>());
      services.AddSingleton<IOptionsMonitor<OpenIdConnectOptions>, OpenIdConnectOptionsProvider>();
      services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsInitializer>();
      services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
      //services.ConfigureExternalCookie(cfg =>
      //{
      //    cfg.Cookie.Domain = "." + configuration["App:Domain"];
      //    cfg.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
      //});
    }
  }
}
