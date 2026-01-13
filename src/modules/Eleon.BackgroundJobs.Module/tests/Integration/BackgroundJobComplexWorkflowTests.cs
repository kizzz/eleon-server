using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.MultiTenancy;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Repositories;
using VPortal.BackgroundJobs.Module.Entities;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;

namespace BackgroundJobs.Module.Integration;

/// <summary>
/// Complex end-to-end workflow tests
/// </summary>
public class BackgroundJobComplexWorkflowTests : ModuleTestBase<BackgroundJobsTestStartupModule>
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
    public async Task CompleteParentChildWorkflow_FullLifecycle_Success()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create parent job
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

            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                // Start and complete parent
                var parentExecution = await domainService.StartExecutionAsync(parentJobId);
                await domainService.MarkExecutionStartedAsync(parentJobId, parentExecution.Id);
                await domainService.CompleteExecutionAsync(
                    parentJobId,
                    parentExecution.Id,
                    successfully: true,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Parent completed",
                    TestConstants.Users.TestUser,
                    false);

            // Create child job
            var childJobId = Guid.NewGuid();
            await domainService.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                childJobId,
                parentJobId,
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

                // Start and complete child
                var childExecution = await domainService.StartExecutionAsync(childJobId);
                await domainService.MarkExecutionStartedAsync(childJobId, childExecution.Id);
                await domainService.CompleteExecutionAsync(
                    childJobId,
                    childExecution.Id,
                    successfully: true,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Child completed",
                    TestConstants.Users.TestUser,
                    false);

                // Assert
                var parentJob = await GetFreshJobAsync(parentJobId);
                var childJob = await GetFreshJobAsync(childJobId);

                parentJob.Status.Should().Be(BackgroundJobStatus.Completed);
                childJob.Status.Should().Be(BackgroundJobStatus.Completed);
                childJob.ParentJobId.Should().Be(parentJobId);
            }
        });
    }

    [Fact]
    public async Task RetryWorkflowWithFailures_MultipleRetries_VariousOutcomes()
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
                // First execution fails
                var execution1 = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution1.Id);
                await domainService.CompleteExecutionAsync(
                    jobId,
                    execution1.Id,
                    successfully: false,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Failed",
                    TestConstants.Users.TestUser,
                    false);

                // Retry
                var execution2 = await domainService.StartExecutionAsync(jobId, isManualRetry: true);
                await domainService.MarkExecutionStartedAsync(jobId, execution2.Id);
                await domainService.CompleteExecutionAsync(
                    jobId,
                    execution2.Id,
                    successfully: false,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Failed again",
                    TestConstants.Users.TestUser,
                    false);

                // Second retry succeeds
                var execution3 = await domainService.StartExecutionAsync(jobId, isManualRetry: true);
                await domainService.MarkExecutionStartedAsync(jobId, execution3.Id);
                await domainService.CompleteExecutionAsync(
                    jobId,
                    execution3.Id,
                    successfully: true,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>(),
                    "Success",
                    TestConstants.Users.TestUser,
                    false);

                // Assert
                var job = await GetFreshJobAsync(jobId);
                job.Status.Should().Be(BackgroundJobStatus.Completed);
                job.Executions.Should().HaveCount(3);
            }
        });
    }

    [Fact]
    public async Task TimeoutWorkflow_CompleteTimeoutScenario_Success()
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
                60, // 60 minute timeout
                3,
                5,
                null);

            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                var execution = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Simulate timeout by completing with timeout error
                await domainService.CompleteExecutionAsync(
                    jobId,
                    execution.Id,
                    successfully: false,
                    null,
                    null,
                    new List<BackgroundJobMessageEntity>
                    {
                        BackgroundJobMessageTestDataBuilder.Create()
                            .WithType(BackgroundJobMessageType.Error)
                            .WithText("Job timed out after 60 minutes")
                            .Build()
                    },
                    "Timeout",
                    BackgroundJobsConstants.ModuleName,
                    false);

                // Assert
                var job = await GetFreshJobAsync(jobId);
                var refreshedExecution = job.Executions.First(e => e.Id == execution.Id);
                job.Status.Should().Be(BackgroundJobStatus.Errored);
                refreshedExecution.Messages.Should().Contain(m => m.TextMessage.Contains("timed out", StringComparison.OrdinalIgnoreCase));
            }
        });
    }

    [Fact]
    public async Task CancellationWorkflow_ComplexCancellation_Success()
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
                var execution = await domainService.StartExecutionAsync(jobId);
                await domainService.MarkExecutionStartedAsync(jobId, execution.Id);

                // Cancel job
                await domainService.CancelJobAsync(
                    jobId,
                    TestConstants.Users.TestUser,
                    true,
                    "Cancelled by user");

                // Assert
                var job = await GetFreshJobAsync(jobId);
                var refreshedExecution = job.Executions.First(e => e.Id == execution.Id);
                job.Status.Should().Be(BackgroundJobStatus.Cancelled);
                refreshedExecution.Status.Should().Be(BackgroundJobExecutionStatus.Cancelled);
            }
        });
    }

    [Fact]
    public async Task ConcurrentWorkflow_MultipleWorkflowsSimultaneously_Success()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var domainService = GetDomainService();

            // Create multiple jobs
            var jobIds = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList();

            foreach (var jobId in jobIds)
            {
                await domainService.CreateAsync(
                    TestConstants.TenantIds.Tenant1,
                    jobId,
                    null,
                    TestConstants.JobTypes.TestJob,
                    TestConstants.Users.TestUser,
                    true,
                    $"Job {jobId}",
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

            // Act - Process all jobs concurrently
            var currentTenant = GetCurrentTenant();
            using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
            {
                var workflows = jobIds.Select(async jobId =>
                {
                    var execution = await domainService.StartExecutionAsync(jobId);
                    await domainService.MarkExecutionStartedAsync(jobId, execution.Id);
                    await domainService.CompleteExecutionAsync(
                        jobId,
                        execution.Id,
                        successfully: true,
                        null,
                        null,
                        new List<BackgroundJobMessageEntity>(),
                        "Success",
                        TestConstants.Users.TestUser,
                        false);
                }).ToList();

                await Task.WhenAll(workflows);
            }

            // Assert
            foreach (var jobId in jobIds)
            {
                using (currentTenant.Change(TestConstants.TenantIds.Tenant1))
                {
                    var job = await repository.GetAsync(jobId, includeDetails: true);
                    job.Status.Should().Be(BackgroundJobStatus.Completed);
                }
            }
        });
    }
}
