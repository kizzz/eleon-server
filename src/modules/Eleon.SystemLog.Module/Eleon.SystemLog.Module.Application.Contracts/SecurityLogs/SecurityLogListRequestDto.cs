using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.Infrastructure.Module.SecurityLogs
{
  public class SecurityLogListRequestDto : PagedAndSortedResultRequestDto
  {
    public new string Sorting { get; set; }
    public new int MaxResultCount { get; set; }
    public new int SkipCount { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Action { get; set; }
    public string Identity { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string ApplicationName { get; set; }
    public string ClientId { get; set; }
    public string CorrelationId { get; set; }
    public string ClientIpAddress { get; set; }
  }
}
