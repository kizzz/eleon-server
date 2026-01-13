
using Messaging.Module.ETO;
using Volo.Abp.DependencyInjection;

namespace BackgroundJobs.Module.BackgroundJobs;
public interface IBackgroundJobAppHubContext
{
  Task JobCompleted(BackgroundJobEto job);
}
