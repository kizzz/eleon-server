using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;

[DisableAuditing]
public class ReportExtraInformation : Entity<Guid>
{
  public string Key { get; set; }
  public string Value { get; set; }
  public ReportInformationSeverity Severity { get; set; }
  public string Type { get; set; }

  public ReportExtraInformation()
  {
  }

  public ReportExtraInformation(Guid id) : base(id)
  {
  }
}
