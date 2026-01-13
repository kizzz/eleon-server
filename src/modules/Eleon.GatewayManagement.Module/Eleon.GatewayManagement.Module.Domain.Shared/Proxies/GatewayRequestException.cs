using System;
using Volo.Abp;
using VPortal.GatewayManagement.Module;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayRequestException : BusinessException
  {
    public GatewayRequestException(Guid gatewayId, Exception callException)
        : base(
              GatewayManagementErrorCodes.GatewayClientErrorOccured,
              innerException: new Exception($"An error occured while calling Gateway Client {gatewayId}. See inner exception for details.", innerException: callException))
    {
    }
  }
}
