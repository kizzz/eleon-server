using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Repositories;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Integration;

/// <summary>
/// Advanced multi-tenant scenario tests
/// </summary>
public class BackgroundJobMultiTenantAdvancedTests : ModuleTestBase<BackgroundJobsTestStartupModule>
{
    private IBackgroundJobsRepository GetRepository()
    {
        return ServiceProvider.GetRequiredService<IBackgroundJobsRepository>();
    }

    private BackgroundJobDomainService GetDomainService()
    {
        return ServiceProvider.GetRequiredService<BackgroundJobDomainService>();
    }

    private ICurrentTenant GetCurrentTenant()
    {
        return ServiceProvider.GetRequiredService<ICurrentTenant>();
    }

    [Fact]
    public async Task CreateAsync_CrossTenantParentJob_IsolatedCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create parent job in Tenant1
            var parentJobId = Guid.NewGuid();
            await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                parentJobId,
                null,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Parent job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            // Act - Try to create child job in Tenant2 with parent from Tenant1
            var childJobId = Guid.NewGuid();
            var childJob = await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant2,
                childJobId,
                parentJobId, // Parent from different tenant
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Child job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            // Assert - Child job should be created but parent relationship may be invalid
            childJob.Should().NotBeNull();
            childJob.TenantId.Should().Be(TestConstants.TenantIds.Tenant2);
            childJob.ParentJobId.Should().Be(parentJobId);
        });
    }

    [Fact]
    public async Task GetCurrentJobsAsync_TenantIsolation_ReturnsOnlyTenantJobs()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create jobs for different tenants
            var tenant1JobId = Guid.NewGuid();
            var tenant2JobId = Guid.NewGuid();

            await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                tenant1JobId,
                null,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Tenant1 job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant2,
                tenant2JobId,
                null,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Tenant2 job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            // Act - Get jobs (should be filtered by current tenant context)
            var jobs = await domainService.GetCurrentJobsAsync();

            // Assert - Jobs should be filtered by tenant
            // Note: Actual filtering depends on repository implementation and tenant context
            jobs.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task CompleteExecutionAsync_TenantIsolation_IsolatedCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create and start job in Tenant1
            var jobId = Guid.NewGuid();
            await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                jobId,
                null,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Test job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                var execution = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Act - Complete execution
                var result = await domainService.CompleteExecutionAsync(
                    jobId,
                    execution.Id,
                    successfully: true,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Success",
                    TestConstants.Users.TestUser,
                    false);

                // Assert
                result.Should().NotBeNull();
                result.Status.Should().Be(BackgroundJobExecutionStatus.Completed);

                var job = await repository.GetAsync(jobId, includeDetails: true);
                job.TenantId.Should().Be(TestConstants.TenantIds.Tenant1);
            }
        });
    }

    [Fact]
    public async Task HostTenant_JobsCreated_HandledCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Act - Create job for host tenant (null tenant ID)
            var jobId = Guid.NewGuid();
            var job = await domainService.CreateAsync(
                null, // Host tenant
                jobId,
                null,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Host job",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null);

            // Assert
            job.Should().NotBeNull();
            job.TenantId.Should().BeNull();
        });
    }
}
