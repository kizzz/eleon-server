using Authorization.Module.RequestLocalization;
using Common.EventBus.Module;
using EleonsoftSdk.Messages.Localization;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace ModuleCollector.LanguageManagementModule.LanguageManagement.Module.Domain.EventHandlers;
public class GetTenantLocalizationEventHandler : IDistributedEventHandler<GetTenantLocalizationSettingsRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<GetTenantLocalizationEventHandler> _logger;
  private readonly RequestLanguageProvider _requestLanguageProvider;
  private readonly IResponseContext _responseContext;
  private readonly ICurrentTenant _currentTenant;

  public GetTenantLocalizationEventHandler(
      IVportalLogger<GetTenantLocalizationEventHandler> logger,
      RequestLanguageProvider requestLanguageProvider,
      IResponseContext responseContext,
      ICurrentTenant currentTenant)
  {
    _logger = logger;
    _requestLanguageProvider = requestLanguageProvider;
    _responseContext = responseContext;
    _currentTenant = currentTenant;
  }
  public async Task HandleEventAsync(GetTenantLocalizationSettingsRequestMsg eventData)
  {

    try
    {
      using (_currentTenant.Change(eventData.TenantId))
      {
        var lang = await _requestLanguageProvider.GetTenantLanguage();

        var response = new GetTenantLocalizationSettingsResponseMsg
        {
          CultureName = lang.CultureName,
          UiCultureName = lang.UiCultureName,
        };
        await _responseContext.RespondAsync(response);
      }

    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
