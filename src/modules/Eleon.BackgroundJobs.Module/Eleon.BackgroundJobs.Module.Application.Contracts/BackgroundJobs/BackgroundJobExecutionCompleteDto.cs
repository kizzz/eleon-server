using Common.Module.Constants;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobExecutionCompleteDto
  {
    public string ParamsForRetryExecution { get; set; }
    public string ExtraParamsForRetryExecution { get; set; }

    public BackgroundJobExecutionStatus Status { get; set; }
    public string Type { get; set; }
    public Guid ExecutionId { get; set; }
    public Guid BackgroundJobId { get; set; }
    public List<BackgroundJobMessageDto> Messages { get; set; }
    public string Result { get; set; }
  }
}
