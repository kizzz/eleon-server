using Common.Module.Constants;
using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Linq;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace VPortal.Identity.Module.ExternalProviders
{
  public class ExternalLoginOptionsConfigurator : IExternalLoginOptionsConfigurator, ITransientDependency
  {
    private readonly TenantSettingsCacheService cache;
    private readonly ICurrentTenant currentTenant;

    public ExternalLoginOptionsConfigurator(
        TenantSettingsCacheService cache,
        ICurrentTenant currentTenant)
    {
      this.cache = cache;
      this.currentTenant = currentTenant;
    }

    public void ConfigureOptions(string authenticationSchemeName, OpenIdConnectOptions options)
    {
      if (Enum.TryParse<ExternalLoginProviderType>(authenticationSchemeName, out var providerType))
      {
        var settings = cache.GetTenantSettings(currentTenant.Id).AsTask().GetAwaiter().GetResult();
        var loginSettings = settings.LoginProviders?.FirstOrDefault(x => x.Type == providerType);
        if (
            loginSettings == null
            || loginSettings.ClientId.IsNullOrWhiteSpace()
            || loginSettings.ClientSecret.IsNullOrWhiteSpace()
            || loginSettings.Authority.IsNullOrWhiteSpace())
        {
          throw new Exception($"Unable to retreive configuration for login via {providerType}. It may be disabled or not configured/");
        }

        options.ClientId = loginSettings.ClientId;
        options.ClientSecret = loginSettings.ClientSecret;
        options.Authority = loginSettings.Authority;
        //if (authenticationSchemeName == Enum.GetName(ExternalLoginProviderType.AzureEntra))
        //{
        //    options.ClientId = "052555d4-03a4-4e59-9faa-e14a9ef7d0fc";
        //    options.ClientSecret = "zSe8Q~W6W8y9yrHFVVpmqkiZyOXYP7vYCLgaQcwO";
        //    options.Authority = "https://login.microsoftonline.com/df9df8cd-ed0b-4bf8-ad9a-252a6bc3b563/";
        //}
      }
    }
  }
}
