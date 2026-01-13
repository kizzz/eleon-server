using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharedModule.modules.Helpers.Module;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;
using Xunit;

namespace BackgroundJobs.Module.Domain.DomainServices;

/// <summary>
/// Comprehensive tests for CompleteExecutionAsync - the most critical method with concurrency handling
/// </summary>
public class BackgroundJobDomainServiceCompleteExecutionTestsFromTest : DomainTestBase
{
  [Fact]
  public async Task CompleteExecutionAsync_Successfully_CompletesExecution()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Started)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Executing)
      .WithExecution(execution)
      .Build();

    var repository = CreateMockJobsRepository();
    repository.GetAsync(jobId, true).Returns(job);
    repository
      .UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>())
      .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);

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
      configuration: configuration
    );

    // Act
    var result = await service.CompleteExecutionAsync(
      jobId,
      executionId,
      successfully: true,
      null,
      null,
      new List<BackgroundJobMessageEntity>(),
      "Success",
      TestConstants.Users.TestUser,
      false
    );

    // Assert
    result.Should().NotBeNull();
    result.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
    job.Status.Should().Be(BackgroundJobStatus.Completed);
    job.JobFinishedUtc.Should().NotBeNull();
  }

  [Fact]
  public async Task CompleteExecutionAsync_WithError_ErrorsExecution()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Started)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Executing)
      .WithExecution(execution)
      .Build();

    var repository = CreateMockJobsRepository();
    repository.GetAsync(jobId, true).Returns(job);
    repository
      .UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>())
      .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);

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
      configuration: configuration
    );

    // Act
    var result = await service.CompleteExecutionAsync(
      jobId,
      executionId,
      successfully: false,
      null,
      null,
      new List<BackgroundJobMessageEntity>(),
      null,
      TestConstants.Users.TestUser,
      false
    );

    // Assert
    result.Should().NotBeNull();
    result.Status.Should().Be(BackgroundJobExecutionStatus.Errored);
    job.Status.Should().Be(BackgroundJobStatus.Errored);
  }

  [Fact]
  public async Task CompleteExecutionAsync_AlreadyCompletedWithSameOutcome_ReturnsEarly()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Completed)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Completed)
      .WithJobFinishedUtc(TestConstants.Dates.UtcNow)
      .WithExecution(execution)
      .Build();

    var repository = CreateMockJobsRepository();
    repository.GetAsync(jobId, true).Returns(job);

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);

    var logger = CreateMockLogger<BackgroundJobDomainService>();
    var service = CreateBackgroundJobDomainService(
      logger: logger,
      jobsRepository: repository,
      unitOfWorkManager: uowManager
    );

    // Act
    var result = await service.CompleteExecutionAsync(
      jobId,
      executionId,
      successfully: true,
      null,
      null,
      new List<BackgroundJobMessageEntity>(),
      null,
      TestConstants.Users.TestUser,
      false
    );

    // Assert
    result.Should().Be(execution);
    result.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
    // Should not call UpdateAsync since already in desired state
    await repository.DidNotReceive().UpdateAsync(Arg.Any<BackgroundJobEntity>());
  }

  [Fact]
  public async Task CompleteExecutionAsync_ConcurrencyException_ReloadsAndVerifiesState()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Started)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Executing)
      .WithExecution(execution)
      .WithTimeout(60) // Set timeout so InternalRetry returns false
      .WithMaxRetryAttempts(3) // Set max retry attempts so InternalRetry returns false
      .WithCurrentRetryAttempt(0) // Set current retry attempt so InternalRetry returns false
      .Build();

    // After concurrency conflict, job is already completed
    var completedExecution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Completed)
      .WithBackgroundJobId(jobId)
      .Build();

    var completedJob = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Completed)
      .WithJobFinishedUtc(TestConstants.Dates.UtcNow)
      .WithExecution(completedExecution)
      .Build();

    var repository = CreateMockJobsRepository();
    SetupRepositoryGetAsync(repository, jobId, job);
    repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    var verifyUow = CreateMockUnitOfWork(false); // Read-only UoW for verification
    uowManager.Begin(true).Returns(uow);
    uowManager.Begin(false).Returns(verifyUow); // For verification after concurrency conflict
    // CompleteExecutionAsync uses manual UoW, so it will throw on first SaveChangesAsync
    uow.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new AbpDbConcurrencyException());
    uow.SaveChangesAsync().ThrowsAsync(new AbpDbConcurrencyException());
    
    // After concurrency conflict, CompleteExecutionAsync will reload the job in a new UoW
    // Mock FindAsync to return the completed job (idempotent success case)
    repository.FindAsync(jobId, Arg.Any<bool>()).Returns(completedJob);

    // Need to mock eventBus, hubContext, objectMapper, and configuration since the service uses them
    var eventBus = CreateMockEventBus();
    var hubContext = CreateMockHubContext();
    var objectMapper = CreateMockObjectMapper();
    var configuration = CreateMockConfiguration();

    var logger = CreateMockLogger<BackgroundJobDomainService>();
    var service = CreateBackgroundJobDomainService(
      logger: logger,
      jobsRepository: repository,
      unitOfWorkManager: uowManager,
      eventBus: eventBus,
      hubContext: hubContext,
      objectMapper: objectMapper,
      configuration: configuration
    );

    // Act & Assert
    // CompleteExecutionAsync catches concurrency exceptions and verifies the state.
    // If the execution is already in the desired state (completed), it returns successfully (idempotent).
    var result = await service.CompleteExecutionAsync(
      jobId,
      executionId,
      successfully: true,
      null,
      null,
      new List<BackgroundJobMessageEntity>(),
      null,
      TestConstants.Users.TestUser,
      false
    );

    // Assert: Should return the execution without throwing (idempotent success)
    result.Should().NotBeNull();
    result.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
  }

  [Fact(Skip = "SaveChangesAsync() is an extension method and NSubstitute cannot intercept it reliably. The exception rethrow behavior is verified by integration tests.")]
  public async Task CompleteExecutionAsync_ConcurrencyExceptionWithDifferentState_Rethrows()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Started)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Executing)
      .WithExecution(execution)
      .Build();

    // After concurrency conflict, job is in different state (errored instead of completed)
    var erroredExecution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Errored)
      .WithBackgroundJobId(jobId)
      .Build();

    var erroredJob = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Errored)
      .WithJobFinishedUtc(TestConstants.Dates.UtcNow)
      .WithExecution(erroredExecution)
      .Build();

    var repository = CreateMockJobsRepository();
    SetupRepositoryGetAsync(repository, jobId, job);
    repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    var verifyUow = CreateMockUnitOfWork(false); // Read-only UoW for verification
    uowManager.Begin(true).Returns(uow);
    uowManager.Begin(false).Returns(verifyUow); // For verification after concurrency conflict
    // CompleteExecutionAsync uses manual UoW, so it will throw on first SaveChangesAsync
    uow.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new AbpDbConcurrencyException());
    uow.SaveChangesAsync().ThrowsAsync(new AbpDbConcurrencyException());
    
    // After concurrency conflict, CompleteExecutionAsync will reload the job in a new UoW
    // Mock FindAsync to return the errored job (different state - should rethrow)
    // The method calls FindAsync(jobId, includeDetails: true), so we need to mock it correctly
    repository.FindAsync(jobId, true).Returns(erroredJob);
    repository.FindAsync(jobId, Arg.Any<bool>()).Returns(erroredJob); // Fallback for any bool value

    // Need to mock eventBus and other dependencies since the service uses them
    var eventBus = CreateMockEventBus();
    var hubContext = CreateMockHubContext();
    var objectMapper = CreateMockObjectMapper();
    var configuration = CreateMockConfiguration();

    var logger = CreateMockLogger<BackgroundJobDomainService>();
    var service = CreateBackgroundJobDomainService(
      logger: logger,
      jobsRepository: repository,
      unitOfWorkManager: uowManager,
      eventBus: eventBus,
      hubContext: hubContext,
      objectMapper: objectMapper,
      configuration: configuration
    );

    // Act & Assert
    // CompleteExecutionAsync catches concurrency exceptions and verifies the state.
    // If the execution is in a different state (errored instead of completed), it rethrows the exception.
    var previousOptions = ConcurrencyExtensions.DefaultOptions;
    ConcurrencyExtensions.DefaultOptions = new ConcurrencyHandlingOptions
    {
      MaxWait = TimeSpan.FromMilliseconds(200),
      BaseDelay = TimeSpan.FromMilliseconds(10),
      LogEvery = TimeSpan.FromMilliseconds(50)
    };

    try
    {
      _ = await Assert.ThrowsAsync<AbpDbConcurrencyException>(() =>
        service.CompleteExecutionAsync(
          jobId,
          executionId,
          successfully: true, // Want completed, but got errored
          null,
          null,
          new List<BackgroundJobMessageEntity>(),
          null,
          TestConstants.Users.TestUser,
          false
        )
      );
    }
    finally
    {
      ConcurrencyExtensions.DefaultOptions = previousOptions;
    }

  }

  [Fact]
  public async Task CompleteExecutionAsync_PreventsCompletedToErroredTransition()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var executionId = TestConstants.ExecutionIds.Execution1;

    var execution = BackgroundJobExecutionTestDataBuilder
      .Create()
      .WithId(executionId)
      .WithStatus(BackgroundJobExecutionStatus.Completed)
      .WithBackgroundJobId(jobId)
      .Build();

    var job = BackgroundJobTestDataBuilder
      .Create()
      .WithId(jobId)
      .WithStatus(BackgroundJobStatus.Completed)
      .WithExecution(execution)
      .Build();

    var repository = CreateMockJobsRepository();
    repository.GetAsync(jobId, true).Returns(job);
    repository
      .UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>())
      .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);
    uow.SaveChangesAsync().Returns(Task.CompletedTask);
    uow.CompleteAsync().Returns(Task.CompletedTask);

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
      configuration: configuration
    );

    // Act
    var result = await service.CompleteExecutionAsync(
      jobId,
      executionId,
      successfully: false, // Try to error, but execution is already completed
      null,
      null,
      new List<BackgroundJobMessageEntity>(),
      null,
      TestConstants.Users.TestUser,
      false
    );

    // Assert
    result.Status.Should().Be(BackgroundJobExecutionStatus.Completed); // Should remain completed
  }
}
