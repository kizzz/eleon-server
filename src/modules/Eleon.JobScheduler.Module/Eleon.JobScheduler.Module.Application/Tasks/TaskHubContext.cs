using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;
using JobScheduler.Module.Tasks;
using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Tasks;

namespace VPortal.JobScheduler.Module.Tasks
{
  public class TaskHubContext : ITaskHubContext, ITransientDependency
  {
    private readonly IVportalLogger<TaskHubContext> logger;
    private readonly ITaskAppHubContext hubContext;
    private readonly IObjectMapper objectMapper;

    public TaskHubContext(IVportalLogger<TaskHubContext> logger, ITaskAppHubContext hubContext, IObjectMapper objectMapper)
    {
      this.logger = logger;
      this.hubContext = hubContext;
      this.objectMapper = objectMapper;
    }

    public async Task TaskCompleted(TaskEntity task)
    {
      try
      {
        var taskDto = objectMapper.Map<TaskEntity, TaskHeaderDto>(task);
        await hubContext.TaskCompleted(taskDto);
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
