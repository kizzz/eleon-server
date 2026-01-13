using System;
using Volo.Abp.Content;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayForwardedResponseDto
  {
    public Guid ResponseId { get; set; }
    public IRemoteStreamContent ResponseContent { get; set; }
  }
}
