namespace GatewayManagement.Module.Proxies
{
  public class RegisterGatewayRequestDto
  {
    public string RegistrationKey { get; set; }
    public string MachineKey { get; set; }
    public string CertificateBase64 { get; set; }
  }
}
