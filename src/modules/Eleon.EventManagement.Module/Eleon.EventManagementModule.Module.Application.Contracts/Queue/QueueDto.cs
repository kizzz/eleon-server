

using EventManagementModule.Module.Domain.Shared.Constants;

namespace EventManagementModule.Module.Application.Contracts.Queue;


public class QueueDto
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public Guid? TenantId { get; set; }
  public int Count { get; set; }
  // public string Type { get; set; }
  public int MessagesLimit { get; set; }
  public string DisplayName { get; set; }
  public string Forwarding { get; set; }

  public bool IsSystem => EventManagementDefaults.IsSystemQueue(Name);
}
