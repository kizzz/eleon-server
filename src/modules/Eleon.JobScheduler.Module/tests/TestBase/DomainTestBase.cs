using System;
using System.Threading;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestBase;
using Logging.Module;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.JobScheduler.Module;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;

namespace JobScheduler.Module.TestBase;

public abstract class DomainTestBase : MockingTestBase
{
    protected ITaskRepository CreateMockTaskRepository()
    {
        return Substitute.For<ITaskRepository>();
    }

    protected IActionRepository CreateMockActionRepository()
    {
        return Substitute.For<IActionRepository>();
    }

    protected ITriggerRepository CreateMockTriggerRepository()
    {
        return Substitute.For<ITriggerRepository>();
    }

    protected ITaskExecutionRepository CreateMockTaskExecutionRepository()
    {
        return Substitute.For<ITaskExecutionRepository>();
    }

    protected IActionExecutionRepository CreateMockActionExecutionRepository()
    {
        return Substitute.For<IActionExecutionRepository>();
    }

    protected ITaskHubContext CreateMockTaskHubContext()
    {
        return Substitute.For<ITaskHubContext>();
    }

    protected new IUnitOfWork CreateMockUnitOfWork(bool requiresNew = false)
    {
        return base.CreateMockUnitOfWork(isTransactional: true);
    }

    protected void SetupActionExecutionRepositoryForIdempotentUpdate(
        IActionExecutionRepository repository,
        ActionExecutionEntity actionExecution)
    {
        // ExecuteIdempotentUpdateAsync uses IBasicRepository.GetAsync(Guid, CancellationToken)
        // Custom repositories have GetAsync(Guid, bool), so we need to handle both
        // Use ReturnsForAnyArgs to match any call signature
        repository.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>())
            .Returns(actionExecution);
        // Also configure as IBasicRepository to handle CancellationToken signature
        var basicRepo = (Volo.Abp.Domain.Repositories.IBasicRepository<ActionExecutionEntity, Guid>)repository;
        basicRepo.GetAsync(actionExecution.Id, default)
            .ReturnsForAnyArgs(actionExecution);
        repository
            .UpdateAsync(Arg.Any<ActionExecutionEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<ActionExecutionEntity>());
        basicRepo.UpdateAsync(Arg.Any<ActionExecutionEntity>(), false, default)
            .ReturnsForAnyArgs(callInfo => callInfo.Arg<ActionExecutionEntity>());
    }

    protected void SetupTaskRepositoryForIdempotentUpdate(
        ITaskRepository repository,
        TaskEntity task)
    {
        repository.GetAsync(task.Id, Arg.Any<bool>())
            .Returns(task);
        var basicRepo = (Volo.Abp.Domain.Repositories.IBasicRepository<TaskEntity, Guid>)repository;
        basicRepo.GetAsync(task.Id, default)
            .ReturnsForAnyArgs(task);
        repository
            .UpdateAsync(Arg.Any<TaskEntity>(), Arg.Any<bool>())
            .Returns(callInfo => callInfo.Arg<TaskEntity>());
        basicRepo.UpdateAsync(Arg.Any<TaskEntity>(), false, default)
            .ReturnsForAnyArgs(callInfo => callInfo.Arg<TaskEntity>());
    }

    protected void SetupTaskExecutionRepositoryForIdempotentUpdate(
        ITaskExecutionRepository repository,
        TaskExecutionEntity taskExecution)
    {
        repository.GetAsync(taskExecution.Id, Arg.Any<bool>())
            .Returns(taskExecution);
        var basicRepo = (Volo.Abp.Domain.Repositories.IBasicRepository<TaskExecutionEntity, Guid>)repository;
        basicRepo.GetAsync(taskExecution.Id, default)
            .ReturnsForAnyArgs(taskExecution);
        repository
            .UpdateAsync(Arg.Any<TaskExecutionEntity>())
            .Returns(callInfo => callInfo.Arg<TaskExecutionEntity>());
        basicRepo.UpdateAsync(Arg.Any<TaskExecutionEntity>(), false, default)
            .ReturnsForAnyArgs(callInfo => callInfo.Arg<TaskExecutionEntity>());
    }

    protected void SetupUnitOfWorkManagerForIdempotentUpdate(IUnitOfWorkManager uowManager)
    {
        var uow = CreateMockUnitOfWork();
        uowManager.Begin(Arg.Any<bool>()).Returns(uow);
    }

    protected TriggerDomainService CreateTriggerDomainService(
        IVportalLogger<TriggerDomainService> logger = null,
        ITriggerRepository triggerRepository = null,
        IDistributedEventBus eventBus = null,
        ITaskRepository taskRepository = null)
    {
        return new TriggerDomainService(
            logger ?? CreateMockLogger<TriggerDomainService>(),
            triggerRepository ?? CreateMockTriggerRepository(),
            eventBus ?? CreateMockEventBus(),
            taskRepository ?? CreateMockTaskRepository());
    }

    protected TaskDomainService CreateTaskDomainService(
        IVportalLogger<TaskDomainService> logger = null,
        ITaskRepository taskRepository = null,
        TriggerDomainService triggerDomainService = null)
    {
        return new TaskDomainService(
            logger ?? CreateMockLogger<TaskDomainService>(),
            taskRepository ?? CreateMockTaskRepository(),
            triggerDomainService ?? CreateTriggerDomainService());
    }

    protected ActionDomainService CreateActionDomainService(
        IVportalLogger<ActionDomainService> logger = null,
        IActionRepository actionRepository = null,
        ITaskRepository taskRepository = null)
    {
        return new ActionDomainService(
            logger ?? CreateMockLogger<ActionDomainService>(),
            actionRepository ?? CreateMockActionRepository(),
            taskRepository ?? CreateMockTaskRepository());
    }

    protected TaskExecutionDomainService CreateTaskExecutionDomainService(
        IVportalLogger<TaskExecutionDomainService> logger = null,
        ITaskRepository taskRepository = null,
        ITaskExecutionRepository taskExecutionRepository = null,
        IActionExecutionRepository actionExecutionRepository = null,
        ICurrentUser currentUser = null,
        TaskDomainService taskDomainService = null,
        ICurrentTenant currentTenant = null,
        IDistributedEventBus eventBus = null,
        TriggerDomainService triggerDomainService = null,
        IUnitOfWorkManager unitOfWorkManager = null,
        ITaskHubContext taskHubContext = null,
        IGuidGenerator guidGenerator = null)
    {
        var mockGuidGenerator = guidGenerator ?? CreateMockGuidGenerator();
        mockGuidGenerator.Create().Returns(Guid.NewGuid());
        
        var mockCurrentUser = currentUser ?? Substitute.For<ICurrentUser>();
        if (currentUser == null)
        {
            mockCurrentUser.IsAuthenticated.Returns(true);
        }
        
        return new TaskExecutionDomainService(
            logger ?? CreateMockLogger<TaskExecutionDomainService>(),
            taskRepository ?? CreateMockTaskRepository(),
            taskExecutionRepository ?? CreateMockTaskExecutionRepository(),
            actionExecutionRepository ?? CreateMockActionExecutionRepository(),
            mockCurrentUser,
            taskDomainService ?? CreateTaskDomainService(taskRepository: taskRepository),
            currentTenant ?? Substitute.For<ICurrentTenant>(),
            eventBus ?? CreateMockEventBus(),
            triggerDomainService ?? CreateTriggerDomainService(),
            unitOfWorkManager ?? CreateMockUnitOfWorkManager(),
            taskHubContext ?? CreateMockTaskHubContext(),
            mockGuidGenerator);
    }
}
