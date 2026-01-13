using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;

[DisableAuditing]
public class HealthCheck : FullAuditedAggregateRoot<Guid>
{
  public string Type { get; set; }
  public string InitiatorName { get; set; }

  public bool InProgress { get; set; } = true;
  public HealthCheckStatus Status { get; set; }

  [NotMapped]
  public List<HealthCheckReport> Reports { get; set; } = new List<HealthCheckReport>();


  public HealthCheck()
  {
  }

  public HealthCheck(Guid id) : base(id)
  {
  }


}
