using Eleon.TestsBase.Lib.TestBase;

namespace VPortal.FileManager.Module.Tests.TestBase;

public abstract class ApplicationTestBase : MockingTestBase
{
    // Application test base for application layer unit tests
    // Domain services are typically not mocked in application tests since they don't have interfaces
    // Instead, mock the repositories and other dependencies that application services use
}
