using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.Integration;

/// <summary>
/// Integration tests for complete job lifecycles and workflows
/// </summary>
public class BackgroundJobWorkflowTests : ModuleTestBase<BackgroundJobsTestStartupModule>
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

    private async Task<BackgroundJobEntity> GetFreshJobAsync(Guid jobId)
    {
        using var scope = ServiceProvider.CreateScope();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin();
        var repository = scope.ServiceProvider.GetRequiredService<IBackgroundJobsRepository>();
        var job = await repository.GetAsync(jobId, includeDetails: true);
        await uow.CompleteAsync();
        return job;
    }

    [Fact]
    public async Task CompleteJobLifecycle_CreateStartMarkStartedComplete_Success()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();
            var currentTenant = GetCurrentTenant();
            var jobId = Guid.NewGuid();
            var tenantId = TestConstants.TenantIds.Tenant1;

            // Act - Create job
            var job = await domainService.CreateAsync(
                tenantId,
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

            // Set tenant context so StartExecutionAsync can find the job
            using (currentTenant.Change(tenantId))
            {
                // Act - Start execution
                var execution = await domainService.StartExecutionAsync(jobId);

                // Act - Mark execution started
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Act - Complete execution
                var completedExecution = await domainService.CompleteExecutionAsync(
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
                completedExecution.Should().NotBeNull();
                completedExecution.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
                
                var finalJob = await GetFreshJobAsync(jobId);
                finalJob.Status.Should().Be(BackgroundJobStatus.Completed);
                finalJob.JobFinishedUtc.Should().NotBeNull();
            }
        });
    }

    [Fact]
    public async Task RetryWorkflow_CreateStartErrorRetryComplete_Success()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();
            var currentTenant = GetCurrentTenant();
            var jobId = Guid.NewGuid();
            var tenantId = TestConstants.TenantIds.Tenant1;

            // Create and start job
            await domainService.CreateAsync(
                tenantId,
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

            // Set tenant context so StartExecutionAsync can find the job
            using (currentTenant.Change(tenantId))
            {
                var execution = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Complete with error
                await domainService.CompleteExecutionAsync(
                    jobId,
                    execution.Id,
                    successfully: false,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    null,
                    TestConstants.Users.TestUser,
                    false);

                // Retry
                var retryResult = await domainService.RetryJob(jobId);

                // Assert
                retryResult.Should().BeTrue();
                var job = await GetFreshJobAsync(jobId);
                job.Status.Should().Be(BackgroundJobStatus.Retring);
                job.Executions.Should().HaveCount(2); // Original + retry
            }
        });
    }

    [Fact]
    public async Task ConcurrencyHandling_MultipleCompletions_HandlesGracefully()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();
            var currentTenant = GetCurrentTenant();
            var jobId = Guid.NewGuid();
            var executionId = TestConstants.ExecutionIds.Execution1;
            var tenantId = TestConstants.TenantIds.Tenant1;

            // Create and start job
            await domainService.CreateAsync(
                tenantId,
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

            // Set tenant context so StartExecutionAsync can find the job
            using (currentTenant.Change(tenantId))
            {
                var execution = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Act - Complete execution
                var completedExecution1 = await domainService.CompleteExecutionAsync(
                    jobId,
                    execution.Id,
                    successfully: true,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Success",
                    TestConstants.Users.TestUser,
                    false);

                // Act - Try to complete again (idempotency test)
                var completedExecution2 = await domainService.CompleteExecutionAsync(
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
                completedExecution1.Should().NotBeNull();
                completedExecution2.Should().NotBeNull();
                completedExecution1.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
                completedExecution2.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
            }
        });
    }

    [Fact]
    public async Task MultiTenantIsolation_JobsIsolatedByTenant()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create jobs for different tenants
            var job1Id = Guid.NewGuid();
            var job2Id = Guid.NewGuid();
            var job1 = await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                job1Id,
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

            var job2 = await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant2,
                job2Id,
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

            // Assert
            job1.TenantId.Should().Be((Guid?)TestConstants.TenantIds.Tenant1);
            job2.TenantId.Should().Be((Guid?)TestConstants.TenantIds.Tenant2);
            (job1.TenantId != job2.TenantId).Should().BeTrue();
        });
    }
}
