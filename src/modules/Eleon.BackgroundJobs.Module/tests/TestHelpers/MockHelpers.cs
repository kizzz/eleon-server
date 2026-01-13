using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.TestHelpers;

public static class MockHelpers
{
    public static IUnitOfWork CreateMockUnitOfWork(bool isTransactional = false)
    {
        return TestMockHelpers.CreateMockUnitOfWork(isTransactional);
    }

    public static IUnitOfWorkManager CreateMockUnitOfWorkManager(IUnitOfWork unitOfWork = null)
    {
        return TestMockHelpers.CreateMockUnitOfWorkManager(unitOfWork);
    }

    public static IBackgroundJobsRepository CreateMockJobsRepository()
    {
        return Substitute.For<IBackgroundJobsRepository>();
    }

    public static IBackgroundJobExecutionsRepository CreateMockExecutionsRepository()
    {
        return Substitute.For<IBackgroundJobExecutionsRepository>();
    }

    public static IBackgroundJobMessagesRepository CreateMockMessagesRepository()
    {
        return Substitute.For<IBackgroundJobMessagesRepository>();
    }

    public static IDistributedEventBus CreateMockEventBus()
    {
        return TestMockHelpers.CreateMockEventBus();
    }

    public static IGuidGenerator CreateMockGuidGenerator(Guid? fixedGuid = null)
    {
        return TestMockHelpers.CreateMockGuidGenerator(fixedGuid);
    }

    public static ICurrentTenant CreateMockCurrentTenant(Guid? tenantId = null)
    {
        return TestMockHelpers.CreateMockCurrentTenant(tenantId);
    }

    public static IVportalLogger<T> CreateMockLogger<T>() where T : class
    {
        return TestMockHelpers.CreateMockLogger<T>();
    }

    public static void SetupRepositoryGetAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryGetAsync(repository, id, entity, includeDetails);
    }

    public static void SetupRepositoryFindAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryFindAsync(repository, id, entity, includeDetails);
    }

    public static void SetupRepositoryGetListAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        List<TEntity> entities)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryGetListAsync(repository, entities);
    }

    public static void SetupRepositoryInsertAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryInsertAsync(repository, entity);
    }

    public static void SetupRepositoryUpdateAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryUpdateAsync(repository, entity);
    }

    public static void SetupRepositoryDeleteAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryDeleteAsync(repository, entity);
    }

    public static void SetupConcurrencyException<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.SetupConcurrencyException<TEntity, TKey>(repository);
    }

    public static void SetupEventBusPublishAsync(IDistributedEventBus eventBus)
    {
        TestMockHelpers.SetupEventBusPublishAsync(eventBus);
    }

    public static void VerifyEventBusPublishAsync<TEvent>(IDistributedEventBus eventBus, int times = 1) where TEvent : class
    {
        TestMockHelpers.VerifyEventBusPublishAsync<TEvent>(eventBus, times);
    }

    public static void VerifyRepositoryGetAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        bool includeDetails = false,
        int times = 1)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.VerifyRepositoryGetAsync(repository, id, includeDetails, times);
    }

    public static void VerifyRepositoryUpdateAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity,
        int times = 1)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.VerifyRepositoryUpdateAsync(repository, entity, times);
    }

    public static void VerifyRepositoryInsertAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity,
        int times = 1)
        where TEntity : class, IEntity<TKey>
    {
        TestMockHelpers.VerifyRepositoryInsertAsync(repository, entity, times);
    }
}
