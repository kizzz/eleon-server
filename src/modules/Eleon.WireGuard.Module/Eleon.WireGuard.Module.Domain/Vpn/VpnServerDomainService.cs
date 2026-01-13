using Logging.Module;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace VPortal.Vpn
{

  public class VpnServerDomainService : DomainService
  {
    private readonly IVportalLogger<VpnServerDomainService> logger;
    private readonly IVpnSettingsRepository vpnSettingsRepository;

    public VpnServerDomainService(
        IVportalLogger<VpnServerDomainService> logger,
        IVpnSettingsRepository vpnSettingsRepository)
    {
      this.logger = logger;
      this.vpnSettingsRepository = vpnSettingsRepository;
    }

    public async Task<VpnPeerSettingsEntity> AddVpnPeer(string serverNetwork, string refId)
    {
      VpnPeerSettingsEntity result = null;
      try
      {
        var network = await vpnSettingsRepository.GetByNetworkName(serverNetwork);

        if (network.Peers.Any(x => x.RefId == refId))
        {
          throw new Exception("The peer already exists.");
        }

        var keys = WireGuardKeyGenerator.GenerateKeyPair();
        var peer = new VpnPeerSettingsEntity(GuidGenerator.Create())
        {
          RefId = refId,
          PublicKey = keys.publicKey,
          PrivateKey = keys.privateKey,
          AllowedIps = GetNextIp(network),
          PersistentKeepalive = 25,
        };

        network.Peers.Add(peer);
        await vpnSettingsRepository.UpdateAsync(network);

        result = peer;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task RemoveVpnPeer(string serverNetwork, string refId)
    {
      try
      {
        var network = await vpnSettingsRepository.GetByNetworkName(serverNetwork);
        if (network == null)
        {
          throw new Exception("Network is not configured.");
        }

        var peer = network.Peers.FirstOrDefault(x => x.RefId == refId);
        if (peer == null)
        {
          throw new Exception($"The peer '{refId}' does not exist.");
        }

        network.Peers.Remove(peer);

        await vpnSettingsRepository.UpdateAsync(network);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<VpnServerSettingsEntity> GetVpnNetwork(string serverNetwork)
    {
      VpnServerSettingsEntity result = null;
      try
      {
        result = await vpnSettingsRepository.GetByNetworkName(serverNetwork);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private static string GetNextIp(VpnServerSettingsEntity settings)
    {
      string nextIp;
      if (settings.Peers.Count == 0)
      {
        string[] ip = settings.Address.Split('/').First().Split('.').ToArray();
        nextIp = $"{ip[0]}.{ip[1]}.{ip[2]}.{int.Parse(ip[3]) + 1}";
      }
      else
      {
        var maxPeer = settings.Peers.MaxBy(x => int.Parse(x.AllowedIps.Split('/').First().Split('.').Last()));
        string[] ip = maxPeer.AllowedIps.Split('/').First().Split('.').ToArray();
        nextIp = $"{ip[0]}.{ip[1]}.{ip[2]}.{int.Parse(ip[3]) + 1}";
      }

      return $"{nextIp}";
    }
  }
}
