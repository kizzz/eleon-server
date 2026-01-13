using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.TenantManagement.Module.TenantAppearance;

namespace TenantSettings.Module.Cache
{
  public class TenantSettingDto
  {
    public Guid? TenantId { get; set; }
    public bool TenantIsolationEnabled { get; set; }
    public string TenantCertificateHash { get; set; }
    public bool IpIsolationEnabled { get; set; }
    public TenantStatus Status { get; set; }
    public List<TenantHostnameDto> Hostnames { get; set; }
    public List<TenantExternalLoginProviderDto> ExternalProviders { get; set; }
    public List<TenantWhitelistedIpDto> WhitelistedIps { get; set; }
    public List<TenantContentSecurityHostDto> ContentSecurityHosts { get; set; }
    public TenantAppearanceSettingDto AppearanceSettings { get; set; }
  }
}
