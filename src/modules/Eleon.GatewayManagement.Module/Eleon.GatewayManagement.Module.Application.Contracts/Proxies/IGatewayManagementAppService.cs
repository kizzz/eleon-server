using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.GatewayManagement.Module.Proxies;

namespace GatewayManagement.Module.Proxies
{
  public interface IGatewayManagementAppService : IApplicationService
  {
    Task<GatewayDto> GetGateway(Guid gatewayId);
    Task<List<GatewayDto>> GetGatewayList(GatewayListRequestDto request);
    Task<string> AddGateway(GatewayDto gateway);
    Task<bool> RemoveGateway(Guid gatewayId);
    Task<GatewayRegistrationKeyDto> RequestGatewayRegistration(Guid gatewayId);
    Task<GatewayRegistrationKeyDto> GetCurrentGatewayRegistrationKey(Guid gatewayId);
    Task<bool> UpdateGateway(UpdateGatewayRequestDto gateway);
    Task<bool> CancelOngoingGatewayRegistration(Guid gatewayId);
    Task AcceptPendingGateway(AcceptPendingGatewayRequestDto request);
    Task RejectPendingGateway(Guid gatewayId);
  }
}
