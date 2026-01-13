using Common.Module.ValueObjects;

namespace Common.Module.Helpers
{
  public class ResultError
  {
    public string Message { get; set; }
    public ResultError() { }
    public ResultError(string message)
    {
      Message = message;
    }
  }
  public class ResultSuccess
  {
    public string Message { get; set; }
    public ResultSuccess()
    {

    }
    public ResultSuccess(string message)
    {
      Message = message;
    }
  }
  public class ResultValueObject<T> : ResultMessageValueObject
  {
    public T Value { get; set; }
  }
  public class ResultMessageValueObject
  {
    public bool IsSuccess { get; set; }
    public bool IsFailed
    {
      get
      {
        return !IsSuccess;
      }
    }
    public List<ResultError> Errors { get; set; }
    public List<ResultSuccess> Successes { get; set; }
  }
  public static class ResultHelper
  {
    public static ResultValueObject<T> Fail<T>(List<string> errors)
    {
      return new ResultValueObject<T>()
      {
        Errors = errors.Select(e => new ResultError(e)).ToList(),
        IsSuccess = false,
        Successes = new List<ResultSuccess>(),
      };
    }

    public static ResultValueObject<T> Fail<T>(List<string> errors, T? value)
    {
      return new ResultValueObject<T>()
      {
        Errors = errors.Select(e => new ResultError(e)).ToList(),
        Value = value,
        IsSuccess = false,
        Successes = new List<ResultSuccess>(),
      };
    }
    public static ResultValueObject<T> Fail<T>(string error, T? value)
    {
      return Fail(new List<string>() { error }, value);
    }

    public static ResultValueObject<T> Fail<T>(string message)
    {
      return Fail<T>(new List<string>() { message });
    }

    public static ResultValueObject<T> Ok<T>(T result)
    {
      return new ResultValueObject<T>()
      {
        IsSuccess = true,
        Value = result,
        Successes = new List<ResultSuccess>(),
        Errors = new List<ResultError>(),
      };
    }
  }
}
