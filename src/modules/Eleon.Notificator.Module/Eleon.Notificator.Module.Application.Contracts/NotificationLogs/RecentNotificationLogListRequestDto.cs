using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.Notificator.Module.NotificationLogs
{
  public class RecentNotificationLogListRequestDto : PagedAndSortedResultRequestDto
  {
    public string ApplicationName { get; set; }
  }
}
