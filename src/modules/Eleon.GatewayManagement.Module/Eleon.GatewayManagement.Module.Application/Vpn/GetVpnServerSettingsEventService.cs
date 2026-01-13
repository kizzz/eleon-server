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

namespace VPortal.GatewayManagement.Module.Vpn
{
  public class GetVpnServerSettingsEventService : IDistributedEventHandler<GetVpnServerSettingsMsg>, ITransientDependency
  {
    private readonly IVportalLogger<GetVpnServerSettingsEventService> logger;
    private readonly IResponseContext responseContext;

    public GetVpnServerSettingsEventService(IVportalLogger<GetVpnServerSettingsEventService> logger, IResponseContext responseContext)
    {
      this.logger = logger;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(GetVpnServerSettingsMsg eventData)
    {
      var response = new VpnServerSettingsGotMsg();
      try
      {
        response.Settings = new Messaging.Module.ETO.VpnServerSettingsEto()
        {
          Address = "10.0.0.1/24",
          PrivateKey = "WMBk/YRSubk5iUkz3rh6+7srBxId/ooLxiiLslQ0+1g=",
          ListenPort = 57599,
          Dns = "8.8.8.8, 8.8.4.4",
          Peers =
                [
                new ()
                        {
                            PublicKey = "nnF70sCPdufMyUyadmFVrGyrTz72908/GRFkLZX9xgU=",
                            AllowedIps = "10.0.0.0/24",
                            Endpoint = "eleonsoft.network-protected.com:51825",
                            PersistentKeepalive = 25,
                        },
                        ],
        };
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
