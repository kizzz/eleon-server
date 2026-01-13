using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace VPortal.Notificator.Module.NotificationLogs
{
  public interface INotificationLogAppService : IApplicationService
  {
    Task<PagedResultDto<NotificationLogDto>> GetNotificationLogList(NotificationLogListRequestDto request);
  }
}
