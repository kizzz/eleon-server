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
  public class RemoveVpnPeerEventService : ITransientDependency, IDistributedEventHandler<RemoveVpnPeerMsg>
  {
    private readonly IVportalLogger<RemoveVpnPeerEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly VpnServerDomainService vpnServerDomainService;

    public RemoveVpnPeerEventService(
        IVportalLogger<RemoveVpnPeerEventService> logger,
        IResponseContext responseContext,
        VpnServerDomainService vpnServerDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.vpnServerDomainService = vpnServerDomainService;
    }

    public async Task HandleEventAsync(RemoveVpnPeerMsg eventData)
    {
      var response = new ActionCompletedMsg();
      try
      {
        await vpnServerDomainService.RemoveVpnPeer(VpnServerConsts.ServerNetworkName, eventData.PeerRefId);
        response.Success = true;
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
