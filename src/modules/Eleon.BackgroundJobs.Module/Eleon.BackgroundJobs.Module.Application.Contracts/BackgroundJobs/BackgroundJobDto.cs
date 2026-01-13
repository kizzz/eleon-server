using Common.Module.Constants;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobDto : BackgroundJobHeaderDto
  {
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public string Result { get; set; }
    public List<BackgroundJobExecutionDto> Executions { get; set; }
  }
}
