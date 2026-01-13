using System;
using System.Collections.Generic;
using System.Linq;
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
/// Complex state transition tests
/// </summary>
public class BackgroundJobDomainServiceStateTransitionTests : DomainTestBase
{
    [Fact]
    public async Task StartExecutionAsync_FromCompletedStatus_ThrowsException()
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

        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task StartExecutionAsync_FromCancelledStatus_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Cancelled)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);

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

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task StartExecutionAsync_AlreadyExecuting_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Starting)
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);

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

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task CancelJobAsync_FromCompletedStatus_ThrowsException()
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
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.CancelJobAsync(jobId, TestConstants.Users.TestUser, true, "Test"));
    }

    [Fact]
    public async Task CompleteExecutionAsync_StateTransitionDuringOperation_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithBackgroundJobId(jobId)
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.FindAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Complete while status might be changing
        var result = await service.CompleteExecutionAsync(
            jobId,
            executionId,
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
    }

    [Fact]
    public async Task RetryJob_StateTransitionDuringRetry_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Errored)
            .AsRetryExecution()
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithCurrentRetryAttempt(1)
            .WithMaxRetryAttempts(3)
            .WithRetrySettings(3, 5)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var retryResult = await service.RetryJob(jobId);

        // Assert
        retryResult.Should().BeTrue();
        job.Status.Should().Be(BackgroundJobStatus.Retring);
    }

    [Fact]
    public async Task MarkExecutionStartedAsync_InvalidStateTransition_HandlesGracefully()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Completed) // Already completed
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(false);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.MarkExecutionStartedAsync(jobId, executionId);

        // Assert - Should handle gracefully, may add error message
        result.Should().NotBeNull();
        execution.Messages.Should().Contain(m => m.MessageType == BackgroundJobMessageType.Error);
    }
}
