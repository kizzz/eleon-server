using System;
using System.Threading;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Eleon.TestsBase.Lib.TestBase;

/// <summary>
/// Shared mock helpers for unit tests.
/// </summary>
public abstract class MockingTestBase
{
    protected IVportalLogger<T> CreateMockLogger<T>() where T : class
    {
        return TestMockHelpers.CreateMockLogger<T>();
    }

    protected IDistributedEventBus CreateMockEventBus()
    {
        return TestMockHelpers.CreateMockEventBus();
    }

    protected IConfiguration CreateMockConfiguration()
    {
        return TestMockHelpers.CreateMockConfiguration();
    }

    /// <summary>
    /// Creates an IConfiguration from a dictionary of key-value pairs.
    /// </summary>
    /// <param name="settings">Dictionary of configuration key-value pairs.</param>
    /// <returns>An IConfiguration instance.</returns>
    protected IConfiguration CreateConfiguration(Dictionary<string, string> settings)
    {
        return ConfigurationTestHelpers.CreateConfiguration(settings);
    }

    /// <summary>
    /// Creates an IConfiguration from a JSON string.
    /// </summary>
    /// <param name="json">JSON configuration string.</param>
    /// <returns>An IConfiguration instance.</returns>
    protected IConfiguration CreateConfigurationFromJson(string json)
    {
        return ConfigurationTestHelpers.CreateConfigurationFromJson(json);
    }

    /// <summary>
    /// Creates a real MemoryCache instance for testing.
    /// </summary>
    /// <returns>A MemoryCache instance.</returns>
    protected Microsoft.Extensions.Caching.Memory.IMemoryCache CreateMemoryCache()
    {
        return MemoryCacheTestHelpers.CreateMemoryCache();
    }

    protected IObjectMapper CreateMockObjectMapper()
    {
        return TestMockHelpers.CreateMockObjectMapper();
    }

    protected IObjectMapper<TContext> CreateMockObjectMapper<TContext>()
    {
        return TestMockHelpers.CreateMockObjectMapper<TContext>();
    }

    protected ICurrentUser CreateMockCurrentUser(Guid? userId = null, string userName = null, Guid? tenantId = null)
    {
        return TestMockHelpers.CreateMockCurrentUser(userId, userName, tenantId);
    }

    protected ICurrentTenant CreateMockCurrentTenant(Guid? tenantId = null, string tenantName = null)
    {
        return TestMockHelpers.CreateMockCurrentTenant(tenantId, tenantName);
    }

    protected IGuidGenerator CreateMockGuidGenerator(Guid? fixedGuid = null)
    {
        return TestMockHelpers.CreateMockGuidGenerator(fixedGuid);
    }

    protected IUnitOfWork CreateMockUnitOfWork(bool isTransactional = true)
    {
        return TestMockHelpers.CreateMockUnitOfWork(isTransactional);
    }

    protected IUnitOfWorkManager CreateMockUnitOfWorkManager(IUnitOfWork unitOfWork = null)
    {
        return TestMockHelpers.CreateMockUnitOfWorkManager(unitOfWork);
    }

    protected void SetupRepositoryGetAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryGetAsync(repository, id, entity, includeDetails);
    }

    protected void SetupRepositoryFindAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TKey id,
        TEntity entity,
        bool includeDetails = false)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryFindAsync(repository, id, entity, includeDetails);
    }

    protected void SetupRepositoryGetListAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        System.Collections.Generic.List<TEntity> entities)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryGetListAsync(repository, entities);
    }

    protected void SetupRepositoryInsertAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryInsertAsync(repository, entity);
    }

    protected void SetupRepositoryUpdateAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryUpdateAsync(repository, entity);
    }

    protected void SetupRepositoryDeleteAsync<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        TEntity entity)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupRepositoryDeleteAsync(repository, entity);
    }

    protected void SetupConcurrencyException<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>
    {
        TestMockHelpers.SetupConcurrencyException<TEntity, TKey>(repository);
    }

    protected void SetupEventBusPublishAsync(IDistributedEventBus eventBus)
    {
        TestMockHelpers.SetupEventBusPublishAsync(eventBus);
    }

    protected void VerifyEventBusPublishAsync<TEvent>(IDistributedEventBus eventBus, int times = 1)
        where TEvent : class
    {
        TestMockHelpers.VerifyEventBusPublishAsync<TEvent>(eventBus, times);
    }

    /// <summary>
    /// Creates a mock IResponseCapableEventBus that implements both IResponseCapableEventBus and IDistributedEventBus.
    /// </summary>
    protected IResponseCapableEventBus CreateMockResponseCapableEventBus()
    {
        return EventBusTestHelpers.CreateMockResponseCapableEventBus();
    }

    /// <summary>
    /// Sets up a RequestAsync call on the event bus to return a specific response.
    /// </summary>
    /// <typeparam name="TRequest">The request message type.</typeparam>
    /// <typeparam name="TResponse">The response message type.</typeparam>
    /// <param name="bus">The event bus mock.</param>
    /// <param name="response">The response to return.</param>
    protected void SetupEventBusRequestAsync<TRequest, TResponse>(
        IResponseCapableEventBus bus,
        TResponse response)
        where TResponse : class
    {
        EventBusTestHelpers.SetupEventBusRequestAsync<TRequest, TResponse>(bus, response);
    }

    /// <summary>
    /// Sets up a RequestAsync call on the event bus to return a response based on a factory function.
    /// </summary>
    /// <typeparam name="TRequest">The request message type.</typeparam>
    /// <typeparam name="TResponse">The response message type.</typeparam>
    /// <param name="bus">The event bus mock.</param>
    /// <param name="responseFactory">Factory function to create the response.</param>
    protected void SetupEventBusRequestAsync<TRequest, TResponse>(
        IResponseCapableEventBus bus,
        Func<TRequest, TResponse> responseFactory)
        where TRequest : class
        where TResponse : class
    {
        EventBusTestHelpers.SetupEventBusRequestAsync<TRequest, TResponse>(bus, responseFactory);
    }

    /// <summary>
    /// Creates a real IdentitySecurityLogManager instance with all required dependencies.
    /// </summary>
    /// <param name="userManager">The identity user manager.</param>
    /// <param name="currentUser">Optional current user for the security log context.</param>
    /// <returns>A configured IdentitySecurityLogManager instance.</returns>
    protected IdentitySecurityLogManager BuildIdentitySecurityLogManager(
        IdentityUserManager userManager,
        IdentityUser currentUser = null)
    {
        return IdentitySecurityLogTestHelpers.BuildIdentitySecurityLogManager(userManager, currentUser);
    }

    /// <summary>
    /// Creates a test AbpDynamicOptionsManager instance for the specified options type.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <returns>A test AbpDynamicOptionsManager instance.</returns>
    protected TestAbpDynamicOptionsManager<TOptions> CreateTestAbpDynamicOptionsManager<TOptions>()
        where TOptions : class, new()
    {
        return OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<TOptions>();
    }

    /// <summary>
    /// Creates a mock IHttpContextAccessor with a default HTTP context.
    /// </summary>
    /// <param name="apiKeyId">Optional API key ID to add as a claim.</param>
    /// <returns>A mock IHttpContextAccessor with configured HTTP context.</returns>
    protected Microsoft.AspNetCore.Http.IHttpContextAccessor CreateHttpContextAccessor(string apiKeyId = null)
    {
        return HttpContextTestHelpers.CreateHttpContextAccessor(apiKeyId);
    }

    /// <summary>
    /// Creates a mock IStringLocalizer with a single key-value pair.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The localized value.</param>
    /// <returns>A mock IStringLocalizer configured with the specified key-value pair.</returns>
    protected Microsoft.Extensions.Localization.IStringLocalizer<T> CreateLocalizer<T>(string key, string value)
    {
        return LocalizationTestHelpers.CreateLocalizer<T>(key, value);
    }

    /// <summary>
    /// Creates a mock IClock that returns a specific DateTime.
    /// </summary>
    /// <param name="now">The DateTime to return for IClock.Now.</param>
    /// <returns>A mock IClock configured to return the specified DateTime.</returns>
    protected Volo.Abp.Timing.IClock CreateClock(DateTime now)
    {
        return TimeTestHelpers.CreateClock(now);
    }

    /// <summary>
    /// Sets up the LazyServiceProvider for a DomainService using reflection.
    /// </summary>
    /// <param name="service">The DomainService instance.</param>
    /// <param name="guidGenerator">The IGuidGenerator to provide.</param>
    /// <param name="clock">The IClock to provide.</param>
    protected void SetLazyServiceProvider(Volo.Abp.Domain.Services.DomainService service, IGuidGenerator guidGenerator, Volo.Abp.Timing.IClock clock)
    {
        ServiceProviderTestHelpers.SetLazyServiceProvider(service, guidGenerator, clock);
    }

    /// <summary>
    /// Sets up the LazyServiceProvider and related properties for an ApplicationService using reflection.
    /// </summary>
    /// <param name="service">The ApplicationService instance (any object that needs app service dependencies).</param>
    /// <param name="objectMapper">The IObjectMapper to provide.</param>
    /// <param name="currentUser">The ICurrentUser to provide.</param>
    protected void SetAppServiceDependencies(object service, IObjectMapper objectMapper, ICurrentUser currentUser)
    {
        ServiceProviderTestHelpers.SetAppServiceDependencies(service, objectMapper, currentUser);
    }
}
