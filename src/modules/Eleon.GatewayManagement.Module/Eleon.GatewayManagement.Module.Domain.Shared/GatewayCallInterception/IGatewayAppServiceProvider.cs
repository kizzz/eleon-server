using System;
using System.Threading.Tasks;

namespace GatewayManagement.Module.GatewayCallInterception
{
  public interface IGatewayAppServiceProvider
  {
    Task<TAppService> ResolveScopedGatewayAppService<TAppService>(Guid gatewayId) where TAppService : class;
  }
}
