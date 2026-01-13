using Common.Module.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.SettingManagement;

namespace VPortal.Notificator.Module.Emailing
{
  [ExposeServices(typeof(ISmtpEmailSenderConfiguration))]
  [Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
  public class SmtpEmailSenderConfiguration : ISmtpEmailSenderConfiguration
  {
    private readonly IConfiguration configuration;
    private readonly ISettingManager settingManager;

    private SmtpEmailSettings _defaultEmailSettings;
    private SmtpEmailSettings DefaultEmailSettings => _defaultEmailSettings ??= GetDefaultSettings();

    public SmtpEmailSenderConfiguration(
        IConfiguration configuration,
        ISettingManager settingManager)
    {
      this.configuration = configuration;
      this.settingManager = settingManager;
    }

    public Task<string> GetDefaultFromAddressAsync()
        => GetValueOrDefault("Abp.Mailing.DefaultFromAddress", s => s.Sender.Address);

    public Task<string> GetDefaultFromDisplayNameAsync()
        => GetValueOrDefault("Abp.Mailing.DefaultFromDisplayName", s => s.Sender.DisplayName);

    public Task<string> GetDomainAsync()
        => GetValueOrDefault("Abp.Mailing.Smtp.Domain", _ => null);

    public async Task<bool> GetEnableSslAsync()
        => bool.Parse(await GetValueOrDefault("Abp.Mailing.Smtp.EnableSsl", _ => "true"));

    public Task<string> GetHostAsync()
        => GetValueOrDefault("Abp.Mailing.Smtp.Host", s => s.Smtp.Server);

    public Task<string> GetPasswordAsync()
        => GetValueOrDefault("Abp.Mailing.Smtp.Password", s => s.Smtp.Credentials.Password);

    public async Task<int> GetPortAsync()
        => int.Parse(await GetValueOrDefault("Abp.Mailing.Smtp.Port", s => s.Smtp.Port.ToString()));

    public async Task<bool> GetUseDefaultCredentialsAsync()
        => bool.Parse(await GetValueOrDefault("Abp.Mailing.Smtp.UseDefaultCredentials", _ => "false"));

    public Task<string> GetUserNameAsync()
        => GetValueOrDefault("Abp.Mailing.Smtp.UserName", s => s.Smtp.Credentials.UserName);

    private async Task<string> GetValueOrDefault(string key, Func<SmtpEmailSettings, string> defaultFactory)
    {
      string value = await settingManager.GetOrNullForCurrentTenantAsync(key, fallback: false);
      if (value.NonEmpty())
      {
        return value;
      }

      if (DefaultEmailSettings.AllowFallbackForTenants)
      {
        return defaultFactory(DefaultEmailSettings);
      }

      return string.Empty;
    }

    private SmtpEmailSettings GetDefaultSettings()
    {
      var emailSection = configuration.GetSection("Email").Get<SmtpEmailSettings>();
      return emailSection;
    }
  }
}
