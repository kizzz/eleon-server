using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.PermissionManagement;

namespace VPortal.Identity.Module.Data
{
  public class IdentityClientManager : ITransientDependency
  {
    private readonly IClientRepository _clientRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPermissionDataSeeder _permissionDataSeeder;

    public IdentityClientManager(
        IClientRepository clientRepository,
        IGuidGenerator guidGenerator,
        IPermissionDataSeeder permissionDataSeeder)
    {
      _clientRepository = clientRepository;
      _guidGenerator = guidGenerator;
      _permissionDataSeeder = permissionDataSeeder;
    }

    public async Task<Client> CreateClientAsync(
        string name,
        IEnumerable<string> scopes,
        IEnumerable<string> grantTypes,
        string secret)
    {
      return await CreateClientAsync(
          name,
          scopes,
          grantTypes,
          secret,
          null);
    }

    public async Task<Client> CreateClientAsync(
    string name,
    IEnumerable<string> scopes,
    IEnumerable<string> grantTypes,
    string secret,
    IEnumerable<string> permissions = null)
    {
      var client = await _clientRepository.FindByClientIdAsync(name);
      if (client == null)
      {
        client = await _clientRepository.InsertAsync(
            new Client(
                _guidGenerator.Create(),
                name
            )
            {
              ClientName = name,
              ProtocolType = "oidc",
              Description = name,
              AlwaysIncludeUserClaimsInIdToken = true,
              AllowOfflineAccess = true,
              AbsoluteRefreshTokenLifetime = 365 * 24 * 60 * 60, // 365 d
              AccessTokenLifetime = 1 * 24 * 60 * 60, // 1 d
              AuthorizationCodeLifetime = 5 * 60, // 5 m
              IdentityTokenLifetime = 15 * 60, // 15 m
              RequireConsent = false,
              RequireClientSecret = false,
              RequirePkce = false,
            },
            autoSave: true
        );
      }

      foreach (var scope in scopes)
      {
        if (client.FindScope(scope) == null)
        {
          client.AddScope(scope);
        }
      }

      foreach (var grantType in grantTypes)
      {
        if (client.FindGrantType(grantType) == null)
        {
          client.AddGrantType(grantType);
        }
      }

      if (!secret.IsNullOrEmpty())
      {
        if (client.FindSecret(secret) == null)
        {
          client.AddSecret(secret);
        }
      }

      if (permissions != null)
      {
        await _permissionDataSeeder.SeedAsync(
            ClientPermissionValueProvider.ProviderName,
            name,
            permissions,
            null
        );
      }

      return await _clientRepository.UpdateAsync(client);
    }
  }
}
