using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using TenantSettings.Module.Cache;
using VPortal.TenantManagement.Module.TenantAppearance;
using VPortal.TenantManagement.Module.TenantIsolation;

namespace VPortal.TenantManagement.Module.TenantSettingsCache
{
  public class TenantSettingsCacheValueDto
  {
    public TenantSettingsCacheValueDto()
    {
    }

    public TenantSettingsCacheValueDto(
        TenantSettingDto tenantSetting,
        List<UserIsolationSettingsDto> userIsolationSettings,
        List<Guid> adminIds)
    {
      TenantSetting = tenantSetting;
      UserIsolationSettings = userIsolationSettings;
      AdminIds = adminIds;

      IsActive = tenantSetting.Status == TenantStatus.Active;
      TenantUrls = tenantSetting.Hostnames.Select(x => x.Url).ToList();
      TenantAppearanceSetting = tenantSetting.AppearanceSettings;
      TenantHostnames = tenantSetting.Hostnames.Select(x => x.Hostname).ToList();
      TenantSecureHostnames = CreateHostnamesCache(tenantSetting, true);
      TenantNonSecureHostnames = CreateHostnamesCache(tenantSetting, false);
      TenantCertificate = tenantSetting.TenantIsolationEnabled ? tenantSetting.TenantCertificateHash : null;
      TenantWhitelistedIps = tenantSetting.IpIsolationEnabled
          ? tenantSetting.WhitelistedIps.Select(x => x.IpAddress).ToList()
          : new List<string>();
      LoginProviders = tenantSetting.ExternalProviders.Where(x => x.Enabled).ToList();
      TenantContentSecurityHosts = tenantSetting.ContentSecurityHosts.Select(x => x.Hostname).ToList();
      EnabledProviders = tenantSetting.ExternalProviders
          .Where(x => x.Enabled)
          .Select(x => x.Type)
          .ToList();
      CertificatesByUsersLookup = userIsolationSettings
          .Where(x => x.UserIsolationEnabled)
          .ToDictionary(x => x.UserId, x => x.UserCertificateHash);
      UsersByCertificatesLookup = userIsolationSettings
          .Where(x => x.UserIsolationEnabled && !string.IsNullOrEmpty(x.UserCertificateHash))
          .ToDictionary(x => x.UserCertificateHash, x => x.UserId);
      AdminUserIds = adminIds;
    }

    public TenantSettingDto TenantSetting { get; set; }
    public List<UserIsolationSettingsDto> UserIsolationSettings { get; set; }
    public List<Guid> AdminIds { get; set; }

    public bool IsActive { get; private set; }

    public IReadOnlyList<string> TenantUrls { get; private set; }

    public TenantAppearanceSettingDto TenantAppearanceSetting { get; private set; }

    public IReadOnlyList<string> TenantHostnames { get; private set; }

    public IReadOnlyList<string> TenantSecureHostnames { get; private set; }

    public IReadOnlyList<string> TenantNonSecureHostnames { get; private set; }

    public string TenantCertificate { get; private set; }

    public IReadOnlyList<string> TenantWhitelistedIps { get; private set; }

    public IReadOnlyList<TenantExternalLoginProviderDto> LoginProviders { get; private set; }

    public IReadOnlyList<string> TenantContentSecurityHosts { get; private set; }

    public IReadOnlyList<ExternalLoginProviderType> EnabledProviders { get; private set; }

    public Dictionary<Guid, string> CertificatesByUsersLookup { get; private set; }

    public Dictionary<string, Guid> UsersByCertificatesLookup { get; private set; }

    public IReadOnlyList<Guid> AdminUserIds { get; private set; }

    private static IReadOnlyList<string> CreateHostnamesCache(TenantSettingDto tenantSetting, bool secure)
    {
      return tenantSetting
          .Hostnames
          .Where(x => secure ? x.AcceptsClientCertificate : !x.AcceptsClientCertificate)
          .OrderByDescending(x => x.Default)
          .ThenByDescending(x => x.Internal)
          .Select(x => x.Hostname)
          .ToList();
    }
  }
}
