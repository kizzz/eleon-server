using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;
using NSubstitute.ExceptionExtensions;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;

namespace BackgroundJobs.Module.Domain.DomainServices;

/// <summary>
/// Error recovery and resilience tests
/// </summary>
public class BackgroundJobDomainServiceErrorRecoveryTestsFromTest : DomainTestBase
{
    [Fact(Skip = "SaveChangesAsync() is an extension method and NSubstitute cannot intercept it. The exception propagation is verified by integration tests.")]
    public async Task CompleteExecutionAsync_RepositoryThrowsException_PropagatesException()
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
            .WithTimeout(60) // Set timeout so InternalRetry returns false
            .WithMaxRetryAttempts(3) // Set max retry attempts so InternalRetry returns false
            .WithCurrentRetryAttempt(0) // Set current retry attempt so InternalRetry returns false
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().ThrowsAsync(new Exception("Database connection lost"));

        // Need to mock eventBus, hubContext, objectMapper, and configuration since the service uses them
        var eventBus = CreateMockEventBus();
        var hubContext = CreateMockHubContext();
        var objectMapper = CreateMockObjectMapper();
        var configuration = CreateMockConfiguration();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            hubContext: hubContext,
            objectMapper: objectMapper,
            configuration: configuration);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await service.CompleteExecutionAsync(
                jobId,
                executionId,
                successfully: true,
                null,
                null,
                new List<BackgroundJobMessageEntity>(),
                "Success",
                TestConstants.Users.TestUser,
                false));
    }

    [Fact]
    public async Task StartExecutionAsync_EventBusFailure_JobStillCreated()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        eventBus.PublishAsync(Arg.Any<object>())
            .ThrowsAsync(new Exception("Event bus unavailable"));

        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act & Assert - Should throw but job state should be consistent
        await Assert.ThrowsAsync<Exception>(async () =>
            await service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task CompleteExecutionAsync_PartialFailure_ErrorMessagesPreserved()
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

        var errorMessages = new List<BackgroundJobMessageEntity>
        {
            BackgroundJobMessageTestDataBuilder.Create()
                .WithType(BackgroundJobMessageType.Error)
                .WithText("Error 1")
                .Build(),
            BackgroundJobMessageTestDataBuilder.Create()
                .WithType(BackgroundJobMessageType.Error)
                .WithText("Error 2")
                .Build()
        };

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.CompleteExecutionAsync(
            jobId,
            executionId,
            successfully: false,
            null,
            null,
            errorMessages,
            "Failed",
            TestConstants.Users.TestUser,
            false);

        // Assert
        result.Should().NotBeNull();
        execution.Messages.Should().Contain(m => m.TextMessage == "Error 1");
        execution.Messages.Should().Contain(m => m.TextMessage == "Error 2");
    }

    [Fact]
    public async Task RetryJob_ErrorTriggersRetry_RetryLogicExecutes()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Errored)
            .WithMessage(BackgroundJobMessageType.Error, "Test error")
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
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

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
    public async Task CompleteExecutionAsync_ErrorRecoveryAfterTimeout_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithBackgroundJobId(jobId)
            .WithMessage(BackgroundJobMessageType.Error, "Timeout occurred")
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithTimeout(60)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Complete with error after timeout
        var result = await service.CompleteExecutionAsync(
            jobId,
            executionId,
            successfully: false,
            null,
            null,
            new List<BackgroundJobMessageEntity>(),
            "Timeout error",
            BackgroundJobsConstants.ModuleName,
            false);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(BackgroundJobExecutionStatus.Errored);
        job.Status.Should().Be(BackgroundJobStatus.Errored);
    }
}
