using Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.Domain.Shared.DomainServices;
using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobHubContext : IBackgroundJobHubContext, ITransientDependency
  {
    private readonly IVportalLogger<BackgroundJobHubContext> logger;
    private readonly IBackgroundJobAppHubContext hubContext;
    private readonly IObjectMapper objectMapper;

    public BackgroundJobHubContext(IVportalLogger<BackgroundJobHubContext> logger, IBackgroundJobAppHubContext hubContext, IObjectMapper objectMapper)
    {
      this.logger = logger;
      this.hubContext = hubContext;
      this.objectMapper = objectMapper;
    }

    public async Task JobCompleted(BackgroundJobEntity job)
    {
      try
      {
        var jobEto = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job);
        await hubContext.JobCompleted(jobEto);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
