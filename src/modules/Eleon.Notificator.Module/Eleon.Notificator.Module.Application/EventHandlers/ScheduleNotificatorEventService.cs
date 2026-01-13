using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using VPortal.Notificator.Module.DomainServices;

namespace VPortal.Notificator.Module.EventServices
{

  public class ScheduleNotificatorEventService :
      IDistributedEventHandler<ScheduleMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ScheduleNotificatorEventService> logger;
    private readonly NotificatorDomainService notificatorDomainService;
    private readonly MultiTenancyDomainService multiTenancyDomainService;
    private readonly IConfiguration configuration;
    private readonly ICurrentTenant currentTenant;

    public ScheduleNotificatorEventService(
        IVportalLogger<ScheduleNotificatorEventService> logger,
        NotificatorDomainService notificatorDomainService,
        MultiTenancyDomainService multiTenancyDomainService,
        IConfiguration configuration,
        ICurrentTenant currentTenant
    )
    {
      this.logger = logger;
      this.notificatorDomainService = notificatorDomainService;
      this.multiTenancyDomainService = multiTenancyDomainService;
      this.configuration = configuration;
      this.currentTenant = currentTenant;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {
      try
      {
        if (configuration.GetValue<bool>("BackgroundJobs") != false)
        {
          await multiTenancyDomainService.ForEachTenant(async (tenantId) =>
          {
            using (currentTenant.Change(tenantId))
            {
              await notificatorDomainService.RequestBulkActiveNotificationsRunAsync();
            }
          });
        }
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
      }
    }
  }

}
