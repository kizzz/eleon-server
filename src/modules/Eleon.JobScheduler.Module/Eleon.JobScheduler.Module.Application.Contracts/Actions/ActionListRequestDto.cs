using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Actions;
public class ActionListRequestDto
{
  public Guid? TaskId { get; set; }
  public string? NameFilter { get; set; }
  public string Sorting { get; set; }
}
