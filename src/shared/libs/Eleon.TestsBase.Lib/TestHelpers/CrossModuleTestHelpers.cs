using System;
using System.Threading.Tasks;
using FluentAssertions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helper utilities for cross-module test scenarios.
/// </summary>
public static class CrossModuleTestHelpers
{
    /// <summary>
    /// Creates test data that spans multiple modules.
    /// </summary>
    public static class TestData
    {
        public static Guid CreateTestTenantId() => Guid.NewGuid();
        public static Guid CreateTestUserId() => Guid.NewGuid();
        public static string CreateTestTenantName() => $"TestTenant_{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Verifies cross-module interactions.
    /// </summary>
    public static class Verification
    {
        public static async Task VerifyEntityExistsAsync<TEntity, TKey>(
            IRepository<TEntity, TKey> repository,
            TKey id,
            bool shouldExist = true)
            where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
        {
            var entity = await repository.FindAsync(id);
            if (shouldExist)
            {
                entity.Should().NotBeNull($"Entity with ID {id} should exist");
            }
            else
            {
                entity.Should().BeNull($"Entity with ID {id} should not exist");
            }
        }

        public static async Task VerifyTenantIsolationAsync<TEntity, TKey>(
            IRepository<TEntity, TKey> repository,
            TKey id,
            Guid? expectedTenantId,
            ICurrentTenant currentTenant)
            where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>, Volo.Abp.MultiTenancy.IMultiTenant
        {
            using (currentTenant.Change(expectedTenantId))
            {
                var entity = await repository.FindAsync(id);
                entity.Should().NotBeNull($"Entity should be accessible in tenant {expectedTenantId}");
                entity.TenantId.Should().Be(expectedTenantId);
            }

            var otherTenantId = Guid.NewGuid();
            using (currentTenant.Change(otherTenantId))
            {
                var entity = await repository.FindAsync(id);
                entity.Should().BeNull($"Entity should not be accessible in tenant {otherTenantId}");
            }
        }
    }

    /// <summary>
    /// Sets up multi-module test scenarios.
    /// </summary>
    public static class Scenario
    {
        public static string CreateScenarioDescription(string module1, string module2, string interaction)
        {
            return $"Cross-module scenario: {module1} interacts with {module2} via {interaction}";
        }
    }
}
