using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.Notificator.Module.NotificationLogs
{
  public class NotificationLogListRequestDto : PagedAndSortedResultRequestDto
  {
    public List<NotificationType> TypeFilter { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string ApplicationName { get; set; }
  }
}
