using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using SharedCollector.deprecated.Messaging.Module.SystemMessages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;

namespace VPortal.Notificator.Module.Notificators.Implementations
{
  public class SmsNotificator : ITransientDependency
  {
    private readonly IVportalLogger<SmsNotificator> logger;
    private readonly IDistributedEventBus _eventBus;
    private readonly IdentityUserManager _userManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly IConfiguration _configuration;
    private readonly NotificatorHelperService _helperService;
    private readonly ISettingManager _settingManager;

    public SmsNotificator(
        IVportalLogger<SmsNotificator> logger,
        IDistributedEventBus eventBus,
        IdentityUserManager userManager,
        ICurrentTenant currentTenant,
        IConfiguration configuration,
        NotificatorHelperService recipientResolver,
        ISettingManager settingManager)
    {
      this.logger = logger;
      _userManager = userManager;
      _currentTenant = currentTenant;
      _configuration = configuration;
      _helperService = recipientResolver;
      _settingManager = settingManager;
      _eventBus = eventBus;
    }

    public async Task<bool> SendSmsAsync(List<string> phoneNumbers, string message, SmsOptions smsOptions = null)
    {
      smsOptions ??= await GetSmsOptionsAsync();

      var result = true;
      switch (smsOptions.Provider)
      {
        case "CUSTOM":
          result = await NotifyCustomAsync(phoneNumbers, message);
          break;
        case "DEFAULT":
          result = true;
          break;
        default:
          break;
      }

      return result;
    }

    private async Task<SmsOptions> GetSmsOptionsAsync()
    {
      var provider = await _settingManager.GetOrNullForCurrentTenantAsync("SmsProviderOption") ?? "DEFAULT";

      return new SmsOptions
      {
        Provider = provider,
      };
    }

    private async Task<bool> NotifyCustomAsync(
        List<string> phoneNumbers,
        string message)
    {
      try
      {
        foreach (var phone in phoneNumbers)
        {
          await _eventBus.PublishAsync(new CustomSmsMsg
          {
            Sender = _helperService.GetTenantName(),
            PhoneNumber = phone,
            Message = message,
            SendTimeUTC = System.DateTime.UtcNow
          });
        }
        return true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }


      return false;
    }


  }
}
