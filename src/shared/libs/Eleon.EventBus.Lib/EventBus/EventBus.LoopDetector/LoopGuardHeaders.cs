using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;
public static class LoopGuardHeaders
{
  public const string Version = "lg-version";        // "1"
  public const string RootRequestId = "lg-root-request-id"; // GUID of the whole call chain
  public const string HopCount = "lg-hop-count";       // int
  public const string Chain = "lg-chain";           // compact breadcrumb string
  public const string DeadlineUtc = "lg-deadline-utc";    // ISO8601 (UTC)
  public const string TraceId = "traceparent";        // W3C traceparent (OTel)
}
