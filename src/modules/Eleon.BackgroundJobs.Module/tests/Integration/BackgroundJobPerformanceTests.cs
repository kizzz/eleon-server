using System;
using System.Collections.Generic;
using System.Diagnostics;
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
/// Performance and scale tests
/// </summary>
public class BackgroundJobPerformanceTests : ModuleTestBase<BackgroundJobsTestStartupModule>
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
    public async Task BulkJobCreation_Creates1000Jobs_Efficiently()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();
            var jobCount = 1000;

            // Act
            var stopwatch = Stopwatch.StartNew();
            var tasks = Enumerable.Range(0, jobCount).Select(async i =>
            {
                var jobId = Guid.NewGuid();
                return await domainService.CreateAsync(
                    TestConstants.TenantIds.Tenant1,
                    jobId,
                    null,
                    TestConstants.JobTypes.TestJob,
                    TestConstants.Users.TestUser,
                    true,
                    $"Job {i}",
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
            }).ToList();

            var jobs = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            jobs.Should().HaveCount(jobCount);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // Should complete in < 30 seconds
        });
    }

    [Fact]
    public async Task QueryPerformance_LargeDataset_ReturnsQuickly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create 500 jobs
            for (int i = 0; i < 500; i++)
            {
                var jobId = Guid.NewGuid();
                await domainService.CreateAsync(
                    TestConstants.TenantIds.Tenant1,
                    jobId,
                    null,
                    TestConstants.JobTypes.TestJob,
                    TestConstants.Users.TestUser,
                    true,
                    $"Job {i}",
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
            }

            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                // Act
                var stopwatch = Stopwatch.StartNew();
                var result = await domainService.GetBackgroundJobsList(
                    sorting: "CreationTime DESC",
                    maxResultCount: 100,
                    skipCount: 0);
                stopwatch.Stop();

                // Assert
                result.Value.Should().HaveCount(100);
                stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should be fast
            }
        });
    }

    [Fact]
    public async Task ConcurrentOperations_100ConcurrentOperations_HandlesCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create jobs
            var jobIds = new List<Guid>();
            for (int i = 0; i < 100; i++)
            {
                var jobId = Guid.NewGuid();
                jobIds.Add(jobId);
                await domainService.CreateAsync(
                    TestConstants.TenantIds.Tenant1,
                    jobId,
                    null,
                    TestConstants.JobTypes.TestJob,
                    TestConstants.Users.TestUser,
                    true,
                    $"Job {i}",
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
            }

            // Act - 100 concurrent start operations
            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                var stopwatch = Stopwatch.StartNew();
                var tasks = jobIds.Select(jobId =>
                    domainService.StartExecutionAsync(jobId)
                ).ToList();

                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();

                // Assert
                results.Should().HaveCount(100);
                results.Should().AllSatisfy(r => r.Should().NotBeNull());
                stopwatch.ElapsedMilliseconds.Should().BeLessThan(60000); // Should complete in < 60 seconds
            }
        });
    }

    [Fact]
    public async Task LargeExecutionLists_JobWith100Executions_HandlesEfficiently()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();
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
                // Create 100 executions
                for (int i = 0; i < 100; i++)
                {
                    var execution = await domainService.StartExecutionAsync(jobId);
                    await domainService.MarkExecutionStartedAsync(jobId, execution.Id);
                    await domainService.CompleteExecutionAsync(
                        jobId,
                        execution.Id,
                        successfully: false,
                        null,
                        null,
                        new List<BackgroundJobMessageEntity>(),
                        $"Result {i}",
                        TestConstants.Users.TestUser,
                        false);
                }
            }

            // Act
            var stopwatch = Stopwatch.StartNew();
            BackgroundJobEntity job;
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                job = await repository.GetAsync(jobId, includeDetails: true);
            }
            stopwatch.Stop();

            // Assert
            job.Should().NotBeNull();
            job.Executions.Should().HaveCount(100);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should load quickly
        });
    }
}
