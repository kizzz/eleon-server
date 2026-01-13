using System;
using Eleon.Logging.Lib.SystemLog.Contracts;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.JobScheduler.Module;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Repositories;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;

namespace JobScheduler.Module.TestBase;

public abstract class DomainTestBaseFromTest
{
    protected IVportalLogger<T> CreateMockLogger<T>()
    {
        var logger = Substitute.For<IVportalLogger<T>>();
        var standardLogger = Substitute.For<ILogger<T>>();
        logger.Log.Returns(standardLogger);
        return logger;
    }

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

    protected IUnitOfWorkManager CreateMockUnitOfWorkManager()
    {
        return Substitute.For<IUnitOfWorkManager>();
    }

    protected IDistributedEventBus CreateMockEventBus()
    {
        return Substitute.For<IDistributedEventBus>();
    }

    protected IConfiguration CreateMockConfiguration()
    {
        return Substitute.For<IConfiguration>();
    }

    protected IObjectMapper CreateMockObjectMapper()
    {
        return Substitute.For<IObjectMapper>();
    }

    protected IGuidGenerator CreateMockGuidGenerator()
    {
        return Substitute.For<IGuidGenerator>();
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

