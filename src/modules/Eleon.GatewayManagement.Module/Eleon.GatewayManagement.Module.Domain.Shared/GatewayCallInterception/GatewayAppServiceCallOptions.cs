using Common.Module.Constants;
using System;
using System.Security.Cryptography.X509Certificates;

namespace VPortal.GatewayManagement.Module.Domain.Shared.GatewayCallInterception
{
  public class GatewayAppServiceCallOptions
  {
    public Guid GatewayId { get; set; }
    public GatewayProtocol GatewayProtocol { get; set; }
    public int? GatewayPort { get; set; }
    public string GatewayIp { get; set; }
    public X509Certificate2 Certificate { get; set; }
  }
}
