using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.JobScheduler.Module.DomainServices;

namespace JobScheduler.Module.EventServices
{
  public class ScheduleEventService :
      IDistributedEventHandler<ScheduleMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ScheduleEventService> logger;
    private readonly TaskExecutionManager manager;
    private readonly IConfiguration configuration;

    public ScheduleEventService(
        IVportalLogger<ScheduleEventService> logger,
        TaskExecutionManager manager,
        IConfiguration configuration)
    {
      this.logger = logger;
      this.manager = manager;
      this.configuration = configuration;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {
      try
      {
        if (configuration.GetValue<bool>("BackgroundJobs") != false)
        {
          await manager.RunDueTasksAsync();
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }
    }
  }
}
