using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp.Modularity;

namespace CollaborationModule.Test.TestBase;

/// <summary>
/// Base class for Collaboration module integration tests.
/// Uses SQLite in-memory database and real EF Core repositories.
/// </summary>
public abstract class CollaborationIntegrationTestBase : AbpIntegratedTestBase<CollaborationTestStartupModule>
{
    // Integration test base with real EF Core + SQLite in-memory
    // Extends AbpIntegratedTestBase with Collaboration-specific helpers
}
