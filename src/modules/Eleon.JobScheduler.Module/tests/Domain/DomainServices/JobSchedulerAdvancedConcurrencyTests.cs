using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
public class JobSchedulerAdvancedConcurrencyTests : DomainTestBase
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
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution1);
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution2);

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

        // Act - Concurrent completions
        var task1 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution1.Id, taskExecutionId, Common.Module.Constants.JobSchedulerExecutionResult.Success, "User1", false));
        var task2 = Task.Run(() => service.AcknowledgeActionCompletedAsync(
            actionExecution2.Id, taskExecutionId, Common.Module.Constants.JobSchedulerExecutionResult.Success, "User2", false));

        var results = await Task.WhenAll(task1, task2);

        // Assert
        results[0].Should().BeTrue();
        results[1].Should().BeTrue();
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
        taskRepository.GetAsync(taskId, false).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());
        SetupTaskRepositoryForIdempotentUpdate(taskRepository, task);

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
        SetupUnitOfWorkManagerForIdempotentUpdate(uowManager);

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
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution });
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution);

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

        var results = await Task.WhenAll(task1, task2);

        // Assert - At least one should succeed idempotently
        (results[0] || results[1]).Should().BeTrue();
        // Action should be completed (idempotent)
        actionExecution.Status.Should().Be(JobSchedulerActionExecutionStatus.Completed);
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
        SetupUnitOfWorkManagerForIdempotentUpdate(uowManager);

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

        var results = await Task.WhenAll(tasks);

        // Assert - Should handle concurrency gracefully
        // At least some should succeed or handle the conflict
        results.Any(r => r).Should().BeTrue();
    }

    [Fact]
    public async Task AcknowledgeActionCompletedAsync_ConcurrencyException_ActionCompletedButTaskExecutionNotUpdated_UpdatesTaskExecutionAndTask()
    {
        // Arrange - This tests the new fix: action execution is completed but task execution and task are not updated
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var actionExecutionId = TestConstants.ActionExecutionIds.ActionExecution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(actionExecutionId)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed) // Already completed
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing) // BUG: Still Executing even though all actions are done
            .WithFinishedAt(null) // Also missing FinishedAtUtc
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running) // BUG: Still Running even though task execution should be completed
            .Build();

        // After concurrency conflict: action execution is completed BUT task execution and task are not updated
        var completedActionExecution = ActionExecutionTestDataBuilder.Create()
            .WithId(actionExecutionId)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { completedActionExecution, actionExecution2 });
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.FindAsync(taskExecutionId).Returns(taskExecution);
        taskExecutionRepository.FindAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(taskExecution);
        var basicTaskExecutionRepository = (IBasicRepository<TaskExecutionEntity, Guid>)taskExecutionRepository;
        basicTaskExecutionRepository.FindAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(taskExecution);
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = CreateMockUnitOfWork(true);
        var verifyUow = CreateMockUnitOfWork(false);
        uowManager.Begin(true).Returns(uow);
        uowManager.Begin(false).Returns(verifyUow);
        uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException(new AbpDbConcurrencyException()));
        uow.SaveChangesAsync().Returns(Task.FromException(new AbpDbConcurrencyException()));

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.AcknowledgeActionCompletedAsync(
            actionExecutionId,
            taskExecutionId,
            Common.Module.Constants.JobSchedulerExecutionResult.Success,
            "TestUser",
            false
        );

        // Assert
        result.Should().BeTrue();

        // FIX VERIFICATION: Task execution should have been updated to Completed
        await taskExecutionRepository.Received().UpdateAsync(
            Arg.Is<TaskExecutionEntity>(te =>
                te.Id == taskExecutionId &&
                te.Status == JobSchedulerTaskExecutionStatus.Completed &&
                te.FinishedAtUtc.HasValue
            )
        );

        // FIX VERIFICATION: Task should have been updated from Running to Ready
        await taskRepository.Received().UpdateAsync(
            Arg.Is<TaskEntity>(t =>
                t.Id == taskId &&
                t.Status == JobSchedulerTaskStatus.Ready
            )
        );

        // Verify completion updates without forcing a specific UoW path
    }

    [Fact]
    public async Task AcknowledgeActionCompletedAsync_StaleTaskExecutionActions_DoesNotBlockFinish()
    {
        // Arrange
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var actionExecutionId = TestConstants.ActionExecutionIds.ActionExecution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(actionExecutionId)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed)
            .Build();

        // TaskExecution has stale action execution states (still Executing)
        var staleActionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(actionExecutionId)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithFinishedAt(null)
            .WithActionExecution(staleActionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution1, actionExecution2 });
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution1);
        SetupActionExecutionRepositoryForIdempotentUpdate(actionExecutionRepository, actionExecution2);

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.GetAsync(taskExecutionId, true).Returns(taskExecution);
        SetupTaskExecutionRepositoryForIdempotentUpdate(taskExecutionRepository, taskExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);
        SetupTaskRepositoryForIdempotentUpdate(taskRepository, task);

        var uowManager = CreateMockUnitOfWorkManager();
        SetupUnitOfWorkManagerForIdempotentUpdate(uowManager);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.AcknowledgeActionCompletedAsync(
            actionExecutionId,
            taskExecutionId,
            JobSchedulerExecutionResult.Success,
            "TestUser",
            false
        );

        // Assert
        result.Should().BeTrue();
        await taskExecutionRepository.Received().UpdateAsync(
            Arg.Is<TaskExecutionEntity>(te =>
                te.Id == taskExecutionId &&
                te.Status == JobSchedulerTaskExecutionStatus.Completed &&
                te.FinishedAtUtc.HasValue
            )
        );
        await taskRepository.Received().UpdateAsync(
            Arg.Is<TaskEntity>(t =>
                t.Id == taskId &&
                t.Status == JobSchedulerTaskStatus.Ready
            )
        );
    }
}
