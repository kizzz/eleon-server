using JobScheduler.Module.HubGroups;
using JobScheduler.Module.Tasks;
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
using VPortal.JobScheduler.Module.Tasks;

namespace VPortal.JobScheduler.Module.Tasks;

[ExposeServices(typeof(ITaskAppHubContext))]
public class TaskAppHubContext : ITaskAppHubContext, ITransientDependency
{
  private readonly IVportalLogger<TaskAppHubContext> _logger;
  private readonly ICurrentTenant _currentTenant;
  private readonly IHubContext<TaskHub, ITaskAppHubContext> _hubContext;

  public TaskAppHubContext(
      IVportalLogger<TaskAppHubContext> logger,
      ICurrentTenant currentTenant,
      IHubContext<TaskHub, ITaskAppHubContext> hubContext)
  {
    _logger = logger;
    _currentTenant = currentTenant;
    _hubContext = hubContext;
  }

  public async Task TaskCompleted(TaskHeaderDto job)
  {
    try
    {
      var group = _hubContext.Clients.Group(new TenantGroupId(_currentTenant).ToString());
      if (group != null)
      {
        await group.TaskCompleted(job);
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
