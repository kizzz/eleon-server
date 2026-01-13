using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using VPortal.DocMessageLog.Module.Domain;
using VPortal.DocMessageLog.Module.Entities;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Domain.Jobs;

public class SendSystemLogJobParams
{
  public SystemLogLevel SystemLogLevel { get; set; }
  public string Message { get; set; }
}

public class SendSystemLogJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IDistributedEventBus _eventBus;
  private readonly SystemLogDomainService _systemLogDomainService;
  private readonly IJsonSerializer _jsonSerializer;

  public SendSystemLogJob(
      ILogger<DefaultBackgroundJob> logger,
      SystemLogDomainService systemLogDomainService,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    _eventBus = eventBus;
    _systemLogDomainService = systemLogDomainService;
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => "SendSystemLog";

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var jobParams = _jsonSerializer.Deserialize<SendSystemLogJobParams>(execution.StartExecutionParams);

    var systemLog = new SystemLogEntity();
    systemLog.LogLevel = jobParams.SystemLogLevel;
    systemLog.Message = jobParams.Message;
    systemLog.ApplicationName = "SendSystemLogJob";
    await _systemLogDomainService.WriteAsync(systemLog);

    var result = new JobResult(true, "System log successfully sent", new List<BackgroundJobTextInfoEto>());

    return result;
  }
}
