using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Notificator.Module.WebPush
{
  public interface IWebPushAppService : IApplicationService
  {
    Task<bool> AddWebPushSubscription(WebPushSubscriptionDto subscription);
  }
}
