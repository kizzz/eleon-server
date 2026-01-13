using Common.Module.Constants;

namespace Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
public class StartNewLifecycleRequestDto
{
  public Guid TemplateId { get; set; }
  public string DocumentId { get; set; }
  public string DocumentObjectType { get; set; }
  public bool StartImmediately { get; set; }
  public bool IsSkipFilled { get; set; }

  public Dictionary<string, object> ExtraProperties { get; set; }
}
