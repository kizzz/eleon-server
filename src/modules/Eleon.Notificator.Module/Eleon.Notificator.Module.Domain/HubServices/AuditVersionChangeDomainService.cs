using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Notificator.Module.Services;

namespace VPortal.Notificator.Module.DomainServices
{

  public class AuditVersionChangeDomainService : DomainService
  {
    private readonly IVportalLogger<AuditVersionChangeDomainService> _logger;
    private readonly IAuditorHubContext _hubContext;

    public AuditVersionChangeDomainService(
        IVportalLogger<AuditVersionChangeDomainService> logger,
        IAuditorHubContext hubContext)
    {
      _logger = logger;
      _hubContext = hubContext;
    }

    public async Task NotifyVersionChanged(AuditVersionChangeNotificationEto notification)
    {
      try
      {
        await _hubContext.NotifyVersionChanged(notification);
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
