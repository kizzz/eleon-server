using Common.Module.Helpers;
using Common.Module.Keys;
using IdentityModel;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey;

namespace VPortal.Identity.Module.MachineKeyValidation
{
  public class MachineKeyHeaderGenerator : ITransientDependency
  {
    private readonly IMachineSecretsProvider machineApiKeyProvider;

    public MachineKeyHeaderGenerator(
        IMachineSecretsProvider machineApiKeyProvider
        )
    {
      this.machineApiKeyProvider = machineApiKeyProvider;
    }

    public async Task<string> GenerateMachineKeyHeader(string token)
    {
      var now = DateTime.UtcNow.ToEpochTime();
      string machineKey = await machineApiKeyProvider.GetMachineKey();
      var compoundKey = new RawCompoundKey(machineKey, token, now.ToString());
      return EncryptionHelper.Encrypt(compoundKey.ToString());
    }
  }
}
