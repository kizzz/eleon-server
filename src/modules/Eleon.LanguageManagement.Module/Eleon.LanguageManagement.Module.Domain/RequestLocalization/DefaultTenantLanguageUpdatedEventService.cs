using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Authorization.Module.RequestLocalization
{
  public class DefaultTenantLanguageUpdatedEventService :
      IDistributedEventHandler<DefaultTenantLanguageUpdatedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<DefaultTenantLanguageUpdatedEventService> logger;
    private readonly RequestLanguageProvider requestLanguageProvider;

    public DefaultTenantLanguageUpdatedEventService(
        IVportalLogger<DefaultTenantLanguageUpdatedEventService> logger,
        RequestLanguageProvider requestLanguageProvider)
    {
      this.logger = logger;
      this.requestLanguageProvider = requestLanguageProvider;
    }

    public async Task HandleEventAsync(DefaultTenantLanguageUpdatedMsg eventData)
    {
      try
      {
        await requestLanguageProvider.SetTenantLanguage(eventData.CultureName, eventData.UiCultureName);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
