using System.Collections.Generic;

namespace VPortal.Infrastructure.Module.Result
{
  public class ResultErrorDto
  {
    public string Message { get; set; }
  }
  public class ResultSuccessDto
  {
    public string Message { get; set; }
  }

  public class ResultDto<T>
  {
    public T Value { get; set; }
    public bool IsFailed { get; set; }

    public bool IsSuccess { get; set; }
    public IList<ResultErrorDto> Errors { get; set; } = new List<ResultErrorDto>();

    public IList<ResultSuccessDto> Successes { get; set; } = new List<ResultSuccessDto>();
  }
}
