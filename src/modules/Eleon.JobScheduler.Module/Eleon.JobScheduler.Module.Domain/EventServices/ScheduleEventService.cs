using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
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
    private readonly IUnitOfWorkManager unitOfWorkManager;

    public ScheduleEventService(
        IVportalLogger<ScheduleEventService> logger,
        TaskExecutionManager manager,
        IConfiguration configuration,
        IUnitOfWorkManager unitOfWorkManager)
    {
      this.logger = logger;
      this.manager = manager;
      this.configuration = configuration;
      this.unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {
      try
      {
        using var uow = unitOfWorkManager.Begin(true);
        if (configuration.GetValue<bool>("BackgroundJobs") != false)
        {
          await manager.RunDueTasksAsync();
        }
        await uow.SaveChangesAsync();
        await uow.CompleteAsync();
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
