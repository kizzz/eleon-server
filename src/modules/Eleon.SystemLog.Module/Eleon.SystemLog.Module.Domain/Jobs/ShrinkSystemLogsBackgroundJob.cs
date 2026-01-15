using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Logging.Module;
using Messaging.Module.ETO;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using VPortal.DocMessageLog.Module.Repositories;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Domain.Jobs;


public sealed class ShrinkSystemLogsJobArgs
{
  // Absolute cutoff: delete logs older than this UTC timestamp
  public DateTime? CutoffUtc { get; set; }

  // Relative cutoff: delete logs older than N minutes from "now" (UTC)
  public long? MaxAgeMinutes { get; set; }
}

public class ShrinkSystemLogsBackgroundJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IJsonSerializer _jsonSerializer;
  private readonly ISystemLogRepository _systemLogRepository;

  public ShrinkSystemLogsBackgroundJob(IJsonSerializer jsonSerializer, ISystemLogRepository systemLogRepository, Microsoft.Extensions.Logging.ILogger<DefaultBackgroundJob> logger, IDistributedEventBus eventBus) : base(logger, eventBus)
  {
    _jsonSerializer = jsonSerializer;
    _systemLogRepository = systemLogRepository;
  }

  protected override string Type => "ShrinkSystemLog";

  protected async override Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var args = _jsonSerializer.Deserialize<ShrinkSystemLogsJobArgs>(execution.StartExecutionParams);

    var olderThan = args.CutoffUtc ?? DateTime.UtcNow;

    if (args.MaxAgeMinutes.HasValue)
    {
      olderThan = olderThan.AddMinutes(-args.MaxAgeMinutes.Value);
    }

    var count = await _systemLogRepository.ShrinkAsync(olderThan);

    return new JobResult(true, $"Completed successfully. Removed {count} entries that are older than {olderThan}");
  }
}
