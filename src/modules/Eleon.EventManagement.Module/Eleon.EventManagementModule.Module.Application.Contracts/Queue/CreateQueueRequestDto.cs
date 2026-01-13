namespace EventManagementModule.Module.Application.Contracts.Queue;
public class CreateQueueRequestDto
{
  public string Name { get; set; }
  public string DisplayName { get; set; }
  public int MessagesLimit { get; set; }
  public string Forwarding { get; set; }
}
