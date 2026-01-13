using Volo.Abp.DependencyInjection;

namespace Common.EventBus.Module
{
  public class CurrentEvent : ITransientDependency
  {
    private static readonly AsyncLocal<Guid> eventId = new();
    public Guid EventId { get => eventId.Value; set => eventId.Value = value; }

    private static readonly AsyncLocal<DateTime> eventSendTime = new();
    public DateTime EventSendTime { get => eventSendTime.Value; set => eventSendTime.Value = value; }
  }
}
