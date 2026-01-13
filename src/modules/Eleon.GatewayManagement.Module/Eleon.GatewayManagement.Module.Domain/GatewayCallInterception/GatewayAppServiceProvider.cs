using GatewayManagement.Module.Proxies;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.GatewayManagement.Module.Domain.Shared.GatewayCallInterception;

namespace GatewayManagement.Module.GatewayCallInterception
{
  public class GatewayAppServiceProvider : IGatewayAppServiceProvider, ITransientDependency
  {
    private readonly GatewayAppServiceResolver gatewayAppServiceResolver;
    private readonly GatewayManagementDomainService gatewayManagementDomainService;

    public GatewayAppServiceProvider(
        GatewayAppServiceResolver gatewayAppServiceResolver,
        GatewayManagementDomainService gatewayManagementDomainService)
    {
      this.gatewayAppServiceResolver = gatewayAppServiceResolver;
      this.gatewayManagementDomainService = gatewayManagementDomainService;
    }

    public async Task<TAppService> ResolveScopedGatewayAppService<TAppService>(Guid gatewayId)
        where TAppService : class
    {
      var gateway = await gatewayManagementDomainService.GetGateway(gatewayId);
      var cert = await gatewayManagementDomainService.GetGatewayCertificate(gatewayId);
      var options = new GatewayAppServiceCallOptions()
      {
        GatewayId = gateway.Id,
        GatewayIp = gateway.IpAddress,
        GatewayPort = gateway.Port,
        GatewayProtocol = gateway.Protocol,
        Certificate = cert,
      };

      return gatewayAppServiceResolver.ResolveScopedGatewayAppService<TAppService>(options);
    }
  }
}
