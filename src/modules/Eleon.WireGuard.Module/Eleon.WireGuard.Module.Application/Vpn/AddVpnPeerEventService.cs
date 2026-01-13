using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.Vpn
{
  public class AddVpnPeerEventService : ITransientDependency, IDistributedEventHandler<AddVpnPeerMsg>
  {
    private readonly IVportalLogger<AddVpnPeerEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly VpnServerDomainService vpnServerDomainService;

    public AddVpnPeerEventService(
        IVportalLogger<AddVpnPeerEventService> logger,
        IResponseContext responseContext,
        VpnServerDomainService vpnServerDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.vpnServerDomainService = vpnServerDomainService;
    }

    public async Task HandleEventAsync(AddVpnPeerMsg eventData)
    {
      var response = new VpnPeerAddedMsg();
      try
      {
        var peer = await vpnServerDomainService.AddVpnPeer(
            VpnServerConsts.ServerNetworkName,
            eventData.RefId);
        var server = await vpnServerDomainService.GetVpnNetwork(VpnServerConsts.ServerNetworkName);

        response.PeerEto = new Messaging.Module.ETO.VpnServerPeerEto()
        {
          AllowedIps = peer.AllowedIps,
          Endpoint = peer.Endpoint,
          PersistentKeepalive = peer.PersistentKeepalive,
          PrivateKey = peer.PrivateKey,
          PublicKey = peer.PublicKey,
          RefId = peer.RefId,
        };

        response.ServerAddress = server.Address;
        response.ServerPublicKey = server.PublicKey;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
