using Eleon.Logging.Lib.VportalLogging;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Threading;
using VPortal.MassTransit;

namespace BackgroundJobs.Module.Workers
{
  public class ScheduleBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
  {
    private readonly IVportalLogger<ScheduleBackgroundWorker> logger;
    private readonly IBoundaryLogger boundaryLogger;
    private readonly IConfiguration configuration;

    public ScheduleBackgroundWorker(
        IVportalLogger<ScheduleBackgroundWorker> logger,
        IBoundaryLogger boundaryLogger,
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IConfiguration configuration
    )
        : base(
            timer, serviceScopeFactory)
    {
      Timer.Period = configuration.GetValue("BackgroundJobSettings:Period", 30000);
      this.logger = logger;
      this.boundaryLogger = boundaryLogger;
      this.configuration = configuration;
    }

    protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
      using var _ = boundaryLogger.Begin("BackgroundWorker ScheduleBackgroundWorker");
      try
      {
        if (configuration.GetValue<bool>("BackgroundJobs") != false)
        {
          // here could be a problem
          // because the event bus could be not ready when the worker starts
          // and the getting required service would be infinite long time
          //  so we run it in a separate task and we wait max 10 seconds

          var sendingTask = Task.Run(async () =>
          {
            using var scope = ServiceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IDistributedEventBus>();
            await publisher.PublishAsync(new ScheduleMsg());
          });

          var resultTask = await Task.WhenAny(sendingTask, Task.Delay(10000));

          if (resultTask != sendingTask)
          {
            logger.Log.LogCritical(new TimeoutException("Sending ScheduleMsg timed out."), "Failed to send the schedule event");
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
    }
  }
}
