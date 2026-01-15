
using Messaging.Module.ETO;
using Volo.Abp.DependencyInjection;
using VPortal.JobScheduler.Module.Tasks;

namespace JobScheduler.Module.Tasks;
public interface ITaskAppHubContext
{
  Task TaskCompleted(TaskHeaderDto task);
}
