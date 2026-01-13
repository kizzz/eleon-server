namespace TenantSettings.Module.Models
{
  public class TenantWhitelistedIp
  {
    public Guid? TenantId { get; set; }
    public string IpAddress { get; set; }
    public bool Enabled { get; set; }
  }
}
