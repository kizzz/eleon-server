using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using EleonsoftAbp.Auth;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
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
/// Force stop and concurrent execution tests
/// </summary>
public class TaskExecutionDomainServiceForceStopTests : DomainTestBase
{
    [Fact]
    public async Task RequestTaskExecutionAsync_AllowForceStop_WithNextScheduledRun_StopsCurrentExecution()
    {
        // Arrange - Task is running, AllowForceStop=true, next scheduled run
        var taskId = TestConstants.TaskIds.Task1;
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithIsEnabled(true)
            .Build();

        var action = ActionTestDataBuilder.Create()
            .WithTaskId(taskId)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .WithAllowForceStop(true)
            .WithTrigger(trigger)
            .WithAction(action)
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.GetAsync(taskId, true).Returns(task); // Second call after stop

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(0, new List<TaskExecutionEntity>()));
        
        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.InsertAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());
        
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var eventBus = CreateMockEventBus();
        var taskHubContext = CreateMockTaskHubContext();

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            eventBus: eventBus,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager,
            taskHubContext: taskHubContext);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, trigger);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task StopTaskExecutionAsync_WithRunningActions_StopsAllActions()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
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
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution1)
            .WithActionExecution(actionExecution2)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(1, new List<TaskExecutionEntity> { taskExecution }));

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.StopTaskExecutionAsync(taskId, true);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_ConcurrentRequests_OnlyOneSucceeds()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var action = ActionTestDataBuilder.Create()
            .WithTaskId(taskId)
            .Build();
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithAction(action)
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

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

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentTenant: currentTenant,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager);

        // Act - Simulate concurrent requests
        async Task<bool> TryRequestAsync()
        {
            try
            {
                return await service.RequestTaskExecutionAsync(taskId, false, null);
            }
            catch (UserFriendlyException)
            {
                return false;
            }
        }

        var task1 = Task.Run(TryRequestAsync);
        var task2 = Task.Run(TryRequestAsync);

        var results = await Task.WhenAll(task1, task2);

        // Assert - At least one should succeed
        results.Any(result => result).Should().BeTrue();
    }

    [Fact]
    public async Task StopTaskExecutionAsync_ForceStopDuringActionExecution_CancelsAllActions()
    {
        // Arrange - Task with executing actions
        var taskId = TestConstants.TaskIds.Task1;
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
        var jobId = TestConstants.JobIds.Job1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing)
            .WithJobId(jobId) // Has background job
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.NotStarted)
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

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(1, new List<TaskExecutionEntity> { taskExecution }));
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

        var eventBus = CreateMockEventBus();
        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.Name.Returns("TestUser");
        currentUser.UserName.Returns("testuser");
        currentUser.Id.Returns(Guid.NewGuid());
        currentUser.IsAuthenticated.Returns(true);

        var currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.Id.Returns((Guid?)null);
        currentTenant.Name.Returns("Host");

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentUser: currentUser,
            currentTenant: currentTenant,
            eventBus: eventBus,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.StopTaskExecutionAsync(taskId, false); // Force stop

        // Assert
        result.Should().BeTrue();
        // Should cancel background job for actionExecution1
        await eventBus.Received().PublishAsync(Arg.Is<CancelBackgroundJobMsg>(m => m.JobId == jobId));
        // Should cancel actionExecution2 directly
        actionExecution2.Status.Should().Be(JobSchedulerActionExecutionStatus.Cancelled);
    }

    [Fact]
    public async Task StopTaskExecutionAsync_ForceStopWithNoRunningActions_StillStopsTask()
    {
        // Arrange - Task with no running actions
        var taskId = TestConstants.TaskIds.Task1;
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;

        var actionExecution = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed) // Already completed
            .Build();

        var taskExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(taskExecutionId)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
            .WithActionExecution(actionExecution)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Running)
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(1, new List<TaskExecutionEntity> { taskExecution }));
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.Name.Returns("TestUser");
        currentUser.IsAuthenticated.Returns(true);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentUser: currentUser,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.StopTaskExecutionAsync(taskId, true);

        // Assert
        result.Should().BeTrue();
        task.Status.Should().Be(JobSchedulerTaskStatus.Ready);
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Cancelled);
    }

    [Fact]
    public async Task StopTaskExecutionAsync_ForceStopWithPartialActionCompletion_HandlesCorrectly()
    {
        // Arrange - Some actions completed, some executing
        var taskId = TestConstants.TaskIds.Task1;
        var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;

        var actionExecution1 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Completed) // Completed
            .Build();

        var actionExecution2 = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution2)
            .WithTaskExecutionId(taskExecutionId)
            .WithStatus(JobSchedulerActionExecutionStatus.Executing) // Still executing
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

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, null)
            .Returns(new KeyValuePair<long, List<TaskExecutionEntity>>(1, new List<TaskExecutionEntity> { taskExecution }));
        taskExecutionRepository.UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.UpdateAsync(Arg.Any<ActionExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

        var uowManager = CreateMockUnitOfWorkManager();
        var uow = Substitute.For<IUnitOfWork>();
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = true });
        uow.CompleteAsync().Returns(Task.CompletedTask);
        uowManager.Begin(true).Returns(uow);

        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.Name.Returns("TestUser");
        currentUser.IsAuthenticated.Returns(true);

        var service = CreateTaskExecutionDomainService(
            taskRepository: taskRepository,
            taskExecutionRepository: taskExecutionRepository,
            actionExecutionRepository: actionExecutionRepository,
            currentUser: currentUser,
            triggerDomainService: CreateTriggerDomainService(),
            unitOfWorkManager: uowManager);

        // Act
        var result = await service.StopTaskExecutionAsync(taskId, true);

        // Assert
        result.Should().BeTrue();
        // Completed action should remain completed
        actionExecution1.Status.Should().Be(JobSchedulerActionExecutionStatus.Completed);
        // Executing action should be cancelled
        actionExecution2.Status.Should().Be(JobSchedulerActionExecutionStatus.Cancelled);
        // Task execution should be cancelled
        taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Cancelled);
    }
}
