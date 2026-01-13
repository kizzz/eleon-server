using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace ExternalLogin.Module
{
  public class OpenIdConnectStateResolver : ITransientDependency
  {
    private const string ProtectorName = "OpenIdStateResolver";
    private static readonly string Salt = GetSalt();
    private static readonly string StateParamName = "state";
    private static readonly string StateCookieName = ".oidc.state";
    private static readonly string UserStateParamPrefix = "OpenIdConnect.User";

    private readonly IDataProtector wrapperDataProtector;
    private readonly IDataProtector dataProtector;
    private readonly ILogger<OpenIdConnectStateResolver> logger;

    public OpenIdConnectStateResolver(IDataProtectionProvider dataProtectionProvider, ILogger<OpenIdConnectStateResolver> logger)
    {
      wrapperDataProtector = dataProtectionProvider.CreateProtector(OpenIdConnectConsts.GeneralProtectorName);
      dataProtector = dataProtectionProvider.CreateProtector(ProtectorName);
      this.logger = logger;
    }

    public void WriteOidcStateToOidcParameters(HttpContext httpContext, OpenIdConnectMessage oidcMessage, string authScheme, string privateKey)
    {
      logger.LogDebug($"Writing OIDC state to OIDC parameters. Auth Scheme: {authScheme}.");
      string encrypted = GetEncryptedCurrentTenantData(httpContext, authScheme, privateKey);
      oidcMessage.SetParameter(StateParamName, encrypted);
    }

    public void WriteOidcStateToCookie(HttpContext httpContext, string authScheme, string privateKey)
    {
      logger.LogDebug($"Writing OIDC state to cookies. Auth Scheme: {authScheme}.");
      string encrypted = GetEncryptedCurrentTenantData(httpContext, authScheme, privateKey);
      httpContext.Response.Cookies.Append(StateCookieName, encrypted);
    }

    public void ClearOidcStateCookie(HttpContext httpContext)
    {
      logger.LogDebug($"Clearing OIDC state cookies.");
      if (httpContext.Request.Cookies.TryGetValue(StateCookieName, out var state))
      {
        httpContext.Response.Cookies.Delete(StateCookieName);
      }
    }

    public bool IsStatePresent(HttpContext httpContext)
    {
      var state = GetStateFromRequest(httpContext);
      return state != null;
    }

    public void TrySetCurrentTenantFromOidcState(HttpContext httpContext)
    {
      var state = GetStateFromRequest(httpContext);
      if (state == null)
      {
        return;
      }

      logger.LogDebug($"Got tenant from OIDC state. Resolved tenant: {state.TenantId}. Resolved auth scheme: {state.AuthScheme}.");
      SetCurrentTenant(state.TenantId, httpContext);
    }

    private OpenIdConnectStateData GetStateFromRequest(HttpContext httpContext)
    {
      logger.LogDebug($"Trying to retreive OIDC state from Http Request.");

      string encryptedOidcProps = GetEncryptedOidcProps(httpContext);
      if (!encryptedOidcProps.IsNullOrEmpty())
      {
        return GetStateFromProps(httpContext, encryptedOidcProps);
      }

      string encryptedStateCookie = GetEncryptedOidcStateFromCookies(httpContext);
      if (!encryptedStateCookie.IsNullOrEmpty())
      {
        return DecryptState(httpContext, encryptedStateCookie);
      }

      return null;
    }

    private string GetEncryptedOidcStateFromCookies(HttpContext httpContext)
    {
      try
      {
        if (httpContext.Request.Cookies.TryGetValue(StateCookieName, out var state))
        {
          return state;
        }
      }
      catch (Exception) { }

      return null;
    }

    private string GetEncryptedOidcProps(HttpContext httpContext)
    {
      try
      {
        if (httpContext.Request.Form.TryGetValue(StateParamName, out var states))
        {
          return states.FirstOrDefault();
        }
      }
      catch (Exception) { }

      try
      {
        if (httpContext.Request.Query.TryGetValue(StateParamName, out var states))
        {
          return states.FirstOrDefault();
        }
      }
      catch (Exception) { }

      return null;
    }

    private string GetEncryptedCurrentTenantData(HttpContext httpContext, string authScheme, string privateKey)
    {
      var tenant = httpContext.RequestServices.GetRequiredService<ICurrentTenant>();
      var payload = new OpenIdConnectStateData(Salt, tenant.Id, authScheme, privateKey);
      string encrypted = dataProtector.Protect(JsonConvert.SerializeObject(payload));
      return encrypted;
    }

    private OpenIdConnectStateData GetStateFromProps(HttpContext httpContext, string encryptedProps)
    {
      logger.LogDebug($"Trying to retreive OIDC state from OIDC properties.");

      var format = new PropertiesDataFormat(wrapperDataProtector);
      var decryptedProps = format.Unprotect(encryptedProps);
      if (decryptedProps == null)
      {
        return null;
      }

      string encryptedState = decryptedProps.Items[UserStateParamPrefix + StateParamName];
      var state = DecryptState(httpContext, encryptedState);

      if (decryptedProps.Items.TryGetValue("scheme", out var schemeInProps) && schemeInProps != state.AuthScheme)
      {
        throw new Exception("OIDC message is corrupted");
      }

      logger.LogDebug($"Retreived OIDC state from OIDC properties.");
      return state;
    }

    private OpenIdConnectStateData DecryptState(HttpContext httpContext, string encryptedState)
    {
      logger.LogDebug($"Decrypting OIDC state.");

      var decryptedState = dataProtector.Unprotect(encryptedState);
      if (decryptedState.IsNullOrWhiteSpace())
      {
        return null;
      }

      var payload = JsonConvert.DeserializeObject<OpenIdConnectStateData>(decryptedState);

      string privateKey = GetPrivateKeyFromOptions(httpContext, payload.TenantId, payload.AuthScheme);
      payload.Validate(Salt, privateKey);

      logger.LogDebug($"Successfully decrypted OIDC state.");
      return payload;
    }

    private void SetCurrentTenant(Guid? tenantId, HttpContext httpContext)
    {
      var tenant = httpContext.RequestServices.GetRequiredService<ICurrentTenant>();
      tenant.Change(tenantId);
    }

    private string GetPrivateKeyFromOptions(HttpContext httpContext, Guid? tenantId, string authScheme)
    {
      var tenant = httpContext.RequestServices.GetRequiredService<ICurrentTenant>();
      using var scope = tenant.Change(tenantId);
      var options = new OpenIdConnectOptions();
      var configurator = httpContext.RequestServices.GetRequiredService<IExternalLoginOptionsConfigurator>();
      configurator.ConfigureOptions(authScheme, options);
      return options.ClientSecret;
    }

    private static string GetSalt()
    {
      byte[] buffer = new byte[100];
      RandomNumberGenerator.Fill(buffer);
      return BitConverter.ToString(buffer);
    }
  }
}
