using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharedModule.modules.MultiTenancy.Module;
using TenantSettings.Module.Helpers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;
using Xunit;
using Messaging.Module.ETO;

namespace BackgroundJobs.Module.Domain.DomainServices;

public class BackgroundJobManagerDomainServiceTests : DomainTestBase
{
  private new MultiTenancyDomainService CreateMultiTenancyService(ICurrentTenant currentTenant = null)
  {
    // Set Enabled = false so GetTenantsAsync returns empty list, making ForEachTenant only process host tenant
    var multiTenancyOptions = new EleonMultiTenancyOptions { Enabled = false };
    var currentTenantForService = currentTenant ?? CreateMockCurrentTenant();
    // Ensure Change returns a disposable that doesn't throw for both null (host) and Guid? (tenant)
    var disposable = Substitute.For<IDisposable>();
    currentTenantForService.Change(Arg.Any<Guid?>()).Returns(disposable);
    currentTenantForService.Change(null).Returns(disposable);
    
    var eventBusForCache = CreateMockEventBus();
    var tenantCacheService = Substitute.ForPartsOf<TenantSettings.Module.Cache.TenantCacheService>(
      eventBusForCache,
      currentTenantForService,
      Options.Create(multiTenancyOptions)
    );
    // GetTenantsAsync will return empty list when Enabled = false (real implementation)
    // Use a real logger factory to ensure extension methods work correctly
    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
    var logger = loggerFactory.CreateLogger<MultiTenancyDomainService>();
    var multiTenancyService = Substitute.ForPartsOf<MultiTenancyDomainService>(
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
  [Fact]
  public async Task RunJobsByScheduledTimeAsync_ProcessesRetryJobs()
  {
    // Arrange
    var retryJobs = new List<BackgroundJobEntity>
    {
      BackgroundJobTestDataBuilder
        .Create()
        .WithNextRetryTime(TestConstants.Dates.PastDate)
        .WithCurrentRetryAttempt(1)
        .WithRetrySettings(3, 5)
        .Build(),
    };

    var newJobs = new List<BackgroundJobEntity>
    {
      BackgroundJobTestDataBuilder
        .Create()
        .WithStatus(BackgroundJobStatus.New)
        .WithScheduleDate(TestConstants.Dates.PastDate)
        .Build(),
    };

    var backgroundJobDomainService = Substitute.For<IBackgroundJobDomainService>();

    backgroundJobDomainService.GetRetryJobsAsync().Returns(retryJobs);
    backgroundJobDomainService.GetCurrentJobsAsync().Returns(newJobs);

    var repository = CreateMockJobsRepository();
    // Use real ConfigurationBuilder to avoid issues with JobFilter constructor
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string>
      {
        { "JobOptions:Whitelist:Enabled", "false" }
      })
      .Build();
    var multiTenancyService = CreateMultiTenancyService();

    var service = new BackgroundJobManagerDomainService(
      CreateMockLogger<BackgroundJobManagerDomainService>(),
      backgroundJobDomainService,
      configuration,
      multiTenancyService,
      repository
    );

    // Act
    await service.RunJobsByScheduledTimeAsync();

    // Assert
    await backgroundJobDomainService.Received(1).GetRetryJobsAsync();
    await backgroundJobDomainService.Received(1).GetCurrentJobsAsync();
  }

  [Fact]
  public async Task RunJobsByScheduledTimeAsync_HandlesExceptionsPerJob()
  {
    // Arrange
    var retryJobs = new List<BackgroundJobEntity>
    {
      BackgroundJobTestDataBuilder.Create().WithNextRetryTime(TestConstants.Dates.PastDate).Build(),
    };

    var backgroundJobDomainService = Substitute.For<IBackgroundJobDomainService>();

    backgroundJobDomainService.GetRetryJobsAsync().Returns(retryJobs);
    backgroundJobDomainService.GetCurrentJobsAsync().Returns(new List<BackgroundJobEntity>());
    backgroundJobDomainService
      .StartExecutionAsync(Arg.Any<Guid>(), false, true)
      .ThrowsAsync(new Exception("Test exception"));

    var repository = CreateMockJobsRepository();
    // Use real ConfigurationBuilder to avoid issues with JobFilter constructor
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string>
      {
        { "JobOptions:Whitelist:Enabled", "false" }
      })
      .Build();
    var multiTenancyService = CreateMultiTenancyService();

    var logger = CreateMockLogger<BackgroundJobManagerDomainService>();
    var service = new BackgroundJobManagerDomainService(
      logger,
      backgroundJobDomainService,
      configuration,
      multiTenancyService,
      repository
    );

    // Act - Should not throw, exceptions are captured and suppressed
    await service.RunJobsByScheduledTimeAsync();

    // Assert
    await backgroundJobDomainService.Received(1).StartExecutionAsync(Arg.Any<Guid>(), false, true);
  }

  [Fact]
  public async Task CancelLongTimeJobsAsync_CancelsJobsExceedingTimeout()
  {
    // Arrange
    var longRunningJobs = new List<VPortal.BackgroundJobs.Module.Repositories.TaskIdWithTimeout>
    {
      new VPortal.BackgroundJobs.Module.Repositories.TaskIdWithTimeout(
        TestConstants.JobIds.Job1,
        60
      ),
    };

    var repository = CreateMockJobsRepository();
    repository.GetLongTimeExecutingJobIdsAsync().Returns(longRunningJobs);

    var backgroundJobDomainService = Substitute.For<IBackgroundJobDomainService>();

    var configuration = CreateMockConfiguration();
    var multiTenancyService = CreateMultiTenancyService();

    var service = new BackgroundJobManagerDomainService(
      CreateMockLogger<BackgroundJobManagerDomainService>(),
      backgroundJobDomainService,
      configuration,
      multiTenancyService,
      repository
    );

    // Act
    await service.CancelLongTimeJobsAsync();

    // Assert
    await backgroundJobDomainService
      .Received(1)
      .CancelJobAsync(
        TestConstants.JobIds.Job1,
        Arg.Any<string>(),
        false,
        Arg.Is<string>(msg => msg.Contains("timeout"))
      );
  }

  [Fact]
  public async Task CancelLongTimeJobsAsync_HandlesExceptionsPerJob()
  {
    // Arrange
    var longRunningJobs = new List<VPortal.BackgroundJobs.Module.Repositories.TaskIdWithTimeout>
    {
      new VPortal.BackgroundJobs.Module.Repositories.TaskIdWithTimeout(
        TestConstants.JobIds.Job1,
        60
      ),
    };

    var repository = CreateMockJobsRepository();
    repository.GetLongTimeExecutingJobIdsAsync().Returns(longRunningJobs);

    var backgroundJobDomainService = Substitute.For<IBackgroundJobDomainService>();

    backgroundJobDomainService
      .CancelJobAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>())
      .ThrowsAsync(new Exception("Test exception"));

    var configuration = CreateMockConfiguration();
    var multiTenancyService = CreateMultiTenancyService();

    var logger = CreateMockLogger<BackgroundJobManagerDomainService>();
    var service = new BackgroundJobManagerDomainService(
      logger,
      backgroundJobDomainService,
      configuration,
      multiTenancyService,
      repository
    );

    // Act - Should not throw, exceptions are captured and suppressed
    await service.CancelLongTimeJobsAsync();

    // Assert
    await backgroundJobDomainService
      .Received(1)
      .CancelJobAsync(Arg.Any<Guid>(), Arg.Any<string>(), false, Arg.Any<string>());
  }
}
