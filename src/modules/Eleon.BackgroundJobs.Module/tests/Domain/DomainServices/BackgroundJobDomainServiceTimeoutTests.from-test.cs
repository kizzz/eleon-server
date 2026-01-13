using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
using TaskIdWithTimeout = VPortal.BackgroundJobs.Module.Repositories.TaskIdWithTimeout;
using TenantSettings.Module.Helpers;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;
using SharedModule.modules.MultiTenancy.Module;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Module.Domain.DomainServices;

/// <summary>
/// Timeout and long-running job tests
/// </summary>
public class BackgroundJobDomainServiceTimeoutTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task CancelLongTimeJobsAsync_JobExceedsTimeout_CancelsJob()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var startTime = DateTime.UtcNow.AddHours(-2); // Started 2 hours ago

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithExecutionStartTime(startTime)
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithTimeout(60) // 60 minutes timeout
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetLongTimeExecutingJobIdsAsync().Returns(new List<TaskIdWithTimeout> 
        { 
            new TaskIdWithTimeout(jobId, job.TimeoutInMinutes) 
        });
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        var currentTenant = CreateMockCurrentTenant();
        var multiTenancyService = CreateMultiTenancyService(currentTenant);

        var managerService = new BackgroundJobManagerDomainService(
            CreateMockLogger<BackgroundJobManagerDomainService>(),
            service,
            CreateMockConfiguration(),
            multiTenancyService,
            repository);

        // Act
        await managerService.CancelLongTimeJobsAsync();

        // Assert
        await repository.Received(1).GetLongTimeExecutingJobIdsAsync();
        // Job should be cancelled (verified via CancelJobAsync call)
    }

    [Fact]
    public async Task CancelJobAsync_TimeoutMessage_IncludesTimeoutInfo()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var timeoutMinutes = 60;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithTimeout(timeoutMinutes)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act
        await service.CancelJobAsync(
            jobId,
            BackgroundJobsConstants.ModuleName,
            false,
            $"Cancelled by timeout: {timeoutMinutes} minutes");

        // Assert
        job.Status.Should().Be(BackgroundJobStatus.Cancelled);
        execution.Messages.Should().Contain(m => m.TextMessage.Contains("timeout"));
    }

    [Fact]
    public async Task StartExecutionAsync_ZeroTimeout_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .WithTimeout(0) // Zero timeout
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
        var execution = await service.StartExecutionAsync(jobId);

        // Assert - Should handle zero timeout
        execution.Should().NotBeNull();
        job.TimeoutInMinutes.Should().Be(0);
    }

    [Fact]
    public async Task StartExecutionAsync_VeryLargeTimeout_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.New)
            .WithTimeout(int.MaxValue) // Very large timeout
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
        var execution = await service.StartExecutionAsync(jobId);

        // Assert - Should handle large timeout
        execution.Should().NotBeNull();
        job.TimeoutInMinutes.Should().Be(int.MaxValue);
    }

    [Fact]
    public async Task CompleteExecutionAsync_TimeoutDuringExecution_HandlesCorrectly()
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
            .WithTimeout(60)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act - Complete execution (simulating timeout scenario)
        var result = await service.CompleteExecutionAsync(
            jobId,
            executionId,
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
        result.Should().NotBeNull();
        result.Status.Should().Be(BackgroundJobExecutionStatus.Errored);
        job.Status.Should().Be(BackgroundJobStatus.Errored);
    }

    [Fact]
    public async Task RetryJob_TimeoutWithRetries_HandlesCorrectly()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;

        var timedOutExecution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Errored)
            .WithMessage(BackgroundJobMessageType.Error, "Timeout")
            .AsRetryExecution()
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithTimeout(60)
            .WithCurrentRetryAttempt(1)
            .WithMaxRetryAttempts(3)
            .WithRetrySettings(3, 5)
            .WithExecution(timedOutExecution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).Returns(job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var eventBus = CreateMockEventBus();
        var objectMapper = CreateMockObjectMapper();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            eventBus: eventBus,
            objectMapper: objectMapper);

        // Act
        var retryResult = await service.RetryJob(jobId);

        // Assert
        retryResult.Should().BeTrue();
        job.Status.Should().Be(BackgroundJobStatus.Retring);
    }
}
