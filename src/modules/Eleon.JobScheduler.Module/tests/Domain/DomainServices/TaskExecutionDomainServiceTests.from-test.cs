using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;
using FluentAssertions;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.Data;
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
/// Comprehensive tests for TaskExecutionDomainService - critical concurrency handling
/// </summary>
public class TaskExecutionDomainServiceTestsFromTest : DomainTestBase
{
  private new IGuidGenerator CreateMockGuidGenerator(Guid? fixedGuid = null)
  {
    var guidGenerator = Substitute.For<IGuidGenerator>();
    if (fixedGuid.HasValue)
    {
      guidGenerator.Create().Returns(fixedGuid.Value);
    }
    else
    {
      guidGenerator.Create().Returns(Guid.NewGuid());
    }
    return guidGenerator;
  }

  private new IUnitOfWork CreateMockUnitOfWork(bool isTransactional = true)
  {
    var uow = Substitute.For<IUnitOfWork>();
    uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = isTransactional });
    uow.CompleteAsync().Returns(Task.CompletedTask);
    return uow;
  }

  [Fact]
  public async Task AcknowledgeActionCompletedAsync_WithSuccess_CompletesActionExecution()
  {
    // Arrange
    var actionExecutionId = TestConstants.ActionExecutionIds.ActionExecution1;
    var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
    var taskId = TestConstants.TaskIds.Task1;

    var actionExecution = ActionExecutionTestDataBuilder
      .Create()
      .WithId(actionExecutionId)
      .WithTaskExecutionId(taskExecutionId)
      .WithStatus(JobSchedulerActionExecutionStatus.Executing)
      .Build();

    var taskExecution = TaskExecutionTestDataBuilder
      .Create()
      .WithId(taskExecutionId)
      .WithTaskId(taskId)
      .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
      .WithActionExecution(actionExecution)
      .Build();

    var task = TaskTestDataBuilder
      .Create()
      .WithId(taskId)
      .WithStatus(JobSchedulerTaskStatus.Running)
      .Build();

    var actionExecutionRepository = CreateMockActionExecutionRepository();
    actionExecutionRepository
      .GetListByTaskExecutionIdAsync(taskExecutionId)
      .Returns(new List<ActionExecutionEntity> { actionExecution });

    var taskExecutionRepository = CreateMockTaskExecutionRepository();
    taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);

    var taskRepository = CreateMockTaskRepository();
    taskRepository.GetWithTriggerAsync(taskId).Returns(task);

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);

    var eventBus = CreateMockEventBus();
    var taskHubContext = CreateMockTaskHubContext();
    var triggerDomainService = CreateTriggerDomainService();
    var taskDomainService = CreateTaskDomainService(
      taskRepository: taskRepository,
      triggerDomainService: triggerDomainService
    );
    var currentUser = Substitute.For<ICurrentUser>();
    var currentTenant = Substitute.For<ICurrentTenant>();

    var service = CreateTaskExecutionDomainService(
      taskRepository: taskRepository,
      taskExecutionRepository: taskExecutionRepository,
      actionExecutionRepository: actionExecutionRepository,
      currentUser: currentUser,
      taskDomainService: taskDomainService,
      currentTenant: currentTenant,
      eventBus: eventBus,
      triggerDomainService: triggerDomainService,
      unitOfWorkManager: uowManager,
      taskHubContext: taskHubContext
    );

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
    actionExecution.Status.Should().Be(JobSchedulerActionExecutionStatus.Completed);
    actionExecution.CompletedAtUtc.Should().NotBeNull();
    await actionExecutionRepository.Received(1).UpdateAsync(Arg.Any<ActionExecutionEntity>());
  }

  [Fact]
  public async Task AcknowledgeActionCompletedAsync_AlreadyCompleted_ReturnsTrueIdempotently()
  {
    // Arrange
    var actionExecutionId = TestConstants.ActionExecutionIds.ActionExecution1;
    var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;

    var actionExecution = ActionExecutionTestDataBuilder
      .Create()
      .WithId(actionExecutionId)
      .WithTaskExecutionId(taskExecutionId)
      .WithStatus(JobSchedulerActionExecutionStatus.Completed)
      .WithCompletedAt(TestConstants.Dates.UtcNow)
      .Build();

    var actionExecutionRepository = CreateMockActionExecutionRepository();
    actionExecutionRepository
      .GetListByTaskExecutionIdAsync(taskExecutionId)
      .Returns(new List<ActionExecutionEntity> { actionExecution });

    var taskExecutionRepository = CreateMockTaskExecutionRepository();
    var taskExecution = TaskExecutionTestDataBuilder
      .Create()
      .WithId(taskExecutionId)
      .WithActionExecution(actionExecution)
      .Build();
    taskExecutionRepository.GetAsync(taskExecutionId, false).Returns(taskExecution);

    var uowManager = CreateMockUnitOfWorkManager();
    var uow = CreateMockUnitOfWork(true);
    uowManager.Begin(true).Returns(uow);

    var service = CreateTaskExecutionDomainService(
      taskExecutionRepository: taskExecutionRepository,
      actionExecutionRepository: actionExecutionRepository,
      unitOfWorkManager: uowManager
    );

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
    // Should not update if already completed
    await actionExecutionRepository.DidNotReceive().UpdateAsync(Arg.Any<ActionExecutionEntity>());
  }

  [Fact]
  public async Task RequestTaskExecutionAsync_ManualRun_CreatesTaskExecution()
  {
    // Arrange
    var taskId = TestConstants.TaskIds.Task1;
    var action1 = ActionTestDataBuilder
      .Create()
      .WithId(TestConstants.ActionIds.Action1)
      .WithTaskId(taskId)
      .Build();
    var action2 = ActionTestDataBuilder
      .Create()
      .WithId(TestConstants.ActionIds.Action2)
      .WithTaskId(taskId)
      .Build();

    var task = TaskTestDataBuilder
      .Create()
      .WithId(taskId)
      .WithStatus(JobSchedulerTaskStatus.Ready)
      .WithCanRunManually(true)
      .WithActions(new List<ActionEntity> { action1, action2 })
      .Build();

    var taskRepository = CreateMockTaskRepository();
    taskRepository.GetAsync(taskId, true).Returns(task);
    taskRepository
      .UpdateAsync(Arg.Any<TaskEntity>(), true)
      .Returns(callInfo => callInfo.Arg<TaskEntity>());

    var taskExecutionRepository = CreateMockTaskExecutionRepository();
    var newTaskExecutionId = Guid.NewGuid();
    taskExecutionRepository
      .InsertAsync(Arg.Any<TaskExecutionEntity>(), true)
      .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

    var actionExecutionRepository = CreateMockActionExecutionRepository();
    actionExecutionRepository
      .InsertAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
      .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

    var guidGenerator = CreateMockGuidGenerator(newTaskExecutionId);

    var uowManager = CreateMockUnitOfWorkManager();
    var eventBus = CreateMockEventBus();
    var triggerDomainService = CreateTriggerDomainService();
    var taskDomainService = CreateTaskDomainService(
      taskRepository: taskRepository,
      triggerDomainService: triggerDomainService
    );
    var currentUser = Substitute.For<ICurrentUser>();
    currentUser.Id.Returns(TestConstants.Users.User1);
    currentUser.Name.Returns("TestUser");

    var service = CreateTaskExecutionDomainService(
      taskRepository: taskRepository,
      taskExecutionRepository: taskExecutionRepository,
      actionExecutionRepository: actionExecutionRepository,
      currentUser: currentUser,
      taskDomainService: taskDomainService,
      eventBus: eventBus,
      triggerDomainService: triggerDomainService,
      unitOfWorkManager: uowManager,
      guidGenerator: guidGenerator
    );

    // Act
    var result = await service.RequestTaskExecutionAsync(taskId, manual: true, runnedTrigger: null);

    // Assert
    result.Should().BeTrue();
    task.Status.Should().Be(JobSchedulerTaskStatus.Running);
    task.CurrentRetryAttempt.Should().Be(0); // Reset on manual run
    await taskRepository.Received(1).UpdateAsync(Arg.Any<TaskEntity>(), true);
    await taskExecutionRepository.Received(1).InsertAsync(Arg.Any<TaskExecutionEntity>(), true);
  }

  [Fact]
  public async Task RequestTaskExecutionAsync_TaskNotReady_ThrowsException()
  {
    // Arrange
    var taskId = TestConstants.TaskIds.Task1;
    var task = TaskTestDataBuilder
      .Create()
      .WithId(taskId)
      .WithStatus(JobSchedulerTaskStatus.Inactive)
      .Build();

    var taskRepository = CreateMockTaskRepository();
    taskRepository.GetAsync(taskId, true).Returns(task);

    var service = CreateTaskExecutionDomainService(taskRepository: taskRepository);

    // Act & Assert
    await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(async () =>
      await service.RequestTaskExecutionAsync(taskId, manual: false, runnedTrigger: null)
    );
  }

  [Fact]
  public async Task StopTaskExecutionAsync_StopsRunningTask()
  {
    // Arrange
    var taskId = TestConstants.TaskIds.Task1;
    var taskExecutionId = TestConstants.TaskExecutionIds.Execution1;
    var actionExecutionId = TestConstants.ActionExecutionIds.ActionExecution1;
    var jobId = TestConstants.JobIds.Job1;

    var actionExecution = ActionExecutionTestDataBuilder
      .Create()
      .WithId(actionExecutionId)
      .WithTaskExecutionId(taskExecutionId)
      .WithStatus(JobSchedulerActionExecutionStatus.Executing)
      .WithJobId(jobId)
      .Build();

    var taskExecution = TaskExecutionTestDataBuilder
      .Create()
      .WithId(taskExecutionId)
      .WithTaskId(taskId)
      .WithStatus(JobSchedulerTaskExecutionStatus.Executing)
      .WithActionExecution(actionExecution)
      .Build();

    var task = TaskTestDataBuilder
      .Create()
      .WithId(taskId)
      .WithStatus(JobSchedulerTaskStatus.Running)
      .Build();

    var taskRepository = CreateMockTaskRepository();
    taskRepository.GetAsync(taskId, true).Returns(task);
    taskRepository
      .UpdateAsync(Arg.Any<TaskEntity>(), true)
      .Returns(callInfo => callInfo.Arg<TaskEntity>());

    var taskExecutionRepository = CreateMockTaskExecutionRepository();
    taskExecutionRepository
      .GetListAsync(taskId, 0, int.MaxValue, null)
      .Returns(
        Task.FromResult(
          new KeyValuePair<long, List<TaskExecutionEntity>>(
            1,
            new List<TaskExecutionEntity> { taskExecution }
          )
        )
      );
    taskExecutionRepository
      .UpdateAsync(Arg.Any<TaskExecutionEntity>())
      .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());

    var actionExecutionRepository = CreateMockActionExecutionRepository();
    actionExecutionRepository
      .UpdateAsync(Arg.Any<ActionExecutionEntity>())
      .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());

    var eventBus = CreateMockEventBus();
    var currentUser = Substitute.For<ICurrentUser>();
    currentUser.UserName.Returns("TestUser");
    currentUser.Id.Returns(TestConstants.Users.User1);
    currentUser.IsAuthenticated.Returns(true);
    currentUser.Name.Returns("TestUser");
    currentUser.SurName.Returns("User");
    currentUser.FindClaimValue(Arg.Any<string>()).Returns((string)null);

    var currentTenant = Substitute.For<ICurrentTenant>();
    currentTenant.Id.Returns((Guid?)null);
    currentTenant.Name.Returns("Host");

    var service = CreateTaskExecutionDomainService(
      taskRepository: taskRepository,
      taskExecutionRepository: taskExecutionRepository,
      actionExecutionRepository: actionExecutionRepository,
      currentUser: currentUser,
      currentTenant: currentTenant,
      eventBus: eventBus
    );

    // Act
    // Ensure ActionExecutions is initialized before calling the service
    if (taskExecution.ActionExecutions == null)
    {
      taskExecution.ActionExecutions = new List<ActionExecutionEntity> { actionExecution };
    }
    var result = await service.StopTaskExecutionAsync(taskId, manually: true);

    // Assert
    result.Should().BeTrue();
    task.Status.Should().Be(JobSchedulerTaskStatus.Ready);
    taskExecution.Status.Should().Be(JobSchedulerTaskExecutionStatus.Cancelled);
    await eventBus.Received(1).PublishAsync(Arg.Any<CancelBackgroundJobMsg>());
    await eventBus.Received(1).PublishAsync(Arg.Any<JobSchedulerTaskExecutionCanceledMsg>());
  }
}
