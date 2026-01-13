using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace GatewayManagement.Module.Proxies
{
  internal class GatewayHttpForwardHubContext : IGatewayHttpForwardHubContext, ITransientDependency
  {
    private readonly IGatewayHttpForwardAppHubContext hub;
    private readonly IVportalLogger<GatewayHttpForwardHubContext> logger;
    private readonly IObjectMapper mapper;

    public GatewayHttpForwardHubContext(
        IGatewayHttpForwardAppHubContext hub,
        IVportalLogger<GatewayHttpForwardHubContext> logger,
        IObjectMapper mapper)
    {
      this.hub = hub;
      this.logger = logger;
      this.mapper = mapper;
    }

    public async Task ForwardHttpRequestToGateway(Guid gatewayId, Guid requestId)
    {
      try
      {
        await hub.ForwardHttpRequestToGateway(gatewayId, requestId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
