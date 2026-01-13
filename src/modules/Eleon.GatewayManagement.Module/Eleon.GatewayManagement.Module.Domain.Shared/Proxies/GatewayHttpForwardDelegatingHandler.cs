using Common.Module.Constants;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayHttpForwardDelegatingHandler : DelegatingHandler, ITransientDependency
  {
    private readonly CurrentGateway currentGateway;
    private readonly IGatewayHttpMessageForwarder gatewayHttpMessageForwarder;

    public GatewayHttpForwardDelegatingHandler(
        CurrentGateway currentGateway,
        IGatewayHttpMessageForwarder gatewayHttpMessageForwarder)
    {
      this.currentGateway = currentGateway;
      this.gatewayHttpMessageForwarder = gatewayHttpMessageForwarder;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
      bool isWssGateway = currentGateway.Options?.GatewayProtocol == GatewayProtocol.WSS;
      return isWssGateway
          ? await gatewayHttpMessageForwarder.ForwardHttpRequestMessageToGateway(currentGateway.Options.GatewayId, request)
          : await base.SendAsync(request, cancellationToken);
    }
  }
}
