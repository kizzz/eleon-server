using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Queue;

/// <summary>
/// Using queue request dto for operations that require some queue but in future the queue identifier can be changed.
/// Examples: add tenant id or using queue id instead of name
/// </summary>
public class QueueRequestDto
{
  public string QueueName { get; set; }
}
