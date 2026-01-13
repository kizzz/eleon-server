using Volo.Abp.EventBus;

namespace Common.Module.Events
{
  [AttributeUsage(AttributeTargets.Class)]
  public class DistributedEventAttribute : Attribute, IEventNameProvider
  {
    public virtual string Name { get; }

    public DistributedEventAttribute(String name = null)
    {
      Name = name;
    }

    public string GetName(Type eventType)
    {
      return Name ?? eventType.Name;
    }
  }
}
