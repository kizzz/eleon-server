using Common.Module.Constants;
using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.Auditor.Module.AuditHistoryRecords
{
  public class DocumentHistoryRequest : PagedAndSortedResultRequestDto
  {
    public string DocumentObjectType { get; set; }
    public string DocumentId { get; set; }
    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }
  }
}
