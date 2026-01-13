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
  public class GetVpnSettingsEventService : ITransientDependency, IDistributedEventHandler<GetVpnSettingsMsg>
  {
    private readonly IVportalLogger<GetVpnSettingsEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly VpnServerDomainService vpnServerDomainService;

    public GetVpnSettingsEventService(
        IVportalLogger<GetVpnSettingsEventService> logger,
        IResponseContext responseContext,
        VpnServerDomainService vpnServerDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.vpnServerDomainService = vpnServerDomainService;
    }

    public async Task HandleEventAsync(GetVpnSettingsMsg eventData)
    {
      var response = new VpnSettingsGotMsg();
      try
      {
        var settings = await vpnServerDomainService.GetVpnNetwork(VpnServerConsts.ServerNetworkName);
        response.Settings = new Messaging.Module.ETO.VpnServerSettingsEto()
        {
          Address = settings.Address,
          Dns = settings.Dns,
          ListenPort = settings.ListenPort,
          PrivateKey = settings.PrivateKey,
          Peers = settings.Peers.Select(x => new Messaging.Module.ETO.VpnServerPeerEto()
          {
            AllowedIps = x.AllowedIps,
            Endpoint = x.Endpoint,
            PersistentKeepalive = x.PersistentKeepalive,
            PublicKey = x.PublicKey,
          }).ToList(),
        };
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
