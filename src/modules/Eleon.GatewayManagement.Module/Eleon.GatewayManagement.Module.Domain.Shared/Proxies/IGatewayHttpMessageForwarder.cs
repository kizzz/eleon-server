using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayHttpMessageForwarder
  {
    Task<HttpResponseMessage> ForwardHttpRequestMessageToGateway(Guid gatewayId, HttpRequestMessage request);
  }
}
