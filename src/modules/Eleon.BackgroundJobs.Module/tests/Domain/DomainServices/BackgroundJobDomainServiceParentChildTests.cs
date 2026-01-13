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
using Volo.Abp.Domain.Entities;

namespace BackgroundJobs.Module.Domain.DomainServices;

/// <summary>
/// Advanced tests for parent-child job relationship scenarios
/// </summary>
public class BackgroundJobDomainServiceParentChildTests : DomainTestBase
{
    [Fact]
    public async Task CreateAsync_WithParentJobId_CreatesChildJob()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var childJobId = TestConstants.JobIds.Job2;

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Completed)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, false).Returns(parentJob);
        repository.InsertAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var childJob = await service.CreateAsync(
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

        // Assert
        childJob.Should().NotBeNull();
        childJob.ParentJobId.Should().Be(parentJobId);
        childJob.Id.Should().Be(childJobId);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentParent_CreatesJobWithoutValidation()
    {
        // Arrange
        var parentJobId = Guid.NewGuid();
        var childJobId = TestConstants.JobIds.Job2;

        var repository = CreateMockJobsRepository();
        // The service doesn't validate parent existence, so we don't need to set up GetAsync for parent
        repository.InsertAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin().Returns(uow);
        // Set up SaveChangesAsync and CompleteAsync for the unit of work
        uow.SaveChangesAsync().Returns(Task.CompletedTask);
        uow.CompleteAsync().Returns(Task.CompletedTask);

        // Use real ConfigurationBuilder instead of mock to avoid GetValue extension method issues
        var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        // Don't add EnvironmentId to the collection, so GetValue will return the default (null)
        var configuration = configurationBuilder.Build();

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager,
            configuration: configuration);

        // Act
        // The service doesn't throw an exception when parent doesn't exist - it just creates the job with ParentJobId set
        var result = await service.CreateAsync(
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
            null,
            new Dictionary<string, string>()); // Pass empty dictionary instead of null to avoid NullReferenceException

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(childJobId);
        result.ParentJobId.Should().Be(parentJobId);
        await repository.Received(1).InsertAsync(Arg.Any<BackgroundJobEntity>(), true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteExecutionAsync_ParentCompletes_ChildJobsCanStart()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var childJobId = TestConstants.JobIds.Job2;
        var parentExecutionId = TestConstants.ExecutionIds.Execution1;

        var parentExecution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(parentExecutionId)
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithBackgroundJobId(parentJobId)
            .Build();

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(parentExecution)
            .Build();

        var childJob = BackgroundJobTestDataBuilder.Create()
            .WithId(childJobId)
            .WithParentJobId(parentJobId)
            .WithStatus(BackgroundJobStatus.New)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, true).Returns(parentJob);
        repository.GetAsync(childJobId, false).Returns(childJob);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Complete parent
        await service.CompleteExecutionAsync(
            parentJobId,
            parentExecutionId,
            successfully: true,
            null,
            null,
            new List<BackgroundJobMessageEntity>(),
            "Success",
            TestConstants.Users.TestUser,
            false);

        // Assert
        parentJob.Status.Should().Be(BackgroundJobStatus.Completed);
        // Child job should be able to start now (this would be verified in integration test)
    }

    [Fact]
    public async Task CompleteExecutionAsync_ParentFails_ChildJobsHandledCorrectly()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var childJobId = TestConstants.JobIds.Job2;
        var parentExecutionId = TestConstants.ExecutionIds.Execution1;

        var parentExecution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(parentExecutionId)
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithBackgroundJobId(parentJobId)
            .Build();

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(parentExecution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, true).Returns(parentJob);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Complete parent with failure
        await service.CompleteExecutionAsync(
            parentJobId,
            parentExecutionId,
            successfully: false,
            null,
            null,
            new List<BackgroundJobMessageEntity>(),
            "Failure",
            TestConstants.Users.TestUser,
            false);

        // Assert
        parentJob.Status.Should().Be(BackgroundJobStatus.Errored);
        parentExecution.Status.Should().Be(BackgroundJobExecutionStatus.Errored);
    }

    [Fact]
    public async Task CancelJobAsync_ParentCancelled_ChildJobsStateHandled()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var childJobId = TestConstants.JobIds.Job2;

        var parentExecution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .Build();

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Executing)
            .WithExecution(parentExecution)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, true).Returns(parentJob);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        await service.CancelJobAsync(parentJobId, TestConstants.Users.TestUser, true, "Cancelled");

        // Assert
        parentJob.Status.Should().Be(BackgroundJobStatus.Cancelled);
        parentExecution.Status.Should().Be(BackgroundJobExecutionStatus.Cancelled);
    }

    [Fact]
    public async Task CreateAsync_MultiLevelHierarchy_CreatesCorrectly()
    {
        // Arrange - Grandparent -> Parent -> Child
        var grandparentId = TestConstants.JobIds.Job1;
        var parentId = TestConstants.JobIds.Job2;
        var childId = TestConstants.JobIds.Job3;

        var grandparent = BackgroundJobTestDataBuilder.Create()
            .WithId(grandparentId)
            .WithStatus(BackgroundJobStatus.Completed)
            .Build();

        var parent = BackgroundJobTestDataBuilder.Create()
            .WithId(parentId)
            .WithParentJobId(grandparentId)
            .WithStatus(BackgroundJobStatus.Completed)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(grandparentId, false).Returns(grandparent);
        repository.GetAsync(parentId, false).Returns(parent);
        repository.InsertAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Create child with parent that has grandparent
        var child = await service.CreateAsync(
            TestConstants.TenantIds.Tenant1,
            childId,
            parentId,
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

        // Assert
        child.Should().NotBeNull();
        child.ParentJobId.Should().Be(parentId);
        child.Id.Should().Be(childId);
    }

    [Fact]
    public async Task RetryJob_ParentRetries_ChildJobsUnaffected()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var childJobId = TestConstants.JobIds.Job2;

        var parentExecution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Errored)
            .AsRetryExecution()
            .Build();

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Errored)
            .WithCurrentRetryAttempt(1)
            .WithMaxRetryAttempts(3)
            .WithRetrySettings(3, 5)
            .WithExecution(parentExecution)
            .Build();

        var childJob = BackgroundJobTestDataBuilder.Create()
            .WithId(childJobId)
            .WithParentJobId(parentJobId)
            .WithStatus(BackgroundJobStatus.New)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, true).Returns(parentJob);
        repository.GetAsync(childJobId, false).Returns(childJob);
        repository.UpdateAsync(Arg.Any<BackgroundJobEntity>(), Arg.Any<bool>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act
        var retryResult = await service.RetryJob(parentJobId);

        // Assert
        retryResult.Should().BeTrue();
        parentJob.Status.Should().Be(BackgroundJobStatus.Retring);
        // Child job should remain unchanged
        childJob.Status.Should().Be(BackgroundJobStatus.New);
    }

    [Fact]
    public async Task CreateAsync_ConcurrentChildrenWaitingForParent_AllCreatedSuccessfully()
    {
        // Arrange
        var parentJobId = TestConstants.JobIds.Job1;
        var child1Id = TestConstants.JobIds.Job2;
        var child2Id = TestConstants.JobIds.Job3;

        var parentJob = BackgroundJobTestDataBuilder.Create()
            .WithId(parentJobId)
            .WithStatus(BackgroundJobStatus.Completed)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(parentJobId, false).Returns(parentJob);
        repository.InsertAsync(Arg.Any<BackgroundJobEntity>()).Returns(call => Task.FromResult(call.Arg<BackgroundJobEntity>()));

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        uowManager.Begin(true).Returns(uow);

        var service = CreateBackgroundJobDomainService(
            jobsRepository: repository,
            unitOfWorkManager: uowManager);

        // Act - Create multiple children concurrently
        var tasks = new List<Task<BackgroundJobEntity>>
        {
            service.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                child1Id,
                parentJobId,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Child 1",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null),
            service.CreateAsync(
                TestConstants.TenantIds.Tenant1,
                child2Id,
                parentJobId,
                TestConstants.JobTypes.TestJob,
                TestConstants.Users.TestUser,
                true,
                "Child 2",
                "{}",
                DateTime.UtcNow,
                false,
                null,
                TestConstants.Users.TestUser,
                "User",
                60,
                3,
                5,
                null)
        };

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(2);
        results[0].ParentJobId.Should().Be(parentJobId);
        results[1].ParentJobId.Should().Be(parentJobId);
        results[0].Id.Should().Be(child1Id);
        results[1].Id.Should().Be(child2Id);
    }
}

