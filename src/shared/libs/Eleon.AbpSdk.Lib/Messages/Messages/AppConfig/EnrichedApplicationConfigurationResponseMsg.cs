using Common.Module.Events;
using Messaging.Module.ETO;

namespace EleonsoftAbp.Messages.AppConfig;

[DistributedEvent]
public class EnrichedApplicationConfigurationResponseMsg
{
  public List<ClientApplicationEto> Applications { get; set; }
  public Guid? TenantId { get; set; }
  public string ErrorMessage { get; set; }
  public bool IsSuccess { get; set; }
}
