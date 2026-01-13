using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.GatewayManagement.Module.Proxies;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayClientAppService : IApplicationService
  {
    Task<GatewayRegistrationResultDto> RegisterGateway(RegisterGatewayRequestDto requestDto);
    Task<bool> ConfirmGatewayRegistration();
    Task<GatewayDto> GetCurrentGateway();
    Task<GatewayWorkspaceDto> GetCurrentGatewayWorkspace(string workspaceName);
    Task SetGatewayHealthStatus(SetGatewayHealthStatusRequestDto request);
  }
}
