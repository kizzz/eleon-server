using Messaging.Module.ETO;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.Auditor
{
  public interface IAuditorAppHubContext
  {
    Task NotifyVersionChanged(AuditVersionChangeNotificationEto notification);
  }
}
