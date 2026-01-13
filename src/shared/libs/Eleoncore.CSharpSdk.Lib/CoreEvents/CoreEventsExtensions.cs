using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eleoncore.SDK.CoreEvents;

public static class CoreEventsExtensions
{
  private const int MinScheduleTimeInSeconds = 10;
  private static int GetValidScheduleTimeInSeconds(int value) => value >= MinScheduleTimeInSeconds ? value : MinScheduleTimeInSeconds;

  // Marker to prevent duplicate Quartz registration
  private class QuartzRegisteredMarker { }

  private static void RegisterServices(IServiceCollection services)
  {
    // Check if Quartz was already added by looking for the marker or IScheduler
    if (services.Any(sd => sd.ServiceType == typeof(QuartzRegisteredMarker)) ||
        services.Any(sd => sd.ServiceType == typeof(IScheduler)))
    {
      // If Quartz is already registered, just add our services
      services.TryAddTransient<QueueScheduler>();
      services.TryAddTransient<QueueScheduleJob>();
      return;
    }

    // Add Quartz services
    services.AddQuartz(q => { q.UseInMemoryStore(); });

    // Add Quartz hosted service only if not already registered
    if (!services.Any(sd => sd.ImplementationType?.Name == "QuartzHostedService"))
    {
      services.AddQuartzHostedService(options =>
      {
        options.WaitForJobsToComplete = true;
      });
    }

    services.TryAddTransient<QueueScheduler>();
    services.TryAddTransient<QueueScheduleJob>();

    services.TryAddSingleton(provider =>
    {
      var factory = provider.GetRequiredService<ISchedulerFactory>();
      return factory.GetScheduler().GetAwaiter().GetResult();
    });

    // Register marker
    services.TryAddSingleton<QuartzRegisteredMarker>();
  }

  public static IServiceCollection AddQueueScheduling(this IServiceCollection services, string queueName, int messagesLimit, string forwarding, int scheduleTime)
  {
    RegisterServices(services);

    services.Configure<ScheduleQueueOptions>(options =>
    {
      options.Queues.Add(new ScheduleQueueOptionsEntry
      {
        QueueName = queueName,
        MessagesLimit = messagesLimit,
        ScheduleTime = GetValidScheduleTimeInSeconds(scheduleTime),
        Forwarding = forwarding
      });
    });

    return services;
  }

  public static async Task<IApplicationBuilder> StartScheduleEventsAsync(this IApplicationBuilder app)
  {
    var options = app.ApplicationServices.GetService<IOptions<ScheduleQueueOptions>>()?.Value;

    if (options == null || options.Queues.Count == 0)
      return app;

    var scheduler = app.ApplicationServices.GetRequiredService<IScheduler>();

    foreach (var queue in options.Queues)
    {
      var jobKey = new JobKey($"CoreEventsScheduler_{queue.QueueName}", "CoreEvents");

      var job = JobBuilder.Create<QueueScheduleJob>()
          .WithIdentity(jobKey)
          .UsingJobData(nameof(ScheduleQueueOptionsEntry.QueueName), queue.QueueName)
          .UsingJobData(nameof(ScheduleQueueOptionsEntry.RecieveMessagesCount), queue.RecieveMessagesCount)
          .UsingJobData(nameof(ScheduleQueueOptionsEntry.MessagesLimit), queue.MessagesLimit)
          .UsingJobData(nameof(ScheduleQueueOptionsEntry.Forwarding), queue.Forwarding)
          .Build();

      // Create trigger with simple interval scheduling
      // Alternative approaches:
      // 1. Using extension method: .WithSchedule(queue.ScheduleTime.ToQuartzSchedule())
      // 2. Using TimeSpan: .WithSchedule(TimeSpan.FromSeconds(queue.ScheduleTime).ToQuartzSchedule())
      // 3. Using cron: .WithCronSchedule(TimeSpan.FromSeconds(queue.ScheduleTime).ToQuartzCronExpression())
      var trigger = TriggerBuilder.Create()
          .WithIdentity($"CoreEventsTrigger_{queue.QueueName}", "CoreEvents")
          .WithSimpleSchedule(x => x
              .WithIntervalInSeconds(GetValidScheduleTimeInSeconds(queue.ScheduleTime))
              .RepeatForever())
          .StartNow()
          .Build();

      await scheduler.ScheduleJob(job, trigger);
    }

    return app;
  }

  // Keep synchronous version for backward compatibility
  public static IApplicationBuilder StartScheduleEvents(this IApplicationBuilder app)
  {
    return StartScheduleEventsAsync(app).GetAwaiter().GetResult();
  }
}

