using Volo.Abp;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class DefaultExceptionClassifier : IExceptionClassifier
{
  public ExceptionKind Classify(Exception ex)
  {
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
}
