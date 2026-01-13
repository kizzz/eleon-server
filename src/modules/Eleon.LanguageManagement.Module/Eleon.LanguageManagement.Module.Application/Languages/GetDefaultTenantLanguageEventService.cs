using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class GetDefaultTenantLanguageEventService :
      IDistributedEventHandler<GetDefaultTenantLanguageMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetDefaultTenantLanguageEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly LanguageDomainService languageDomainService;

    public GetDefaultTenantLanguageEventService(
        IVportalLogger<GetDefaultTenantLanguageEventService> logger,
        IResponseContext responseContext,
        LanguageDomainService languageDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.languageDomainService = languageDomainService;
    }

    public async Task HandleEventAsync(GetDefaultTenantLanguageMsg eventData)
    {
      var response = new DefaultTenantLanguageGotMsg();
      try
      {
        var language = await languageDomainService.GetDefaultLanguage();
        response.CultureName = language.CultureName;
        response.UiCultureName = language.UiCultureName;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
