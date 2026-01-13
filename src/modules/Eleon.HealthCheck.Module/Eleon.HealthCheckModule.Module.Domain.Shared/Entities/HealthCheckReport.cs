using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;


[DisableAuditing]
public class HealthCheckReport : CreationAuditedEntity<Guid>, IMultiTenant
{
  public virtual string ServiceName { get; set; }
  public virtual string ServiceVersion { get; set; }
  public virtual TimeSpan UpTime { get; set; }
  public virtual string CheckName { get; set; }
  public virtual HealthCheckStatus Status { get; set; }
  public virtual string Message { get; set; }
  public virtual Guid? TenantId { get; set; }
  public virtual bool IsPublic { get; set; }

  public virtual Guid HealthCheckId { get; set; }

  // use extra properties to store additional info like response time, error details, etc.

  public virtual ICollection<ReportExtraInformation> ExtraInformation { get; set; } = new List<ReportExtraInformation>();

  public HealthCheckReport()
  {
  }

  public HealthCheckReport(Guid id) : base(id)
  {
  }
}
