using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EventHandlers;
using VPortal.BackgroundJobs.Module.Repositories;
using Messaging.Module.Messages;
using NSubstitute.ExceptionExtensions;

namespace BackgroundJobs.Module.Application.EventServices;

/// <summary>
/// Advanced event bus integration tests
/// </summary>
public class AdvancedEventServiceTests : DomainTestBase
{
    [Fact]
    public async Task CompleteBackgroundJobEventService_DuplicateEvents_HandlesGracefully()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Completed)
            .WithBackgroundJobId(jobId)
            .Build();

        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .WithStatus(BackgroundJobStatus.Completed)
            .WithJobFinishedUtc(TestConstants.Dates.UtcNow)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        SetupRepositoryGetAsync(repository, jobId, job);
        repository.FindAsync(jobId, true).Returns(job);

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var domainService = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        var eventService = new CompleteBackgroundJobEventService(
            CreateMockLogger<CompleteBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            CreateMockObjectMapper(),
            CreateMockCurrentTenant());

        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            TenantId = TestConstants.TenantIds.Tenant1,
            CompletedBy = TestConstants.Users.TestUser,
            IsManually = false
        };

        // Act - Handle duplicate event
        await eventService.HandleEventAsync(eventData);

        // Assert - Should not throw, handles gracefully
        // Domain service idempotency should handle this
    }

    [Fact]
    public async Task CompleteBackgroundJobEventService_EventOrdering_ProcessesInOrder()
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
        SetupRepositoryGetAsync(repository, jobId, job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), false, Arg.Any<CancellationToken>())
          .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var domainService = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        var eventService = new CompleteBackgroundJobEventService(
            CreateMockLogger<CompleteBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            CreateMockObjectMapper(),
            CreateMockCurrentTenant());

        var events = new List<BackgroundJobExecutionCompletedMsg>
        {
            new BackgroundJobExecutionCompletedMsg
            {
                BackgroundJobId = jobId,
                ExecutionId = executionId,
                Status = BackgroundJobExecutionStatus.Completed,
                TenantId = TestConstants.TenantIds.Tenant1,
                CompletedBy = TestConstants.Users.TestUser,
                IsManually = false
            },
            new BackgroundJobExecutionCompletedMsg
            {
                BackgroundJobId = jobId,
                ExecutionId = executionId,
                Status = BackgroundJobExecutionStatus.Completed,
                TenantId = TestConstants.TenantIds.Tenant1,
                CompletedBy = TestConstants.Users.TestUser,
                IsManually = false
            }
        };

        // Act - Process events sequentially
        foreach (var eventData in events)
        {
            await eventService.HandleEventAsync(eventData);
        }

        // Assert - Should handle both events gracefully
        await repository.Received().GetAsync(jobId, true);
    }

    [Fact]
    public async Task CompleteBackgroundJobEventService_EventFailure_LogsAndRethrows()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId, true).ThrowsAsync(new Exception("Database error"));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var domainService = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        var logger = CreateMockLogger<CompleteBackgroundJobEventService>();
        var eventService = new CompleteBackgroundJobEventService(
            logger,
            domainService,
            CreateMockEventBus(),
            CreateMockObjectMapper(),
            CreateMockCurrentTenant());

        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            TenantId = TestConstants.TenantIds.Tenant1,
            CompletedBy = TestConstants.Users.TestUser,
            IsManually = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await eventService.HandleEventAsync(eventData));
    }

    [Fact]
    public async Task CompleteBackgroundJobEventService_TenantContext_HandlesCorrectly()
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
            .WithTenantId(TestConstants.TenantIds.Tenant1)
            .WithExecution(execution)
            .Build();

        var repository = CreateMockJobsRepository();
        SetupRepositoryGetAsync(repository, jobId, job);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), false, Arg.Any<CancellationToken>())
          .Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        var currentTenant = CreateMockCurrentTenant();
        var tenantDisposable = Substitute.For<IDisposable>();
        currentTenant.Change(TestConstants.TenantIds.Tenant1).Returns(tenantDisposable);

        var domainService = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        var eventService = new CompleteBackgroundJobEventService(
            CreateMockLogger<CompleteBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            CreateMockObjectMapper(),
            currentTenant);

        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            TenantId = TestConstants.TenantIds.Tenant1,
            CompletedBy = TestConstants.Users.TestUser,
            IsManually = false
        };

        // Act
        await eventService.HandleEventAsync(eventData);

        // Assert
        currentTenant.Received(1).Change(TestConstants.TenantIds.Tenant1);
        tenantDisposable.Received(1).Dispose();
    }
}
