using Eleon.Logging.Lib.VportalLogging;
using Volo.Abp;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class DefaultExceptionClassifierTests
{
  [Fact]
  public void Classify_returns_business_for_business_exception()
  {
    var classifier = new DefaultExceptionClassifier();

    var result = classifier.Classify(new BusinessException("ERR"));

    Assert.Equal(ExceptionKind.Business, result);
  }

  [Fact]
  public void Classify_returns_cancellation_for_operation_canceled()
  {
    var classifier = new DefaultExceptionClassifier();

    var result = classifier.Classify(new OperationCanceledException());

    Assert.Equal(ExceptionKind.Cancellation, result);
  }

  [Fact]
  public void Classify_returns_business_for_missing_session_id()
  {
    var classifier = new DefaultExceptionClassifier();

    var result = classifier.Classify(new InvalidOperationException("Check session endpoint enabled, but SessionId is missing"));

    Assert.Equal(ExceptionKind.Business, result);
  }

  [Fact]
  public void Classify_returns_unexpected_for_other_exceptions()
  {
    var classifier = new DefaultExceptionClassifier();

    var result = classifier.Classify(new InvalidOperationException("boom"));

    Assert.Equal(ExceptionKind.Unexpected, result);
  }
}
