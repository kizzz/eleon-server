using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using NSubstitute;
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
/// Comprehensive retry logic edge case tests
/// Tests all scenarios for task retry after failure
/// </summary>
public class RetryLogicEdgeCasesTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_WithFinishedAtUtcNull_DoesNotIncrementRetryAttempt()
    {
        // Arrange - Last execution failed but FinishedAtUtc is null (edge case)
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(null) // Null FinishedAtUtc
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions, so ensure no action executions have it set
        // (empty list means IsStatusChangedManually will be false)

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(TimeSpan.FromMinutes(5), 3)
            .WithCurrentRetryAttempt(0)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Since FinishedAtUtc is null, isRetryScenario will be false (FinishedAtUtc.HasValue check fails)
        // So CurrentRetryAttempt should remain 0 (not incremented)
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_WithRestartAfterFailIntervalNull_DoesNotIncrementRetryAttempt()
    {
        // Arrange - Retry enabled but RestartAfterFailInterval is null
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions - empty list means false

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(null, 3) // Null interval means IsRetryEnabled will be false
            .WithCurrentRetryAttempt(0)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Since RestartAfterFailInterval is null, IsRetryEnabled will be false
        // So CurrentRetryAttempt should remain 0
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_WithMaxAttemptsReached_DoesNotIncrementRetryAttempt()
    {
        // Arrange - CurrentRetryAttempt already at max
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions - empty list means false

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(TimeSpan.FromMinutes(5), 3)
            .WithCurrentRetryAttempt(3) // Already at max
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Since CurrentRetryAttempt >= MaxAttempts, it's not a retry scenario
        // So CurrentRetryAttempt should remain 3 (not incremented)
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 3), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_WithIsRetryEnabledFalse_DoesNotIncrementRetryAttempt()
    {
        // Arrange - Retry disabled
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions - empty list means false

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(null, 0) // Retry disabled (null interval or 0 max attempts)
            .WithCurrentRetryAttempt(0)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Since IsRetryEnabled is false, it's not a retry scenario
        // So CurrentRetryAttempt should remain 0
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_WithManualStatusChange_DoesNotIncrementRetryAttempt()
    {
        // Arrange - Last execution manually changed
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // For manually changed scenario, add an action execution with IsStatusChangedManually = true
        var manualActionExecution = ActionExecutionTestDataBuilder.Create()
            .WithId(TestConstants.ActionExecutionIds.ActionExecution1)
            .WithTaskExecutionId(lastExecution.Id)
            .WithStatus(JobSchedulerActionExecutionStatus.Failed)
            .WithStatusChangedBy("ManualUser", manually: true)
            .Build();
        lastExecution.ActionExecutions.Add(manualActionExecution);

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(TimeSpan.FromMinutes(5), 3)
            .WithCurrentRetryAttempt(0)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Since IsStatusChangedManually is true, it's not a retry scenario
        // So CurrentRetryAttempt should remain 0
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_RetryScenario_ValidRetry_IncrementsRetryAttempt()
    {
        // Arrange - Valid retry scenario
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions - empty list means false

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithRetrySettings(TimeSpan.FromMinutes(5), 3)
            .WithCurrentRetryAttempt(1) // Will be incremented to 2
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();
        task.Executions.Add(lastExecution);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, null);

        // Assert
        result.Should().BeTrue();
        // Valid retry scenario - CurrentRetryAttempt should be incremented from 1 to 2
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 2), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_ManualRun_ResetsRetryAttempt()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithCanRunManually(true) // Required for manual runs
            .WithCurrentRetryAttempt(2)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, true, null);

        // Assert
        result.Should().BeTrue();
        // Manual run should reset CurrentRetryAttempt to 0
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task RequestTaskExecutionAsync_TriggerRun_ResetsRetryAttempt()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithStatus(JobSchedulerTaskStatus.Ready)
            .WithCurrentRetryAttempt(2)
            .WithAction(ActionTestDataBuilder.Create().WithId(TestConstants.ActionIds.Action1).Build())
            .Build();

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId, true).Returns(task);
        taskRepository.UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());

        var taskExecutionRepository = CreateMockTaskExecutionRepository();
        taskExecutionRepository.InsertAsync(Arg.Any<TaskExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

        var actionExecutionRepository = CreateMockActionExecutionRepository();
        actionExecutionRepository.GetAsync(Arg.Any<Guid>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var actionExec = new ActionExecutionEntity(id, Guid.NewGuid())
                {
                    Status = JobSchedulerActionExecutionStatus.NotStarted,
                    Action = task.Actions.FirstOrDefault()
                };
                return actionExec;
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
            unitOfWorkManager: uowManager,
            guidGenerator: guidGenerator);

        // Act
        var result = await service.RequestTaskExecutionAsync(taskId, false, trigger);

        // Assert
        result.Should().BeTrue();
        // Trigger run should reset CurrentRetryAttempt to 0
        await taskRepository.Received().UpdateAsync(Arg.Is<TaskEntity>(t => t.CurrentRetryAttempt == 0), Arg.Any<bool>());
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_RetryScenario_WithNullFinishedAtUtc_ReturnsNull()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(null) // Null FinishedAtUtc
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions, so ensure no action executions have it set
        // (empty list means IsStatusChangedManually will be false)

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithRetrySettings(TimeSpan.FromMinutes(5), 3)
            .WithCurrentRetryAttempt(1)
            .Build();
        task.Executions.Add(lastExecution);

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        // Should return null because FinishedAtUtc is null
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_RetryScenario_WithNullRestartAfterFailInterval_ReturnsNull()
    {
        // Arrange
        var taskId = TestConstants.TaskIds.Task1;
        var lastExecution = TaskExecutionTestDataBuilder.Create()
            .WithId(TestConstants.TaskExecutionIds.Execution1)
            .WithTaskId(taskId)
            .WithStatus(JobSchedulerTaskExecutionStatus.Failed)
            .WithFinishedAt(DateTime.UtcNow.AddMinutes(-10))
            .Build();
        // IsStatusChangedManually is computed from ActionExecutions - empty list means false

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithRetrySettings(null, 3) // Null interval
            .WithCurrentRetryAttempt(1)
            .Build();
        task.Executions.Add(lastExecution);

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        // Should return null because RestartAfterFailInterval is null
        result.Should().BeNull();
    }
}

