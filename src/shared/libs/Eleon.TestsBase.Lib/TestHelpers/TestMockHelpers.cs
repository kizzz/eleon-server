using System;
using System.Threading;
using System.Threading.Tasks;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Shared mock helpers for tests.
/// </summary>
public static class TestMockHelpers
{
    public static IVportalLogger<T> CreateMockLogger<T>() where T : class
    {
        var logger = Substitute.For<IVportalLogger<T>>();
        var standardLogger = Substitute.For<ILogger<T>>();
        logger.Log.Returns(standardLogger);
        return logger;
    }

    public static IDistributedEventBus CreateMockEventBus()
    {
        return Substitute.For<IDistributedEventBus>();
    }

    public static IConfiguration CreateMockConfiguration()
    {
        return Substitute.For<IConfiguration>();
    }

    public static IObjectMapper CreateMockObjectMapper()
    {
        return Substitute.For<IObjectMapper>();
    }

    public static IObjectMapper<TContext> CreateMockObjectMapper<TContext>()
    {
        return Substitute.For<IObjectMapper<TContext>>();
    }

    public static ICurrentUser CreateMockCurrentUser(Guid? userId = null, string userName = null, Guid? tenantId = null)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.Id.Returns(userId);
        currentUser.UserName.Returns(userName);
        currentUser.TenantId.Returns(tenantId);
        return currentUser;
    }

    public static ICurrentTenant CreateMockCurrentTenant(Guid? tenantId = null, string tenantName = null)
    {
        var currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.Id.Returns(tenantId);
        currentTenant.Name.Returns(tenantName ?? (tenantId.HasValue ? $"Tenant-{tenantId}" : "Host"));
        currentTenant.Change(Arg.Any<Guid?>()).Returns(Substitute.For<IDisposable>());
        return currentTenant;
    }

    public static IGuidGenerator CreateMockGuidGenerator(Guid? fixedGuid = null)
    {
        var generator = Substitute.For<IGuidGenerator>();
        generator.Create().Returns(fixedGuid ?? Guid.NewGuid());
        return generator;
    }

    public static IUnitOfWork CreateMockUnitOfWork(bool isTransactional = true)
    {
        var uow = Substitute.For<IUnitOfWork>();
        uow.IsReserved.Returns(false);
        uow.IsDisposed.Returns(false);
        uow.IsCompleted.Returns(false);
        uow.Options.Returns(new AbpUnitOfWorkOptions { IsTransactional = isTransactional });
        uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        uow.CompleteAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        return uow;
    }

    public static IUnitOfWorkManager CreateMockUnitOfWorkManager(IUnitOfWork unitOfWork = null)
    {
        var manager = Substitute.For<IUnitOfWorkManager>();
        var uow = unitOfWork ?? CreateMockUnitOfWork();
        manager.Begin(Arg.Any<AbpUnitOfWorkOptions>(), Arg.Any<bool>()).Returns(uow);
        manager.Begin(Arg.Any<AbpUnitOfWorkOptions>()).Returns(uow);
        manager.Begin(Arg.Any<bool>()).Returns(uow);
        return manager;
    }

    public static void SetupRepositoryGetAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.GetAsync(id, includeDetails, Arg.Any<CancellationToken>())
            .Returns(entity);
    }

    public static void SetupRepositoryFindAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.FindAsync(id, includeDetails, Arg.Any<CancellationToken>())
            .Returns(entity);
    }

    public static void SetupRepositoryGetListAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        System.Collections.Generic.List<TEntity> entities)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.GetListAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(entities);
    }

    public static void SetupRepositoryInsertAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.InsertAsync(entity, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(entity);
    }

    public static void SetupRepositoryUpdateAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.UpdateAsync(entity, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(entity);
    }

    public static void SetupRepositoryDeleteAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.DeleteAsync(entity, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
    }

    public static void SetupConcurrencyException<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.UpdateAsync(Arg.Any<TEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns<Task<TEntity>>(callInfo => throw new Volo.Abp.Data.AbpDbConcurrencyException());
    }

    public static void SetupEventBusPublishAsync(IDistributedEventBus eventBus)
    {
        eventBus.PublishAsync(Arg.Any<object>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.CompletedTask);
    }

    public static void VerifyEventBusPublishAsync<TEvent>(IDistributedEventBus eventBus, int times = 1)
        where TEvent : class
    {
        eventBus.Received(times).PublishAsync(
            Arg.Is<TEvent>(e => e != null),
            Arg.Any<bool>(),
            Arg.Any<bool>());
    }

    public static void VerifyRepositoryGetAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        bool includeDetails = false,
        int times = 1)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.Received(times).GetAsync(id, includeDetails, Arg.Any<CancellationToken>());
    }

    public static void VerifyRepositoryUpdateAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity,
        int times = 1)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.Received(times).UpdateAsync(entity, Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    public static void VerifyRepositoryInsertAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity,
        int times = 1)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        repository.Received(times).InsertAsync(entity, Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
