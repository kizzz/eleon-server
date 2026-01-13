using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using VPortal.Vpn;

namespace VPortal.Saas;

public class VpnDataSeedContributor : IDataSeedContributor, ITransientDependency
{
  private readonly IVpnSettingsRepository vpnSettingsRepository;
  private readonly IGuidGenerator guidGenerator;

  public VpnDataSeedContributor(
      IVpnSettingsRepository vpnSettingsRepository,
      IGuidGenerator guidGenerator)
  {
    this.vpnSettingsRepository = vpnSettingsRepository;
    this.guidGenerator = guidGenerator;
  }


  public virtual async Task SeedAsync(DataSeedContext context)
  {
    if (context == null || context.TenantId != null)
    {
      return;
    }

    var existing = await vpnSettingsRepository.GetByNetworkName(VpnServerConsts.ServerNetworkName);
    if (existing == null)
    {
      var keys = WireGuardKeyGenerator.GenerateKeyPair();
      var seed = new VpnServerSettingsEntity(guidGenerator.Create(), VpnServerConsts.ServerNetworkName)
      {
        Address = "10.0.0.1",
        PrivateKey = keys.privateKey,
        PublicKey = keys.publicKey,
        ListenPort = 57599,
        Dns = "8.8.8.8, 8.8.4.4",
      };

      await vpnSettingsRepository.InsertAsync(seed);
    }
  }
}
