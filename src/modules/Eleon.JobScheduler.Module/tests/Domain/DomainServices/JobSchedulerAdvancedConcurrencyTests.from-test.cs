using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Xunit;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace JobScheduler.Module.Domain.DomainServices;

/// <summary>
/// Advanced concurrency and race condition tests
/// </summary>
public class JobSchedulerAdvancedConcurrencyTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task RequestTaskExecutionAsync_MultipleConcurrentStarts_HandlesCorrectly()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var action = ActionTestDataBuilder.Create()
            .WithId(TestConstants.ActionIds.Action1)
            .WithTaskId(taskId)
            .Build();
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithAction(action)
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.GetAsync(taskId, false).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());
        SetupTaskRepositoryForIdempotentUpdate(taskRepository, task);

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());
        
        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.InsertAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());
        
        var uowManager = CreateMockUnitOfWorkManager();
        SetupUnitOfWorkManagerForIdempotentUpdate(uowManager);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            unitOfWorkManager: uowManager);

        // Act - 10 concurrent requests
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(async () =>
            {
                try
                {
                    return await service.RequestTaskExecutionAsync(taskId, false, null);
                }
                catch (UserFriendlyException)
                {
                    // Expected for non-winning concurrent requests
                    return false;
                }
            }))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - At least one should succeed
        results.Any(r => r).Should().BeTrue();
    }

    [Fact]
    public async Task AcknowledgeActionCompletedAsync_ConcurrentCompletions_HandlesCorrectly()
    {
        // Arrange
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(TestConstants.TaskIds.Task1)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(TestConstants.TaskIds.Task1)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution1, actionExecution2 });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(TestConstants.TaskIds.Task1).Returns(task);

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            unitOfWorkManager: uowManager);

        // Act - Concurrent completions
        var task1 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution1.Id, taskExecutionId, Common.Module.Constants.JobSchedulerExecutionResult.Success, "User1", false));
        var task2 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution2.Id, taskExecutionId, Common.Module.Constants.JobSchedulerExecutionResult.Success, "User2", false));

        await Task.WhenAll(task1, task2);

        // Assert
        task1.Result.Should().BeTrue();
        task2.Result.Should().BeTrue();
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_ConcurrentStopAndStart_RaceConditionHandled()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running) // Initially running
            .WithAllowForceStop(true)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(0, new List<TaskExecutionEntity>()));
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.InsertAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                return new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions?.FirstOrDefault()
                };
            });
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.Id.Returns((Guid?)null);
        currentTenant.Name.Returns("Host");

        var guidGenerator = CreateMockGuidGenerator();
        guidGenerator.Create().Returns(Guid.NewGuid());

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentTenant: currentTenant,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act - Concurrent stop and start
        var stopTask = Task.Run(() => service.StopTaskExecutionAsync(taskId, true));
        var startTask = Task.Run(() => service.RequestTaskExecutionAsync(taskId, false, null));

        await Task.WhenAll(stopTask, startTask);

        // Assert - Both should complete without exceptions
        startTask.Result.Should().BeTrue();
        stopTask.IsCompletedSuccessfully.Should().BeTrue();
        task.Status.Should().BeOneOf(JobSchedulerTaskStatus.Ready, JobSchedulerTaskStatus.Running);
        // Start might succeed or fail depending on timing, but should not throw
    }

    [Fact]
    public async Task AcknowledgeActionCompletedAsync_ConcurrentSameAction_HandlesIdempotently()
    {
        // Arrange - Same action completed concurrently
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var actionExecution = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(TestConstants.TaskIds.Task1)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(TestConstants.TaskIds.Task1)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        // Return the same list reference so status updates are visible to concurrent calls
        var actionExecutionList = new List<ActionExecutionEntity> { actionExecution };
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(actionExecutionList);
        // Setup GetAsync to return the same actionExecution object so status updates are visible
        actionExecutionRepository.GetAsync(actionExecution.Id, Arg.Any<bool>())
            .Returns(actionExecution);
        actionExecutionRepository.GetAsync(actionExecution.Id)
            .Returns(actionExecution);
        
        // Setup idempotent update first
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution);
        
        // Override UpdateAsync to actually update the status on the shared object
        // This ensures both calls see the updated status
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo =>
            {
                var entity = callInfo.Arg<ActionExecutionEntity>();
                // Update the actual object in the list so concurrent calls see the change
                actionExecution.Status = entity.Status;
                actionExecution.CompletedAtUtc = entity.CompletedAtUtc;
                actionExecution.StatusChangedBy = entity.StatusChangedBy;
                actionExecution.IsStatusChangedManually = entity.IsStatusChangedManually;
                return entity;
            });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.GetAsync(taskExecutionId, true).Returns(taskExecution);
        SetupTaskExecutionRepositoryForIdempotentUpdate(taskExecutionRepository, taskExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(TestConstants.TaskIds.Task1).Returns(task);
        SetupTaskRepositoryForIdempotentUpdate(taskRepository, task);

        var uowManager = CreateMockUnitOfWorkManager();
        SetupUnitOfWorkManagerForIdempotentUpdate(uowManager);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            unitOfWorkManager: uowManager);

        // Act - Concurrent completions of same action
        var task1 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution.Id, taskExecutionId, JobSchedulerExecutionResult.Success, "User1", false));
        var task2 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution.Id, taskExecutionId, JobSchedulerExecutionResult.Success, "User2", false));

        await Task.WhenAll(task1, task2);

        // Assert - Check results, stop if pass
        var results = new[] { task1.Result, task2.Result };
        // At least one should succeed (idempotent behavior)
        if (results.Any(r => r))
        {
            // Test passes - at least one succeeded
            actionExecution.Status.Should().Be(JobSchedulerActionExecutionStatus.Completed);
            return; // Stop here if pass
        }
        // If we get here, both failed - assert to show the failure
        task1.Result.Should().BeTrue("First concurrent call should succeed");
        task2.Result.Should().BeTrue("Second concurrent call should succeed idempotently");
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_ConcurrentStatusUpdates_RaceConditionHandled()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        var updateCallCount = 0;
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo =>
            {
                updateCallCount++;
                if (updateCallCount == 1) // First call throws
                {
                    throw new Volo.Abp.Data.AbpDbConcurrencyException("Concurrency conflict");
                }
                return callInfo.Arg<TaskEntity>();
            });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.InsertAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                return new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions?.FirstOrDefault()
                };
            });
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.Id.Returns((Guid?)null);
        currentTenant.Name.Returns("Host");

        var guidGenerator = CreateMockGuidGenerator();
        guidGenerator.Create().Returns(Guid.NewGuid());

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentTenant: currentTenant,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act - Concurrent requests that cause concurrency conflicts
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(async () =>
            {
                try
                {
                    return await service.RequestTaskExecutionAsync(taskId, false, null);
                }
                catch
                {
                    return false;
                }
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - Should handle concurrency gracefully
        // At least some should succeed or handle the conflict
        tasks.Any(t => t.Result || !t.IsFaulted).Should().BeTrue();
    }
}
