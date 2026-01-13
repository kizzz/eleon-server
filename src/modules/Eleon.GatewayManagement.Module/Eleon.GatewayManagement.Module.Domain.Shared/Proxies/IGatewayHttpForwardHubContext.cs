using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayHttpForwardHubContext : ITransientDependency
  {
    Task ForwardHttpRequestToGateway(Guid gatewayId, Guid requestId);
  }
}
