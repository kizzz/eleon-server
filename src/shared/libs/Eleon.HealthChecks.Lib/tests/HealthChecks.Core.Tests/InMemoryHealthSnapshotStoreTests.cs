using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Core;

public class InMemoryHealthSnapshotStoreTests
{
    [Fact]
    public void Store_ShouldReplacePreviousSnapshot()
    {
        // Arrange
        var store = new InMemoryHealthSnapshotStore();
        var snapshot1 = CreateSnapshot("1");
        var snapshot2 = CreateSnapshot("2");

        // Act
        store.Store(snapshot1);
        store.Store(snapshot2);

        // Assert
        var latest = store.GetLatest();
        Assert.NotNull(latest);
        Assert.Equal("2", latest.Type);
    }

    [Fact]
    public void GetLatest_ShouldReturnNull_WhenEmpty()
    {
        // Arrange
        var store = new InMemoryHealthSnapshotStore();

        // Act
        var result = store.GetLatest();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetLatest_ShouldReturnLatest_AfterMultipleStores()
    {
        // Arrange
        var store = new InMemoryHealthSnapshotStore();
        var snapshot1 = CreateSnapshot("1");
        var snapshot2 = CreateSnapshot("2");
        var snapshot3 = CreateSnapshot("3");

        // Act
        store.Store(snapshot1);
        store.Store(snapshot2);
        store.Store(snapshot3);

        // Assert
        var latest = store.GetLatest();
        Assert.NotNull(latest);
        Assert.Equal("3", latest.Type);
    }

    [Fact]
    public void Clear_ShouldRemoveAllSnapshots()
    {
        // Arrange
        var store = new InMemoryHealthSnapshotStore();
        store.Store(CreateSnapshot("1"));

        // Act
        store.Clear();

        // Assert
        var result = store.GetLatest();
        Assert.Null(result);
    }

    [Fact]
    public void Store_ShouldBeThreadSafe()
    {
        // Arrange
        var store = new InMemoryHealthSnapshotStore();
        var tasks = new List<Task>();

        // Act - Concurrent stores
        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => store.Store(CreateSnapshot($"snapshot-{index}"))));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert - Should have a snapshot (last one stored)
        var latest = store.GetLatest();
        Assert.NotNull(latest);
    }

    private static HealthSnapshot CreateSnapshot(string type)
    {
        return new HealthSnapshot(
            Guid.NewGuid(),
            DateTime.UtcNow,
            type,
            "test",
            new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto(),
            true,
            TimeSpan.Zero);
    }
}
