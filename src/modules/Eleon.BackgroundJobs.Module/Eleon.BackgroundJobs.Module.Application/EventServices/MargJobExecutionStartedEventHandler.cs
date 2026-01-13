using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.BackgroundJobs.Module.DomainServices;

namespace EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Application.EventServices;
public class MargJobExecutionStartedEventHandler : IDistributedEventHandler<MarkJobExecutionStartedMsg>, ITransientDependency
{
  private readonly IVportalLogger<MargJobExecutionStartedEventHandler> _logger;
  private readonly IBackgroundJobDomainService _backgroundJobDomainService;

  public MargJobExecutionStartedEventHandler(
      IVportalLogger<MargJobExecutionStartedEventHandler> logger,
      IBackgroundJobDomainService backgroundJobDomainService)
  {
    _logger = logger;
    _backgroundJobDomainService = backgroundJobDomainService;
  }

  public async Task HandleEventAsync(MarkJobExecutionStartedMsg eventData)
  {

    try
    {
      await _backgroundJobDomainService.MarkExecutionStartedAsync(eventData.JobId, eventData.ExecutionId);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
