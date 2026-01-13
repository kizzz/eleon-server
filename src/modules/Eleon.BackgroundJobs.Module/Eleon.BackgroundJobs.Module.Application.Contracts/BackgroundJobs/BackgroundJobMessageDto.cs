using Common.Module.Constants;
using System;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobMessageDto
  {
    public string TextMessage { get; set; }
    public BackgroundJobMessageType MessageType { get; set; }
    public DateTime CreationTime { get; set; }
  }
}
