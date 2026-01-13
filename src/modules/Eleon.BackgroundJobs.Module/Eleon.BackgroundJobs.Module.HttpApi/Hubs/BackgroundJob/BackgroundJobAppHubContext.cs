using BackgroundJobs.Module.BackgroundJobs;
using BackgroundJobs.Module.HubGroups;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.HttpApi.Hubs.BackgroundJob;

[ExposeServices(typeof(IBackgroundJobAppHubContext))]
public class BackgroundJobAppHubContext : IBackgroundJobAppHubContext, ITransientDependency
{
  private readonly IVportalLogger<BackgroundJobAppHubContext> _logger;
  private readonly ICurrentTenant _currentTenant;
  private readonly IHubContext<BackgroundJobHub, IBackgroundJobAppHubContext> _hubContext;

  public BackgroundJobAppHubContext(
      IVportalLogger<BackgroundJobAppHubContext> logger,
      ICurrentTenant currentTenant,
      IHubContext<BackgroundJobHub, IBackgroundJobAppHubContext> hubContext)
  {
    _logger = logger;
    _currentTenant = currentTenant;
    _hubContext = hubContext;
  }

  public async Task JobCompleted(BackgroundJobEto job)
  {
    try
    {
      var group = _hubContext.Clients.Group(new TenantGroupId(_currentTenant).ToString());
      if (group != null)
      {
        await group.JobCompleted(job);
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }
}
