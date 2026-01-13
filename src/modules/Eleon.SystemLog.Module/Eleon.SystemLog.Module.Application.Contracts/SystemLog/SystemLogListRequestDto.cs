using Eleon.Logging.Lib.SystemLog.Contracts;
using Volo.Abp.Application.Dtos;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;

public class SystemLogListRequestDto : PagedAndSortedResultRequestDto
{
  public string SearchQuery { get; set; }
  public string DocIdFilter { get; set; }
  public SystemLogLevel? MinLogLevelFilter { get; set; }
  public string InitiatorFilter { get; set; }
  public string InitiatorTypeFilter { get; set; }
  public string ApplicationNameFilter { get; set; }
  public DateTime? CreationFromDateFilter { get; set; }
  public DateTime? CreationToDateFilter { get; set; }
  public bool OnlyUnread { get; set; } = false;
}
