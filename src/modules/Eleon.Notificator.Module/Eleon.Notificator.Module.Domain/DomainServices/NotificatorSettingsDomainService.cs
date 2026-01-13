using Common.EventBus.Module;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
using EleonsoftSdk.modules.Azure;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using Scriban;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using Volo.Abp.SettingManagement;
using VPortal.Notificator.Module.Emailing;
using VPortal.Notificator.Module.Settings;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;

public class NotificatorSettings
{
  public GeneralNotificatorOptions GeneralSettings { get; set; }
  public SmtpOptions SmtpSettings { get; set; }
  public AzureEwsOptions AzureEws { get; set; }
  public TelegramOptions Telegram { get; set; }
}

public class NotificatorSettingsDomainService : DomainService
{
  private readonly ISettingManager _settingManager;
  private readonly IVportalLogger<NotificatorSettingsDomainService> logger;
  private readonly IDistributedEventBus requestClient;
  private readonly IConfiguration _configuration;
  private readonly IJsonSerializer _jsonSerializer;

  public NotificatorSettingsDomainService(
      ISettingManager settingManager,
      IVportalLogger<NotificatorSettingsDomainService> logger,
      IDistributedEventBus requestClient,
      IConfiguration configuration,
      IJsonSerializer jsonSerializer)
  {
    this._settingManager = settingManager;
    this.logger = logger;
    this.requestClient = requestClient;
    _configuration = configuration;
    _jsonSerializer = jsonSerializer;
  }

  public async Task<List<string>> ValidateTemplateAsync(string templateType, string template)
  {
    if (templateType == NotificatorConstants.TemplateTypes.Scriban)
    {
      var parsed = Template.Parse(template);
      if (parsed.HasErrors)
      {
        var errors = string.Join(", ", parsed.Messages.Select(x => x.Message));

        return new List<string> { errors };
      }
    }
    else if (templateType == NotificatorConstants.TemplateTypes.PlainText)
    {
      if (!template.Contains("{message}"))
      {
        return new List<string>()
                {
                    "The template must contain the {message} placeholder."
                };
      }
    }

    return [];
  }

  public async Task<NotificatorSettings> GetAsync()
  {
    try
    {
      var emailSettings = await GetEmailSettingsAsync();
      var azureEwsSettings = await _settingManager.GetOrDefaultForCurrentTenantAsync<AzureEwsOptions>(NotificatorModuleSettings.AzureEwsSettings);
      var telegramSettings = await _settingManager.GetOrDefaultForCurrentTenantAsync<TelegramOptions>(NotificatorModuleSettings.TelegramSettings);
      var generalSettings = await _settingManager.GetOrDefaultForCurrentTenantAsync<GeneralNotificatorOptions>(NotificatorModuleSettings.GeneralSettings);
      return new NotificatorSettings
      {
        GeneralSettings = generalSettings,
        SmtpSettings = emailSettings,
        AzureEws = azureEwsSettings,
        Telegram = telegramSettings
      };
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

  private async Task<SmtpOptions> GetEmailSettingsAsync()
  {

    try
    {
      var defaultEmailSettings = _configuration.GetSection("DebugSettings:DevSmtp").Get<SmtpEmailSettings>();

      string defaultFromAddress =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromAddress")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromAddress") :
          defaultEmailSettings.Sender.Address;

      string defaultFromDisplayName =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromDisplayName")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.DefaultFromDisplayName") :
          defaultEmailSettings.Sender.DisplayName;

      string host =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Host")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Host") :
          defaultEmailSettings.Smtp.Server;

      int port = !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Port")) &&
          Convert.ToInt32(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Port")) > 0 ?
          Convert.ToInt32(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Port")) :
          defaultEmailSettings.Smtp.Port;

      string username =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.UserName")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.UserName") :
          defaultEmailSettings.Smtp.Credentials.UserName;

      string password = !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Password")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Password") :
          defaultEmailSettings.Smtp.Credentials.Password;

      string domain = !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Domain")) ?
          await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.Domain") :
          null;

      bool enableSsl =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.EnableSsl")) ?
          Convert.ToBoolean(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.EnableSsl")) :
          true;

      bool useDefaultCredentials =
          !string.IsNullOrEmpty(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.UseDefaultCredentials")) ?
          Convert.ToBoolean(await _settingManager.GetOrNullForCurrentTenantAsync("Abp.Mailing.Smtp.UseDefaultCredentials")) :
          false;

      return new SmtpOptions(
              defaultFromAddress,
              defaultFromDisplayName,
              host,
              port,
              username,
              password,
              domain,
              enableSsl,
              useDefaultCredentials);
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

  public async Task UpdateAsync(NotificatorSettings settings)
  {

    try
    {
      await UpdateGeneralAsync(settings.GeneralSettings);
      await UpdateSmtpOptionsAsync(settings.SmtpSettings);
      await UpdateAzureEwsOptionsAsync(settings.AzureEws);
      await UpdateTelegramOptionsAsync(settings.Telegram);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  private async Task UpdateGeneralAsync(GeneralNotificatorOptions options)
  {
    await _settingManager.SetForCurrentTenantAsync<GeneralNotificatorOptions>(NotificatorModuleSettings.GeneralSettings, options);
  }

  private async Task UpdateSmtpOptionsAsync(SmtpOptions settings)
  {

    try
    {
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.DefaultFromAddress", settings.DefaultFromAddress, true); // EmailSettingNames.DefaultFromAddress
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.DefaultFromDisplayName", settings.DefaultFromDisplayName, true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.Host", settings.Host, true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.Port", settings.Port.ToString(), true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.UserName", settings.Username, true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.Password", settings.Password, true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.Domain", settings.Domain, true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.EnableSsl", settings.EnableSsl.ToString(), true);
      await _settingManager.SetForCurrentTenantAsync("Abp.Mailing.Smtp.UseDefaultCredentials", settings.UseDefaultCredentials.ToString(), true);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  private async Task UpdateAzureEwsOptionsAsync(AzureEwsOptions settings)
  {
    try
    {
      AzureMailService.CloseConnection();
      await _settingManager.SetForCurrentTenantAsync<AzureEwsOptions>(NotificatorModuleSettings.AzureEwsSettings, settings);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  private async Task UpdateTelegramOptionsAsync(TelegramOptions settings)
  {
    try
    {
      await _settingManager.SetForCurrentTenantAsync<TelegramOptions>(NotificatorModuleSettings.TelegramSettings, settings);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<string> SendCustomTestEmailAsync(string subject, string sender, string target, string body)
  {
    var result = string.Empty;
    try
    {
      var request = new SendEmailMsg();
      request.Subject = subject;
      request.SenderEmailAddress = sender;
      request.TargetEmailAddress = target;
      request.Body = body;

      var response = await requestClient.RequestAsync<SendEmailGotMsg>(request, 300);
      if (!string.IsNullOrEmpty(response.ErrorMsg))
      {
        result = response.ErrorMsg;
      }
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    finally
    {
    }

    return result;
  }
}
