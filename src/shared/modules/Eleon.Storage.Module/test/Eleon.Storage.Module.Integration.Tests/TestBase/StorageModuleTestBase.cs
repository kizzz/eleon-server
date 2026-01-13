using System;
using System.IO;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using VPortal.Storage.Module;

namespace Eleon.Storage.Module.Integration.Tests.TestBase;

/// <summary>
/// Base class for Storage Module integration tests with real IO.
/// Provides temp directory management and ABP integration test setup.
/// </summary>
public abstract class StorageModuleTestBase : AbpIntegratedTestBase<StorageDomainModule>, IDisposable
{
    protected string TempDirectory { get; private set; }

    protected StorageModuleTestBase()
    {
        TempDirectory = StorageTestHelpers.CreateTempDirectory($"StorageIntegrationTest_{GetType().Name}");
    }

    /// <summary>
    /// Creates a new temp directory for this test instance.
    /// </summary>
    protected string CreateTestTempDirectory(string prefix = null)
    {
        return StorageTestHelpers.CreateTempDirectory(prefix ?? $"StorageTest_{Guid.NewGuid():N}");
    }

    /// <summary>
    /// Cleans up temp directory.
    /// </summary>
    public virtual void Dispose()
    {
        if (!string.IsNullOrWhiteSpace(TempDirectory))
        {
            StorageTestHelpers.DeleteTempDirectory(TempDirectory);
        }
    }
}
