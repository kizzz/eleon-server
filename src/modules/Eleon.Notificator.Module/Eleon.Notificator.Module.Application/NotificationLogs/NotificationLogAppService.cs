using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.NotificationLogs
{
  public class NotificationLogAppService : NotificatorModuleAppService, INotificationLogAppService
  {
    private readonly IVportalLogger<NotificationLogAppService> logger;
    private readonly NotificationLogDomainService domainService;

    public NotificationLogAppService(
        IVportalLogger<NotificationLogAppService> logger,
        NotificationLogDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<PagedResultDto<NotificationLogDto>> GetNotificationLogList(NotificationLogListRequestDto request)
    {
      PagedResultDto<NotificationLogDto> result = null;
      try
      {
        var list = await domainService.GetListAsync(request.SkipCount, request.MaxResultCount, request.ApplicationName, request.Sorting, request.TypeFilter, request.FromDate, request.ToDate);
        var items = ObjectMapper.Map<List<NotificationLogEntity>, List<NotificationLogDto>>(list.Value);
        result = new(list.Key, items);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
