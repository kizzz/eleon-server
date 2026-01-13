using Common.Module.Constants;

namespace TenantSettings.Module.Models
{
  public class TenantSetting
  {
    public Guid? TenantId { get; set; }
    public bool TenantIsolationEnabled { get; set; }
    public string TenantCertificateHash { get; set; }
    public bool IpIsolationEnabled { get; set; }
    public TenantStatus Status { get; set; }
    public virtual List<TenantHostnameValueObject> Hostnames { get; set; } = new List<TenantHostnameValueObject>();
    public virtual List<TenantExternalLoginProvider> ExternalProviders { get; set; } = new List<TenantExternalLoginProvider>();
    public virtual List<TenantWhitelistedIp> WhitelistedIps { get; set; } = new List<TenantWhitelistedIp>();
    public virtual List<TenantContentSecurityHost> ContentSecurityHosts { get; set; } = new List<TenantContentSecurityHost>();
    public virtual TenantAppearanceSetting AppearanceSettings { get; set; } = new TenantAppearanceSetting();
  }
}
