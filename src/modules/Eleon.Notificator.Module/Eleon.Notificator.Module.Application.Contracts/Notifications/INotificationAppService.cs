using Volo.Abp.Application.Services;

namespace VPortal.Notificator.Module.Notifications
{
  public interface INotificationAppService : IApplicationService
  {
    Task<bool> SendAsync(NotificationDto input);
    Task<bool> SendBulkAsync(List<NotificationDto> input);
  }
}
