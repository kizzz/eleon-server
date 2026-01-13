namespace JobScheduler.Module.Triggers
{
  public class TriggerListRequestDto
  {
    public bool? IsEnabledFilter { get; set; }
    public Guid? TaskId { get; set; }
  }
}
