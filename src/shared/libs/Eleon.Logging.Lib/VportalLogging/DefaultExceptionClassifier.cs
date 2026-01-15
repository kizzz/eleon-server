using Volo.Abp;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class DefaultExceptionClassifier : IExceptionClassifier
{
  private const string MissingSessionIdMessage = "Check session endpoint enabled, but SessionId is missing";

  public ExceptionKind Classify(Exception ex)
  {
    if (IsMissingSessionIdException(ex))
    {
      return ExceptionKind.Business;
    }

    if (ex is IBusinessException)
    {
      return ExceptionKind.Business;
    }

    if (ex is OperationCanceledException || ex is TaskCanceledException)
    {
      return ExceptionKind.Cancellation;
    }

    return ExceptionKind.Unexpected;
  }

  private static bool IsMissingSessionIdException(Exception ex)
  {
    if (string.Equals(ex.Message, MissingSessionIdMessage, StringComparison.Ordinal))
    {
      return true;
    }

    return ex.InnerException != null
      && string.Equals(ex.InnerException.Message, MissingSessionIdMessage, StringComparison.Ordinal);
  }
}
