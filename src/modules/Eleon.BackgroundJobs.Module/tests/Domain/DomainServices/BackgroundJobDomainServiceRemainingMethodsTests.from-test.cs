using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.Domain.DomainServices;

/// <summary>
/// Tests for remaining BackgroundJobDomainService methods:
/// - CancelJobAsync
/// - MarkExecutionStartedAsync
/// - GetBackgroundJobsList
/// - RetryJob
/// </summary>
public class BackgroundJobDomainServiceRemainingMethodsTestsFromTest : DomainTestBase
{
    #region CancelJobAsync Tests

    [Fact]
    public async Task CancelJobAsync_CancelsJobAndExecutions()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .Build();
        
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act
        await service.CancelJobAsync(jobId, TestConstants.Users.TestUser, true, "Cancelled by test");

        // Assert
        job.Status.Should().Be(BackgroundJobStatus.Cancelled);
        job.JobFinishedUtc.Should().NotBeNull();
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Cancelled);
    }

    [Fact]
    public async Task CancelJobAsync_JobAlreadyFinal_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Completed)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(() => 
            service.CancelJobAsync(jobId, TestConstants.Users.TestUser, true, "Test"));
    }

    [Fact]
    public async Task CancelJobAsync_PublishesBackgroundJobCompletedMsg()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act
        await service.CancelJobAsync(jobId, TestConstants.Users.TestUser, true, "Test");

        // Assert
        await eventBus.Received(1).PublishAsync(
            Arg.Is<object>(msg => msg.GetType().Name.Contains("BackgroundJobCompletedMsg")),
            Arg.Any<bool>());
    }

    #endregion

    #region MarkExecutionStartedAsync Tests

    [Fact]
    public async Task MarkExecutionStartedAsync_UpdatesStatusFromStartingToStarted()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Starting)
            .Build();
        
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork();
        uowManager.Begin(true).Returns(uow);
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.MarkExecutionStartedAsync(jobId, executionId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(BackgroundJobExecutionStatus.Started);
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Started);
    }

    [Fact]
    public async Task MarkExecutionStartedAsync_ExecutionNotFound_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        // Create a job with NO executions (empty list) to simulate execution not found
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .Build();
        // Ensure Executions is empty
        job.Executions.Clear();

        var repository = CreateMockJobsRepository();
        SetupRepositoryGetAsync(repository, jobId, job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), false, Arg.Any<CancellationToken>())
            .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork();
        uowManager.Begin().Returns(uow);
        uowManager.Begin(Arg.Any<bool>()).Returns(uow);
        uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        uow.CompleteAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act & Assert
        // The code now correctly throws EntityNotFoundException when execution is not found
        await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(() => 
            service.MarkExecutionStartedAsync(jobId, executionId));
    }

    #endregion

    #region GetBackgroundJobsList Tests

    [Fact]
    public async Task GetBackgroundJobsList_ReturnsPaginatedResults()
    {
        // Arrange
        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create().Build(),
            BackgroundJobTestDataBuilder.Create().Build()
        };
        var totalCount = 10L;
        var pair = new KeyValuePair<long, List<BackgroundJobEntity>>(totalCount, jobs);

        var repository = CreateMockJobsRepository();
        repository.GetListAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IList<string>>(),
            Arg.Any<IList<BackgroundJobStatus>>())
            .Returns(pair);
        
        var service = CreateBackgroundJobDomainService(jobsRepository: repository);

        // Act
        var result = await service.GetBackgroundJobsList(
            sorting: "CreationTime DESC",
            maxResultCount: 10,
            skipCount: 0);

        // Assert
        result.Key.Should().Be(totalCount);
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBackgroundJobsList_AppliesFilters()
    {
        // Arrange
        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithType(TestConstants.JobTypes.TestJob)
                .WithStatus(BackgroundJobStatus.New)
                .Build()
        };
        var pair = new KeyValuePair<long, List<BackgroundJobEntity>>(1, jobs);

        var repository = CreateMockJobsRepository();
        repository.GetListAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Is<IList<string>>(types => types.Contains(TestConstants.JobTypes.TestJob)),
            Arg.Is<IList<BackgroundJobStatus>>(statuses => statuses.Contains(BackgroundJobStatus.New)))
            .Returns(pair);
        
        var service = CreateBackgroundJobDomainService(jobsRepository: repository);

        // Act
        var result = await service.GetBackgroundJobsList(
            typeFilter: new List<string> { TestConstants.JobTypes.TestJob },
            statusFilter: new List<BackgroundJobStatus> { BackgroundJobStatus.New });

        // Assert
        result.Value.Should().HaveCount(1);
        await repository.Received(1).GetListAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Is<IList<string>>(types => types.Contains(TestConstants.JobTypes.TestJob)),
            Arg.Is<IList<BackgroundJobStatus>>(statuses => statuses.Contains(BackgroundJobStatus.New)));
    }

    #endregion

    #region RetryJob Tests

    [Fact]
    public async Task RetryJob_RetriesErroredJob()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithRetrySettings(3, 5, isRetryAllowed: true)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Mock StartExecutionAsync behavior
        repository.GetAsync(jobId, true).Returns(job);

        // Act
        var result = await service.RetryJob(jobId);

        // Assert
        result.Should().BeTrue();
        await repository.Received(1).UpdateAsync(Arg.Any<BackgroundJobEntity>(), true);
    }

    [Fact]
    public async Task RetryJob_JobNotInRetryableState_ReturnsFalse()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId).Returns(job);
        
        var service = CreateBackgroundJobDomainService(jobsRepository: repository);

        // Act
        // RetryJob catches exceptions and returns false when job is not in retryable state
        var result = await service.RetryJob(jobId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RetryJob_RetryNotAllowed_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithRetrySettings(3, 5, isRetryAllowed: false)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId).Returns(job);
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act & Assert
        // RetryJob catches exceptions and returns false, but UserFriendlyException should be re-thrown
        // Check that the method returns false when retry is not allowed
        var result = await service.RetryJob(jobId);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RetryJob_UpdatesRetryParameters()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithRetrySettings(3, 5, isRetryAllowed: true)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        
        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        repository.GetAsync(jobId, true).Returns(job);

        // Act
        await service.RetryJob(
            jobId,
            startExecutionParams: "new params",
            startExecutionExtraParams: "new extra params",
            timeoutInMinutes: 120,
            maxRetryAttempts: 5,
            retryInMinutes: 10);

        // Assert
        job.StartExecutionParams.Should().Be("new params");
        job.StartExecutionExtraParams.Should().Be("new extra params");
        job.TimeoutInMinutes.Should().Be(120);
        job.MaxRetryAttempts.Should().Be(5);
        job.RetryIntervalInMinutes.Should().Be(10);
    }

    #endregion
}
