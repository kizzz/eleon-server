using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.JobScheduler.Module.Repositories;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;

namespace JobScheduler.Module.TestHelpers;

public static class MockHelpers
{
    public static IVportalLogger<T> CreateMockLogger<T>() where T : class
    {
        return TestMockHelpers.CreateMockLogger<T>();
    }

    public static ITaskRepository CreateMockTaskRepository()
    {
        return Substitute.For<ITaskRepository>();
    }

    public static IActionRepository CreateMockActionRepository()
    {
        return Substitute.For<IActionRepository>();
    }

    public static ITriggerRepository CreateMockTriggerRepository()
    {
        return Substitute.For<ITriggerRepository>();
    }

    public static ITaskExecutionRepository CreateMockTaskExecutionRepository()
    {
        return Substitute.For<ITaskExecutionRepository>();
    }

    public static IActionExecutionRepository CreateMockActionExecutionRepository()
    {
        return Substitute.For<IActionExecutionRepository>();
    }

    public static ITaskHubContext CreateMockTaskHubContext()
    {
        return Substitute.For<ITaskHubContext>();
    }

    public static IUnitOfWorkManager CreateMockUnitOfWorkManager()
    {
        return TestMockHelpers.CreateMockUnitOfWorkManager();
    }

    public static IDistributedEventBus CreateMockEventBus()
    {
        return TestMockHelpers.CreateMockEventBus();
    }

    public static IConfiguration CreateMockConfiguration()
    {
        return TestMockHelpers.CreateMockConfiguration();
    }

    public static IObjectMapper CreateMockObjectMapper()
    {
        return TestMockHelpers.CreateMockObjectMapper();
    }

    public static ICurrentUser CreateMockCurrentUser(Guid? userId = null, string userName = null, Guid? tenantId = null)
    {
        return TestMockHelpers.CreateMockCurrentUser(userId, userName, tenantId);
    }

    public static ICurrentTenant CreateMockCurrentTenant(Guid? tenantId = null, string tenantName = null)
    {
        return TestMockHelpers.CreateMockCurrentTenant(tenantId, tenantName);
    }

    public static IGuidGenerator CreateMockGuidGenerator(Guid? fixedGuid = null)
    {
        return TestMockHelpers.CreateMockGuidGenerator(fixedGuid);
    }
}
