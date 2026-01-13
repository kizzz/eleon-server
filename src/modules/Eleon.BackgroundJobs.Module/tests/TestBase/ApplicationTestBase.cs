using Eleon.TestsBase.Lib.TestBase;
using NSubstitute;
using VPortal.BackgroundJobs.Module.DomainServices;
using Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.Domain.Shared.DomainServices;

namespace BackgroundJobs.Module.TestBase;

public abstract class ApplicationTestBase : MockingTestBase
{
    protected IBackgroundJobDomainService CreateMockDomainService()
    {
        // Use the interface instead of concrete class to avoid NSubstitute void method issues
        return Substitute.For<IBackgroundJobDomainService>();
    }
}
