using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace ExternalLogin.Module
{
  public class OpenIdConnectOptionsInitializer : IConfigureNamedOptions<OpenIdConnectOptions>
  {
    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly IExternalLoginOptionsConfigurator optionsConfigurator;
    private readonly ICurrentTenant currentTenant;

    public OpenIdConnectOptionsInitializer(
        IDataProtectionProvider dataProtectionProvider,
        IExternalLoginOptionsConfigurator optionsConfigurator,
        ICurrentTenant currentTenant)
    {
      this.dataProtectionProvider = dataProtectionProvider;
      this.optionsConfigurator = optionsConfigurator;
      this.currentTenant = currentTenant;
    }

    public void Configure(string name, OpenIdConnectOptions options)
    {
      SetDefaults(options);

      MapClaims(options);

      optionsConfigurator.ConfigureOptions(name, options);

      if (options.ClientSecret.IsNullOrEmpty() || options.ClientId.IsNullOrEmpty() || options.Authority.IsNullOrEmpty())
      {
        throw new Exception("OIDC options configurator did not set some requred fields.");
      }

      ListenOidcEvents(options.Events, name, options.ClientSecret);
    }

    public void Configure(OpenIdConnectOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");

    private void SetDefaults(OpenIdConnectOptions options)
    {
      var protector = dataProtectionProvider.CreateProtector(OpenIdConnectConsts.GeneralProtectorName);
      options.DataProtectionProvider = protector;
      options.StateDataFormat = new PropertiesDataFormat(protector);
      options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
      options.CorrelationCookie.Expiration = TimeSpan.FromDays(20);
      options.ResponseMode = OpenIdConnectResponseMode.FormPost;
      options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
      options.Scope.Add("openid");
      options.Scope.Add("email");
      options.GetClaimsFromUserInfoEndpoint = true;
      options.SaveTokens = true;
      options.SignInScheme = IdentityConstants.ExternalScheme;
      options.CallbackPath = "/signin-oidc";
      options.RemoteSignOutPath = "/signout-oidc";
      options.SignedOutRedirectUri = "/Acoount/Logout";
      options.Events ??= new OpenIdConnectEvents();
    }

    private void MapClaims(OpenIdConnectOptions options)
    {
      options.ClaimActions.MapAbpClaimTypes();
      options.ClaimActions.MapUniqueJsonKey(ClaimTypes.NameIdentifier, "sub");
    }

    private void ListenOidcEvents(OpenIdConnectEvents events, string authSchemeName, string privateKey)
    {
      events.OnRedirectToIdentityProvider = async redirectContext =>
      {
        WriteOidcStateToParameters(redirectContext, authSchemeName, privateKey);
        await WriteSecurityLog(redirectContext.HttpContext, authSchemeName, ExternalLoginSecurityLogActions.RedirectToExternalProviderForLogin);
      };

      events.OnRedirectToIdentityProviderForSignOut = async redirectContext =>
      {
        WriteOidcStateToParameters(redirectContext, authSchemeName, privateKey);
        await WriteSecurityLog(redirectContext.HttpContext, authSchemeName, ExternalLoginSecurityLogActions.RedirectToExternalProviderForLogout);
      };

      events.OnSignedOutCallbackRedirect = async signedOutContext =>
      {
        var resolver = signedOutContext.HttpContext.RequestServices.GetRequiredService<OpenIdConnectStateResolver>();
        resolver.WriteOidcStateToCookie(signedOutContext.HttpContext, authSchemeName, privateKey);
        await WriteSecurityLog(signedOutContext.HttpContext, authSchemeName, ExternalLoginSecurityLogActions.LogoutFromExternalProvider);
      };

      events.OnRemoteFailure = async remoteFailureContext =>
      {
        if (remoteFailureContext.Failure is OpenIdConnectProtocolException &&
                  remoteFailureContext.Failure.Message.Contains("access_denied"))
        {
          remoteFailureContext.HandleResponse();
          remoteFailureContext.Response.Redirect($"{remoteFailureContext.Request.PathBase}/");
        }

        await WriteSecurityLog(remoteFailureContext.HttpContext, authSchemeName, ExternalLoginSecurityLogActions.ExternalLoginFailed);
      };
    }

    private async Task WriteSecurityLog(HttpContext context, string authSchemeName, string action)
    {
      var logger = context.RequestServices.GetRequiredService<IExternalLoginSecurityLogManager>();
      await logger.WriteSecurityLog(action, authSchemeName);
    }

    private void WriteOidcStateToParameters(RedirectContext redirectContext, string authSchemeName, string privateKey)
    {
      var resolver = redirectContext.HttpContext.RequestServices.GetRequiredService<OpenIdConnectStateResolver>();
      resolver.WriteOidcStateToOidcParameters(redirectContext.HttpContext, redirectContext.ProtocolMessage, authSchemeName, privateKey);
    }
  }
}
