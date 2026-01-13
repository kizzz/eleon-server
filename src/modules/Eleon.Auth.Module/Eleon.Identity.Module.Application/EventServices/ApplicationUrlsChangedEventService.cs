using ExternalLogin.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.Data;

namespace VPortal.Identity.Module.EventServices
{
  public class ApplicationUrlsChangedEventService :
      IDistributedEventHandler<ApplicationUrlsChangedMsg>,
      IDistributedEventHandler<IdentityUrlsChangedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ApplicationUrlsChangedEventService> logger;
    private readonly IdentityApplicationUrlsChangeObserver identityApplicationUrlsChangeObserver;

    public ApplicationUrlsChangedEventService(
        IVportalLogger<ApplicationUrlsChangedEventService> logger,
        IdentityApplicationUrlsChangeObserver identityApplicationUrlsChangeObserver)
    {
      this.logger = logger;
      this.identityApplicationUrlsChangeObserver = identityApplicationUrlsChangeObserver;
    }

    public async Task HandleEventAsync(ApplicationUrlsChangedMsg eventData)
    {
      try
      {
        var urls = eventData.Urls.ToDictionary(x => x.ApplicationType, x => x.ApplicationUrls);
        await identityApplicationUrlsChangeObserver.HandleApplicationUrlsChange(urls);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }

    public async Task HandleEventAsync(IdentityUrlsChangedMsg eventData)
    {
      try
      {
        var urls = eventData.AppUrls.ToDictionary(x => x.AppType, x => x.Urls);
        await identityApplicationUrlsChangeObserver.HandleApplicationUrlsChange(urls);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
