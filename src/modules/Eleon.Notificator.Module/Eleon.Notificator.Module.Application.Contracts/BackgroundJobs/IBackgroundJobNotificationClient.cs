using Messaging.Module.ETO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Notificator.Module.BackgroundJobs;

public interface IBackgroundJobNotificationClient : ITransientDependency
{
  Task AddJob(BackgroundJobEto job);
  Task UpdateJob(BackgroundJobEto job);
}
