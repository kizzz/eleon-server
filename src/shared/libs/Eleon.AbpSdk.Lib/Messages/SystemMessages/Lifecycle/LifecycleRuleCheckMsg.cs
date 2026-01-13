using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class LifecycleRuleCheckMsg : VportalEvent
  {
    public DocumentTemplateElementMapFunctionType FunctionType { get; set; }
    public required string Function { get; set; }
    public required string SourceXml { get; set; }
  }
}
