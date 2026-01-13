using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.BackgroundJobs.Module.DomainServices;

namespace EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Application.EventServices;
public class CancelBackgroundJobEventHandler : IDistributedEventHandler<CancelBackgroundJobMsg>, ITransientDependency
{
  private readonly IBackgroundJobDomainService _jobDomainService;
  private readonly IVportalLogger<CancelBackgroundJobEventHandler> _logger;

  public CancelBackgroundJobEventHandler(IBackgroundJobDomainService jobDomainService, IVportalLogger<CancelBackgroundJobEventHandler> logger)
  {
    _jobDomainService = jobDomainService;
    _logger = logger;
  }

  public async Task HandleEventAsync(CancelBackgroundJobMsg eventData)
  {
    try
    {
      await _jobDomainService.CancelJobAsync(eventData.JobId, eventData.CancelledBy, eventData.IsManually, eventData.CancelledMessage);
    }
    catch (Exception e)
    {
      _logger.CaptureAndSuppress(e);
    }
    finally
    {
    }
  }
}
