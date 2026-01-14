using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.WebPush
{
  public interface IWebPushAppService : IApplicationService
  {
    Task<bool> AddWebPushSubscriptionAsync(WebPushSubscriptionDto subscription);
  }
}
