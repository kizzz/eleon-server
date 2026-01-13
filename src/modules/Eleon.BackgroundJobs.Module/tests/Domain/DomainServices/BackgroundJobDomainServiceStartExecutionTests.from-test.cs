using System;
using System.Collections.Generic;
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

public class BackgroundJobDomainServiceStartExecutionTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task StartExecutionAsync_CreatesExecutionWithStartingStatus()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
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
        var result = await service.StartExecutionAsync(jobId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(BackgroundJobExecutionStatus.Starting);
        result.BackgroundJobEntityId.Should().Be(jobId);
        job.Status.Should().Be(BackgroundJobStatus.Executing);
        job.Executions.Should().Contain(result);
    }

    [Fact]
    public async Task StartExecutionAsync_WithManualRetry_UpdatesStatusToRetring()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithRetrySettings(3, 5, isRetryAllowed: true)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
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
        var result = await service.StartExecutionAsync(jobId, isManualRetry: true);

        // Assert
        result.Should().NotBeNull();
        job.Status.Should().Be(BackgroundJobStatus.Retring);
        job.CurrentRetryAttempt.Should().Be(0); // Reset on manual retry
    }

    [Fact]
    public async Task StartExecutionAsync_WithAutoRetry_IncrementsRetryAttempt()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithCurrentRetryAttempt(1)
            .WithRetrySettings(3, 5)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
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
        var result = await service.StartExecutionAsync(jobId, autoRetry: true);

        // Assert
        result.Should().NotBeNull();
        job.Status.Should().Be(BackgroundJobStatus.Retring);
        job.CurrentRetryAttempt.Should().Be(2); // Incremented
    }

    [Fact]
    public async Task StartExecutionAsync_ManualRetryNotAllowed_ThrowsException()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithRetrySettings(3, 5, isRetryAllowed: false)
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
            service.StartExecutionAsync(jobId, isManualRetry: true));
    }

    [Fact]
    public async Task StartExecutionAsync_JobAlreadyCompleted_ThrowsException()
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
            service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task StartExecutionAsync_JobAlreadyExecuting_ThrowsException()
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
        
        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(() => 
            service.StartExecutionAsync(jobId));
    }

    [Fact]
    public async Task StartExecutionAsync_SystemInternalJob_PublishesInternalSystemJobMessage()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .AsSystemInternal()
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
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
        await service.StartExecutionAsync(jobId);

        // Assert
        await eventBus.Received(1).PublishAsync(
            Arg.Is<object>(msg => msg.GetType().Name.Contains("StartingInternalSystemJobExecutionMsg")),
            Arg.Any<bool>());
    }

    [Fact]
    public async Task StartExecutionAsync_RegularJob_PublishesRegularJobMessage()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        
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
        await service.StartExecutionAsync(jobId);

        // Assert
        await eventBus.Received(1).PublishAsync(
            Arg.Is<object>(msg => msg.GetType().Name.Contains("StartingJobExecutionMsg")),
            Arg.Any<bool>());
    }
}

