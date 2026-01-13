

using Common.Module.Constants;

namespace VPortal.JobScheduler.Module.Actions
{
  public class ActionDto
  {
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string EventName { get; set; }
    public Guid TaskId { get; set; }
    public string ActionParams { get; set; }
    public string ActionExtraParams { get; set; }

    public int? RetryInterval { get; set; }
    public int MaxRetryAttempts { get; set; }

    public List<Guid> ParentActionIds { get; set; }
    public int TimeoutInMinutes { get; set; }
    public string OnFailureRecepients { get; set; }
    public TextFormat ParamsFormat { get; set; }
  }
}
