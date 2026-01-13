using Common.Module.Constants;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayRegistrationKeyDto
  {
    public string Key { get; set; }
    public int ExpiresAfterMs { get; set; }
    public GatewayRegistrationKeyStatus Status { get; set; }
  }
}
