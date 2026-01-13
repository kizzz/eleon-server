using System;
using Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.Domain.Shared.DomainServices;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.TestBase;

public abstract class DomainTestBaseFromTest
{
  protected IVportalLogger<T> CreateMockLogger<T>()
  {
    var logger = Substitute.For<IVportalLogger<T>>();
    var standardLogger = Substitute.For<ILogger<T>>();
    // Configure the Log property BEFORE any other operations to ensure it's set up correctly
    logger.Log.Returns(standardLogger);

    return logger;
  }

  protected IBackgroundJobExecutionsRepository CreateMockExecutionsRepository()
  {
    return Substitute.For<IBackgroundJobExecutionsRepository>();
  }

  protected IBackgroundJobsRepository CreateMockJobsRepository()
  {
    return Substitute.For<IBackgroundJobsRepository>();
  }

  protected IBackgroundJobHubContext CreateMockHubContext()
  {
    return Substitute.For<IBackgroundJobHubContext>();
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

  protected IObjectMapper<ModuleDomainModule> CreateMockObjectMapper()
  {
    return Substitute.For<IObjectMapper<ModuleDomainModule>>();
  }

  protected IGuidGenerator CreateMockGuidGenerator()
  {
    return Substitute.For<IGuidGenerator>();
  }

  protected ICurrentTenant CreateMockCurrentTenant()
  {
    var currentTenant = Substitute.For<ICurrentTenant>();
    // Mock Change to return a disposable so CurrentTenant.Change(tenantId) works in tests
    currentTenant.Change(NSubstitute.Arg.Any<Guid?>()).Returns(Substitute.For<IDisposable>());
    return currentTenant;
  }

  protected IUnitOfWork CreateMockUnitOfWork(bool isTransactional = true)
  {
    var mockUow = Substitute.For<IUnitOfWork>();
    mockUow.Options.Returns(
      new Volo.Abp.Uow.AbpUnitOfWorkOptions { IsTransactional = isTransactional }
    );
    return mockUow;
  }

  protected BackgroundJobDomainService CreateBackgroundJobDomainService(
    IVportalLogger<BackgroundJobDomainService> logger = null,
    IBackgroundJobExecutionsRepository executionsRepository = null,
    IBackgroundJobsRepository jobsRepository = null,
    IBackgroundJobHubContext hubContext = null,
    IUnitOfWorkManager unitOfWorkManager = null,
    IDistributedEventBus eventBus = null,
    IConfiguration configuration = null,
    IObjectMapper<ModuleDomainModule> objectMapper = null,
    IGuidGenerator guidGenerator = null,
    ICurrentTenant currentTenant = null
  )
  {
    var service = new BackgroundJobDomainService(
      logger ?? CreateMockLogger<BackgroundJobDomainService>(),
      executionsRepository ?? CreateMockExecutionsRepository(),
      jobsRepository ?? CreateMockJobsRepository(),
      hubContext ?? CreateMockHubContext(),
      unitOfWorkManager ?? CreateMockUnitOfWorkManager(),
      eventBus ?? CreateMockEventBus(),
      configuration ?? CreateMockConfiguration(),
      objectMapper ?? CreateMockObjectMapper()
    );

    // Set up GuidGenerator via LazyServiceProvider for DomainService
    var mockGuidGenerator = guidGenerator ?? CreateMockGuidGenerator();
    mockGuidGenerator.Create().Returns(Guid.NewGuid());

    var lazyServiceProvider = Substitute.For<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
    // Mock the generic LazyGetRequiredService<T> method
    lazyServiceProvider.LazyGetRequiredService<Volo.Abp.Guids.IGuidGenerator>()
        .Returns(mockGuidGenerator);

    var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService)
        .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
    if (lazyServiceProviderProp != null && lazyServiceProviderProp.CanWrite)
    {
        lazyServiceProviderProp.SetValue(service, lazyServiceProvider);
    }

    // Set up CurrentTenant for DomainService
    var mockCurrentTenant = currentTenant ?? CreateMockCurrentTenant();
    var currentTenantProp = typeof(Volo.Abp.Domain.Services.DomainService)
        .GetProperty("CurrentTenant", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
    if (currentTenantProp != null && currentTenantProp.CanWrite)
    {
        currentTenantProp.SetValue(service, mockCurrentTenant);
    }

    return service;
  }

  protected IBackgroundJobDomainService CreateMockDomainService()
  {
    // Use the interface instead of concrete class to avoid NSubstitute void method issues
    return Substitute.For<IBackgroundJobDomainService>();
  }

  protected TenantSettings.Module.Helpers.MultiTenancyDomainService CreateMultiTenancyService(ICurrentTenant currentTenant = null)
  {
    // Set Enabled = false so GetTenantsAsync returns empty list, making ForEachTenant only process host tenant
    var multiTenancyOptions = new SharedModule.modules.MultiTenancy.Module.EleonMultiTenancyOptions { Enabled = false };
    var currentTenantForService = currentTenant ?? CreateMockCurrentTenant();
    // Ensure Change returns a disposable that doesn't throw for both null (host) and Guid? (tenant)
    var disposable = Substitute.For<IDisposable>();
    currentTenantForService.Change(NSubstitute.Arg.Any<Guid?>()).Returns(disposable);
    currentTenantForService.Change(null).Returns(disposable);

    var eventBusForCache = CreateMockEventBus();
    var tenantCacheService = Substitute.ForPartsOf<TenantSettings.Module.Cache.TenantCacheService>(
      eventBusForCache,
      currentTenantForService,
      Microsoft.Extensions.Options.Options.Create(multiTenancyOptions)
    );
    // GetTenantsAsync will return empty list when Enabled = false (real implementation)
    // Use a real logger factory to ensure extension methods work correctly
    var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning));
    var logger = loggerFactory.CreateLogger<TenantSettings.Module.Helpers.MultiTenancyDomainService>();
    var multiTenancyService = Substitute.ForPartsOf<TenantSettings.Module.Helpers.MultiTenancyDomainService>(
      logger,
      CreateMockEventBus(),
      tenantCacheService
    );
    // Set up CurrentTenant for DomainService - this is critical for DoForTenant to work
    // Based on ABP testing patterns and StatesGroupTemplateDomainServiceTests example
    var currentTenantProp = typeof(Volo.Abp.Domain.Services.DomainService)
      .GetProperty("CurrentTenant", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
    if (currentTenantProp != null)
    {
      if (currentTenantProp.CanWrite)
      {
        currentTenantProp.SetValue(multiTenancyService, currentTenantForService);
      }
      else
      {
        // Property is read-only, try to set via backing field
        var field = typeof(Volo.Abp.Domain.Services.DomainService)
          .GetField("_currentTenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
          ?? typeof(Volo.Abp.Domain.Services.DomainService)
          .GetField("currentTenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
          field.SetValue(multiTenancyService, currentTenantForService);
        }
      }
    }

    // Also set up LazyServiceProvider to provide ICurrentTenant if accessed via lazy loading
    // This is important for DomainService base class properties (ABP pattern)
    var lazyServiceProvider = Substitute.For<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
    lazyServiceProvider.LazyGetRequiredService<Volo.Abp.MultiTenancy.ICurrentTenant>()
        .Returns(currentTenantForService);

    var lazyServiceProviderProp = typeof(Volo.Abp.Domain.Services.DomainService)
        .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
    if (lazyServiceProviderProp != null && lazyServiceProviderProp.CanWrite)
    {
        lazyServiceProviderProp.SetValue(multiTenancyService, lazyServiceProvider);
    }

    return multiTenancyService;
  }
}
