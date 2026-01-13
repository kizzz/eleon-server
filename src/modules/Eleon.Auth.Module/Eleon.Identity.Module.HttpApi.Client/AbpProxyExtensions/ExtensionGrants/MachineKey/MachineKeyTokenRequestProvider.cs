using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.Keys;
using Duende.IdentityModel.Client;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityModel;

namespace VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey
{
  [ExposeServices(typeof(IExtensionGrantTokenRequestProvider))]
  public class MachineKeyTokenRequestProvider : IExtensionGrantTokenRequestProvider, ITransientDependency
  {
    private readonly IMachineSecretsProvider machineApiKeyProvider;

    public MachineKeyTokenRequestProvider(IMachineSecretsProvider machineApiKeyProvider)
    {
      this.machineApiKeyProvider = machineApiKeyProvider;
    }

    public string GrantType => VPortalExtensionGrantsConsts.Names.MachineKeyGrant;

    public async Task<TokenRequest> GetTokenRequest(IdentityClientConfiguration configuration)
    {
      var tokenRequest = new TokenRequest
      {
        ClientId = configuration.ClientId,
        ClientSecret = configuration.ClientSecret,
        GrantType = GrantType,
      };

      string machineKey = await machineApiKeyProvider.GetMachineKey();
      string clientKey = await machineApiKeyProvider.GetClientCompoundKey();
      var parsedClientKey = ClientCompoundKey.Parse(clientKey);
      var clientMachineCompoundKey = new ClientMachineCompoundKey(parsedClientKey.ClientKey, machineKey, parsedClientKey.TenantId);
      string encryptedCompoundKey = EncryptionHelper.Encrypt(clientMachineCompoundKey.ToString());
      tokenRequest.Parameters.AddRequired(VPortalExtensionGrantsConsts.MachineKey.MachineKeyParameter, encryptedCompoundKey);

      return tokenRequest;
    }
  }
}
