using Common.Module.Constants;
using Common.Module.Extensions;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
using EleonsoftSdk.modules.Azure;
using Logging.Module;
using MailKit.Security;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MimeKit;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating;
using VPortal.Notificator.Module.Emailing;
using VPortal.Notificator.Module.Localization;
using VPortal.Notificator.Module.Settings;

namespace VPortal.Notificator.Module.Notificators.Implementations;

public static class EmailServerType
{
  public const string Smtp = "smtp";
  public const string AzureEws = "azureews";

  public const string Default = Smtp;
}

public class EmailNotificator : ITransientDependency
{
  private readonly IVportalLogger<EmailNotificator> logger;
  private readonly ISettingManager settingManager;
  private readonly IConfiguration configuration;
  private readonly IServiceProvider _serviceProvider;

  public EmailNotificator(
      IVportalLogger<EmailNotificator> logger,
      ISettingManager settingManager,
      IConfiguration configuration,
      IServiceProvider serviceProvider
      )
  {
    this.logger = logger;
    this.settingManager = settingManager;
    this.configuration = configuration;
    _serviceProvider = serviceProvider;
  }

  public async Task SendEmailAsync(string subject, string body, List<string> targetEmails, bool isHtml = true, Dictionary<string, string> attachments = null)
  {
    var settings = await settingManager.GetOrDefaultForCurrentTenantAsync<GeneralNotificatorOptions>(NotificatorModuleSettings.GeneralSettings);


    switch (settings.ServerType?.ToLower())
    {
      case EmailServerType.AzureEws:
        await (_serviceProvider.GetRequiredService<AzureNotificator>()).SendEmailAsync(await GetAzureSettingsAsync(), subject, body, targetEmails, isHtml, attachments);
        break;
      default: // EmailServerType.Smtp
        await (_serviceProvider.GetRequiredService<SmtpNotificator>()).SendEmailAsync(await GetSmtpSettingsAsync(), subject, body, targetEmails, null, isHtml, attachments);
        break;
    }
  }

  private async Task<AzureEwsOptions> GetAzureSettingsAsync()
  {
    var settings = await settingManager.GetOrDefaultForCurrentTenantAsync<AzureEwsOptions>(NotificatorModuleSettings.AzureEwsSettings);

    settings.IgnoreServerCertificateErrors = configuration.GetValue("AzureEws:IgnoreServerCertificateErrors", true);

    return settings;
  }

  private async Task<SmtpEmailSettings> GetSmtpSettingsAsync()
  {

    try
    {
      var emailSettings = new SmtpEmailSettings
      {
        Sender = new SmtpEmailSettings.SenderSettings { },
        Smtp = new SmtpEmailSettings.SmtpSettings { Credentials = new SmtpEmailSettings.SmtpCredentials { } }
      };

      var emailAddress = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromAddress");
      if (!string.IsNullOrEmpty(emailAddress))
      {
        emailSettings.Sender.Address = emailAddress;
      }

      var fromDisplayName = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromDisplayName");
      if (!string.IsNullOrEmpty(fromDisplayName))
      {
        emailSettings.Sender.DisplayName = fromDisplayName;
      }

      var server = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Host");
      if (!string.IsNullOrEmpty(server))
      {
        emailSettings.Smtp.Server = server;
      }

      var port = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Port");
      if (!string.IsNullOrEmpty(port))
      {
        emailSettings.Smtp.Port = Convert.ToInt32(port);
      }

      string username = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.UserName");
      if (!string.IsNullOrEmpty(username))
      {
        emailSettings.Smtp.Credentials.UserName = username;
      }

      string password = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Password");
      if (!string.IsNullOrEmpty(password))
      {
        emailSettings.Smtp.Credentials.Password = password;
      }

      var enableSsl = await settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.EnableSsl");
      if (!string.IsNullOrEmpty(enableSsl))
      {
        emailSettings.Smtp.UseSsl = Convert.ToBoolean(enableSsl);
      }

      return emailSettings;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
