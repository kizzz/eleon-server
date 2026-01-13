using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;
using Xunit;

namespace JobScheduler.Module.Domain.DomainServices;

/// <summary>
/// Task execution edge case tests
/// Tests scenarios: no actions, all cancelled, mixed results, timeout scenarios
/// </summary>
public class TaskExecutionEdgeCasesTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task RequestTaskExecutionAsync_TaskWithNoActions_ThrowsException()
    {
        // Arrange - Task with no actions
        var taskId = TestConstants.TaskIds.Task1;
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithActions(new List<ActionEntity>()) // Empty actions list
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.RequestTaskExecutionAsync(taskId, false, null));
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_TaskWithNullActions_ThrowsException()
    {
        // Arrange - Task with null actions
        var taskId = TestConstants.TaskIds.Task1;
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .Build();
        task.Actions = null; // Null actions

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository);

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            await service.RequestTaskExecutionAsync(taskId, false, null));
    }

    [Fact]
    public async Task FinishTaskExecutionAsync_AllActionsCancelled_MarksTaskExecutionAsCancelled()
    {
        // Arrange
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution1, actionExecution2 });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        var taskHubContext = CreateMockTaskHubContext();
        var triggerDomainService = CreateTriggerDomainService();
        var taskDomainService = CreateTaskDomainService(
            taskRepository: taskRepository,
            triggerDomainService: triggerDomainService);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            taskDomainService: taskDomainService,
            eventBus: eventBus,
            triggerDomainService: triggerDomainService,
            unitOfWorkManager: uowManager,
            taskHubContext: taskHubContext);

        // Act - Complete both actions (triggers FinishTaskExecutionAsync check)
        // Since all actions are cancelled, task execution should be marked as cancelled
        // First complete action1
        await service.AcknowledgeActionCompletedAsync(
            actionExecution1.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Cancelled,
            "TestUser",
            false);
        
        // Then complete action2 - this should trigger FinishTaskExecutionAsync
        await service.AcknowledgeActionCompletedAsync(
            actionExecution2.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Cancelled,
            "TestUser",
            false);

        // Assert
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Cancelled);
        taskExecution.FinishedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task FinishTaskExecutionAsync_MixedResults_MarksTaskExecutionAsFailed()
    {
        // Arrange - Mixed results: one completed, one failed
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution1, actionExecution2 });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        var taskHubContext = CreateMockTaskHubContext();
        var triggerDomainService = CreateTriggerDomainService();
        var taskDomainService = CreateTaskDomainService(
            taskRepository: taskRepository,
            triggerDomainService: triggerDomainService);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            taskDomainService: taskDomainService,
            eventBus: eventBus,
            triggerDomainService: triggerDomainService,
            unitOfWorkManager: uowManager,
            taskHubContext: taskHubContext);

        // Act - Complete both actions (triggers FinishTaskExecutionAsync)
        // First complete action1
        await service.AcknowledgeActionCompletedAsync(
            actionExecution1.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Success,
            "TestUser",
            false);
        
        // Then complete action2 with fail - this should trigger FinishTaskExecutionAsync
        await service.AcknowledgeActionCompletedAsync(
            actionExecution2.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Fail,
            "TestUser",
            false);

        // Assert
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Failed);
        taskExecution.FinishedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task FinishTaskExecutionAsync_AllActionsCompleted_MarksTaskExecutionAsCompleted()
    {
        // Arrange
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Start as executing
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution1, actionExecution2 });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        var taskHubContext = CreateMockTaskHubContext();
        var triggerDomainService = CreateTriggerDomainService();
        var taskDomainService = CreateTaskDomainService(
            taskRepository: taskRepository,
            triggerDomainService: triggerDomainService);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            taskDomainService: taskDomainService,
            eventBus: eventBus,
            triggerDomainService: triggerDomainService,
            unitOfWorkManager: uowManager,
            taskHubContext: taskHubContext);

        // Act - Complete both actions (triggers FinishTaskExecutionAsync)
        // First complete action1
        await service.AcknowledgeActionCompletedAsync(
            actionExecution1.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Success,
            "TestUser",
            false);
        
        // Then complete action2 - this should trigger FinishTaskExecutionAsync
        await service.AcknowledgeActionCompletedAsync(
            actionExecution2.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Success,
            "TestUser",
            false);

        // Assert
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Completed);
        taskExecution.FinishedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task FinishTaskExecutionAsync_AlreadyInFinalState_SkipsFinishOperation()
    {
        // Arrange - Task execution already completed
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var taskId = TestConstants.TaskIds.Task1;

        var actionExecution = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed)
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Completed) // Already completed
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-5)) // Already finished
            .WithActionExecution(actionExecution)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .Build();

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId)
            .Returns(new List<ActionExecutionEntity> { actionExecution });

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetWithTriggerAsync(taskId).Returns(task);

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        var taskHubContext = CreateMockTaskHubContext();
        var triggerDomainService = CreateTriggerDomainService();
        var taskDomainService = CreateTaskDomainService(
            taskRepository: taskRepository,
            triggerDomainService: triggerDomainService);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            taskDomainService: taskDomainService,
            eventBus: eventBus,
            triggerDomainService: triggerDomainService,
            unitOfWorkManager: uowManager,
            taskHubContext: taskHubContext);

        // Act - Try to complete action (should be idempotent)
        var result = await service.AcknowledgeActionCompletedAsync(
            actionExecution.Id,
            taskExecutionId,
            JobSchedulerExecutionResult.Success,
            "TestUser",
            false);

        // Assert
        result.Should().BeTrue();
        // Task execution should remain in Completed state
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Completed);
        // Should not update task execution (already in final state)
        await taskExecutionRepository.DidNotReceive().UpdateAsync(Arg.Is<TaskExecutionEntity>(t => t.Id == taskExecutionId));
    }
}

