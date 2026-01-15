using System;
using Volo.Abp.Domain.Entities;

namespace EventManagementModule.Module.Domain.Shared.Entities;

public class EventQueueMessageBodyEntity : Entity<Guid>
{
  public byte[] Payload { get; set; } = Array.Empty<byte>();
  public string ContentType { get; set; } = "application/json";
  public string? Encoding { get; set; }

  public EventQueueMessageEntity? Message { get; set; }

  public EventQueueMessageBodyEntity()
  {
  }

  public EventQueueMessageBodyEntity(Guid id) : base(id)
  {
  }
}
