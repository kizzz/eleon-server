using Common.Module.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace GatewayManagement.Module.HttpForwarding
{
  [HubRoute(HttpForwardingRemoteConsts.HttpForwardingHubRoute)]
  public class GatewayHttpForwardHub : AbpHub
  {
    private static readonly ConcurrentDictionary<string, string> _connections = new();

    public override Task OnConnectedAsync()
    {
      _connections[GetGatewayId()] = Context.ConnectionId;

      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      _connections.TryRemove(GetGatewayId(), out _);

      return base.OnDisconnectedAsync(exception);
    }

    private string GetGatewayId()
        => Context.User.Claims.First(x => x.Type == OptionalUserClaims.ProxyId).Value;

    public static string GetConnectionId(Guid gatewayId) => _connections.GetValueOrDefault(gatewayId.ToString(), null);
  }
}
