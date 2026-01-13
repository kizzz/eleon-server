using System.Threading;
namespace Eleon.Tests.Common;

public abstract class TestBase
{
    protected CancellationToken DefaultCancellationToken => CancellationToken.None;
}
