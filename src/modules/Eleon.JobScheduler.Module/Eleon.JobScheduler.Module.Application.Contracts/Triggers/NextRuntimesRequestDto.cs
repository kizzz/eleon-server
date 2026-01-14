using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.JobScheduler.Module.Full.Eleon.JobScheduler.Module.Application.Contracts.Triggers
{
  public class NextRuntimesRequestDto
  {
    public Guid TriggerId { get; set; }
    public DateTime? FromUtc { get; set; }
    public int Count { get; set; }
  }
}
