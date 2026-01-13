using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayHttpForwardingAppService : IApplicationService
  {
    Task<IRemoteStreamContent> GetForwardedRequest(Guid requestId);
    Task<bool> SendForwardedResponse(GatewayForwardedResponseDto response);
  }
}
