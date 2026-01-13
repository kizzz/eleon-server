using Common.Module.Extensions;
using Logging.Module;
using Microsoft.AspNetCore.SignalR;
using GatewayManagement.Module.Proxies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayManagement.Module.HttpForwarding
{
  public class GatewayHttpForwardAppHubContext : IGatewayHttpForwardAppHubContext
  {
    private readonly IVportalLogger<GatewayHttpForwardAppHubContext> logger;
    private readonly IHubContext<GatewayHttpForwardHub> hubContext;

    public GatewayHttpForwardAppHubContext(
        IVportalLogger<GatewayHttpForwardAppHubContext> logger,
        IHubContext<GatewayHttpForwardHub> hubContext)
    {
      this.logger = logger;
      this.hubContext = hubContext;
    }

    public async Task<string> ForwardHttpRequestToGateway(Guid gatewayId, Guid requestId)
    {
      string result = null;
      try
      {
        string connectionId = GatewayHttpForwardHub.GetConnectionId(gatewayId);

        if (connectionId.NonEmpty())
        {
          var connection = hubContext.Clients.Client(connectionId);
          var cts = new CancellationTokenSource();
          await connection.SendAsync(HttpForwardingRemoteConsts.ForwardHttpRequestMethodName, requestId.ToString(), cts.Token);
        }
        else
        {
          throw new Exception($"Unable to forward an HTTP request: the gateway client {gatewayId} is not connected to the host server.");
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
