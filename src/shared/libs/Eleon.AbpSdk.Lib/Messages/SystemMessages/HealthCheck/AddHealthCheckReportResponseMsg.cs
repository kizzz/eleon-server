using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

[DistributedEvent]
public class AddHealthCheckReportResponseMsg : VportalEvent
{
  public Guid ReportId { get; set; }
  public bool IsSuccess { get; set; }
}


[DistributedEvent]
public class AddHealthCheckReportBulkResponseMsg : VportalEvent
{
  public List<Guid> ReportIds { get; set; } = new List<Guid>();
  public bool IsSuccess { get; set; }

}
