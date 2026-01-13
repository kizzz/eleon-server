

namespace EventManagementModule.Module.Application.Contracts.Event;
public class PublishMessageRequestDto
{
  public string Name { get; set; }
  public string QueueName { get; set; }
  public string Message { get; set; }
}
