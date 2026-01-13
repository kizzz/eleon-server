namespace Messaging.Module.Messages
{
  public class VportalEvent
  {
    public Guid? TenantId { get; set; }
    public string TenantName { get; set; } = null!;
  }
}
