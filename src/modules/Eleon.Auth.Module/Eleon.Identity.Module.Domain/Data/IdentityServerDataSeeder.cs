using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.Uow;
using VPortal.Identity.Module.Data;

namespace VPortal.Core.Infrastructure.Module.IdentityServer;

[ExposeServices(typeof(IDataSeeder), typeof(IdentityServerDataSeeder))]
public class IdentityServerDataSeeder : IDataSeeder, ITransientDependency
{
  private class ClientIds
  {
    public const string Swagger = "VPortal_Swagger";
    public const string Sdk = "VPortal_SDK_Api";
    public const string Angular = "VPortal_App";
    public const string Driver = "VPortal_DriverClient";
    public const string Proxy = "VPortal_ProxyClient";
  }

  private readonly IdentityApiResourseManager resourseSeeder;
  private readonly IdentityApiScopeManager scopeSeeder;
  private readonly IdentityClientManager clientSeeder;
  private readonly IIdentityResourceDataSeeder identityResourceDataSeeder;
  private readonly IConfiguration configuration;

  public IdentityServerDataSeeder(
      IdentityApiResourseManager resourseSeeder,
      IdentityApiScopeManager scopeSeeder,
      IdentityClientManager clientSeeder,
      IIdentityResourceDataSeeder identityResourceDataSeeder,
      IConfiguration configuration)
  {
    this.resourseSeeder = resourseSeeder;
    this.scopeSeeder = scopeSeeder;
    this.clientSeeder = clientSeeder;
    this.identityResourceDataSeeder = identityResourceDataSeeder;
    this.configuration = configuration;
  }

  public virtual async Task SeedAsync(DataSeedContext context)
  {
    await identityResourceDataSeeder.CreateStandardResourcesAsync();
    await CreateApiResourcesAsync();
    await CreateApiScopesAsync();
    await CreateClientsAsync();
  }

  private async Task CreateApiScopesAsync()
  {
    await scopeSeeder.CreateApiScopeAsync("VPortal");
  }

  private async Task CreateApiResourcesAsync()
  {
    await resourseSeeder.CreateApiResourceAsync("VPortal", VportalIdentitySeedConsts.CommonApiUserClaims);
  }

  private async Task CreateClientsAsync()
  {
    var cfg = configuration.GetSection("IdentityServer:Clients");
    await CreateAngularClient(IdentitySeedClientDescription.Parse(cfg, ClientIds.Angular));
    await CreateSdkClient(IdentitySeedClientDescription.Parse(cfg, ClientIds.Sdk));
    await CreateSwaggerClient(IdentitySeedClientDescription.Parse(cfg, ClientIds.Swagger));
    await CreateProxyClient(IdentitySeedClientDescription.Parse(cfg, ClientIds.Proxy));
    await CreateDriverClient(IdentitySeedClientDescription.Parse(cfg, ClientIds.Driver));
  }

  private async Task CreateSwaggerClient(IdentitySeedClientDescription cfg)
  {
    var swaggerClientId = cfg.ClientId;
    var swaggerRootUrl = cfg.RootUrl.TrimEnd('/');

    var client = await clientSeeder.CreateClientAsync(
        name: swaggerClientId,
        scopes: VportalIdentitySeedConsts.CommonScopes,
        grantTypes: new[] { "authorization_code" },
        secret: cfg.ClientSecret.Sha256());
    if (client.RedirectUris.Count == 0)
    {
      client.AddRedirectUri("dummy_swagger");
    }
  }

  private async Task CreateAngularClient(IdentitySeedClientDescription cfg)
  {
    var consoleAndAngularClientId = cfg.ClientId;
    var client = await clientSeeder.CreateClientAsync(
        name: consoleAndAngularClientId,
        scopes: VportalIdentitySeedConsts.CommonScopes,
        grantTypes: new[] { "password", "client_credentials", "authorization_code", "LinkLogin", "Impersonation" },
        secret: cfg.ClientSecret.Sha256());
    if (client.RedirectUris.Count == 0)
    {
      client.AddRedirectUri("dummy_client");
    }
  }

  private async Task CreateProxyClient(IdentitySeedClientDescription cfg)
  {
    var client = await clientSeeder.CreateClientAsync(
        name: cfg.ClientId,
        scopes: new[] { "VPortal" },
        grantTypes: new[] { "x_machine_key" },
        secret: cfg.ClientSecret.Sha256());

    if (client.RedirectUris.Count == 0)
    {
      client.AddRedirectUri("dummy_proxy");
    }
  }
  private async Task CreateSdkClient(IdentitySeedClientDescription cfg)
  {
    var client = await clientSeeder.CreateClientAsync(
        name: cfg.ClientId,
        scopes: new[] { "VPortal" },
        grantTypes: new[] { "x_api_key" },
        secret: cfg.ClientSecret.Sha256());
    if (client.RedirectUris.Count == 0)
    {
      client.AddRedirectUri("dummy_proxy");
    }
  }

  private async Task CreateDriverClient(IdentitySeedClientDescription cfg)
  {
    var client = await clientSeeder.CreateClientAsync(
        name: cfg.ClientId,
        scopes: VportalIdentitySeedConsts.CommonScopes,
        grantTypes: new[] { "authorization_code" },
        secret: cfg.ClientSecret.Sha256());
    if (client.RedirectUris.Count == 0)
    {
      client.AddRedirectUri("dummy_proxy");
    }
  }
}
