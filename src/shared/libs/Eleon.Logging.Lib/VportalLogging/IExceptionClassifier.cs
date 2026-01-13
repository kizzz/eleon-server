namespace Eleon.Logging.Lib.VportalLogging;

public interface IExceptionClassifier
{
  ExceptionKind Classify(Exception ex);
}
