using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayHttpForwardAppHubContext : ITransientDependency
  {
    Task<string> ForwardHttpRequestToGateway(Guid gatewayId, Guid requestId);
  }
}
