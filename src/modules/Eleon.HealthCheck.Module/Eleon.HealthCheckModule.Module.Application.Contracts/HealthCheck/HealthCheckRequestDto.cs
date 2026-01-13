using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class HealthCheckRequestDto : PagedAndSortedResultRequestDto
{
  public string Type { get; set; }
  public string Initiator { get; set; }
  public DateTime? MinTime { get; set; }
  public DateTime? MaxTime { get; set; }
}
