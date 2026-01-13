using Common.Module.Constants;

namespace TenantSettings.Module.Models
{
  public class TenantHostnameValueObject
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Domain { get; set; }
    public string Subdomain { get; set; }
    public int Port { get; set; }
    public bool IsSsl { get; set; }
    public VportalApplicationType ApplicationType { get; set; }
    public bool Internal { get; set; }
    public bool AcceptsClientCertificate { get; set; }
    public bool Default { get; set; }
    public string Url { get; set; }
    public string HostnameWithPort { get; set; }
    public string Hostname { get; set; }
    public Guid? AppId { get; set; }
  }
}
