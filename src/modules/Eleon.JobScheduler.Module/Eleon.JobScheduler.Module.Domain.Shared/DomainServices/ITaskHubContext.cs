using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.JobScheduler.Module.Entities;

namespace Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices
{
  public interface ITaskHubContext
  {
    Task TaskCompleted(TaskEntity task);
  }
}
