using Common.Module.Constants;
using TenantSettings.Module.Models;

namespace TenantSettings.Module.Cache
{
  public class TenantSettingsCacheValue
  {
    public TenantSettingsCacheValue()
    {
    }

    public TenantSettingsCacheValue(
        TenantSetting tenantSetting,
        List<UserIsolationSettings> userIsolationSettings,
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

    public TenantSetting TenantSetting { get; set; } = new TenantSetting();
    public List<UserIsolationSettings> UserIsolationSettings { get; set; } = new List<UserIsolationSettings>();
    public List<Guid> AdminIds { get; set; } = new List<Guid>();

    public bool IsActive { get; private set; }

    public IReadOnlyList<string> TenantUrls { get; private set; } = new List<string>();

    public TenantAppearanceSetting TenantAppearanceSetting { get; private set; } = new TenantAppearanceSetting();

    public IReadOnlyList<string> TenantHostnames { get; private set; } = new List<string>();

    public IReadOnlyList<string> TenantSecureHostnames { get; private set; } = new List<string>();

    public IReadOnlyList<string> TenantNonSecureHostnames { get; private set; } = new List<string>();

    public string TenantCertificate { get; private set; }

    public IReadOnlyList<string> TenantWhitelistedIps { get; private set; } = new List<string>();

    public IReadOnlyList<TenantExternalLoginProvider> LoginProviders { get; private set; } = new List<TenantExternalLoginProvider>();

    public IReadOnlyList<string> TenantContentSecurityHosts { get; private set; } = new List<string>();

    public IReadOnlyList<ExternalLoginProviderType> EnabledProviders { get; private set; } = new List<ExternalLoginProviderType>();

    public IReadOnlyDictionary<Guid, string> CertificatesByUsersLookup { get; private set; } = new Dictionary<Guid, string>();

    public IReadOnlyDictionary<string, Guid> UsersByCertificatesLookup { get; private set; } = new Dictionary<string, Guid>();

    public IReadOnlyList<Guid> AdminUserIds { get; private set; } = new List<Guid>();

    private static IReadOnlyList<string> CreateHostnamesCache(TenantSetting tenantSetting, bool secure)
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
