using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ExternalLogin.Module
{
  public interface IExternalLoginOptionsConfigurator
  {
    void ConfigureOptions(string authenticationSchemeName, OpenIdConnectOptions options);
  }
}
