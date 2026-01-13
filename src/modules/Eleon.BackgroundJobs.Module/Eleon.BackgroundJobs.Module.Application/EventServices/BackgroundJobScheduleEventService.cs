using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.BackgroundJobs.Module.DomainServices;

namespace BackgroundJobs.Module.EventServices
{

  public class BackgroundJobScheduleEventService :
      IDistributedEventHandler<ScheduleMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<BackgroundJobScheduleEventService> logger;
    private readonly BackgroundJobManagerDomainService backgroundJobManagerDomainService;
    private readonly IConfiguration configuration;

    public BackgroundJobScheduleEventService(
        IVportalLogger<BackgroundJobScheduleEventService> logger,
        BackgroundJobManagerDomainService backgroundJobManagerDomainService,
        IConfiguration configuration
    )
    {
      this.logger = logger;
      this.backgroundJobManagerDomainService = backgroundJobManagerDomainService;
      this.configuration = configuration;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {

      try
      {
        if (configuration.GetValue<bool>("BackgroundJobs") != false)
        {
          await backgroundJobManagerDomainService.RunJobsByScheduledTimeAsync();
          await backgroundJobManagerDomainService.CancelLongTimeJobsAsync();
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

    }
  }

}
