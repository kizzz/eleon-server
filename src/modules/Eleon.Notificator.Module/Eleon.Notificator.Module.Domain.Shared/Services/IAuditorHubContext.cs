using Messaging.Module.ETO;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.Services
{
  public interface IAuditorHubContext
  {
    Task NotifyVersionChanged(AuditVersionChangeNotificationEto notification);
  }
}
