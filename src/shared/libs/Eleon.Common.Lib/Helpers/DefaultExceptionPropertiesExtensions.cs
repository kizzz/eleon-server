
namespace Logging.Module.ErrorHandling.Extensions;
public static class DefaultExceptionPropertiesExtensions
{
  public static Exception WithStatusCode(this Exception exception, int statusCode, bool force = false)
  {
    if (!force && exception.Data.Contains("StatusCode"))
    {
      return exception;
    }

    exception.Data["StatusCode"] = statusCode;
    return exception;
  }

  public static Exception WithFriendlyMessage(this Exception exception, string message, bool force = false)
  {
    if (!force && exception.Data.Contains("FriendlyMessage"))
    {
      return exception;
    }

    exception.Data["FriendlyMessage"] = message;
    return exception;
  }

  public static Exception WithExceptionId(this Exception exception, string exceptionId, bool force = false)
  {
    if (!force && exception.Data.Contains("ExceptionId"))
    {
      return exception;
    }

    exception.Data["ExceptionId"] = exceptionId;
    return exception;
  }

  public static Exception WithData(this Exception exception, string key, object value, bool force = false)
  {
    if (!force && exception.Data.Contains(key))
    {
      return exception;
    }

    exception.Data[key] = value;
    return exception;
  }

  public static Exception WithMessageAsFriendly(this Exception exception)
  {
    exception.Data["FriendlyMessage"] = exception.Message;
    return exception;
  }

  public static int GetStatusCode(this Exception exception)
  {
    if (exception.Data.Contains("StatusCode") && exception.Data["StatusCode"] is int statusCode)
    {
      return statusCode;
    }
    return 500; // Default to 500 if not set
  }
}
