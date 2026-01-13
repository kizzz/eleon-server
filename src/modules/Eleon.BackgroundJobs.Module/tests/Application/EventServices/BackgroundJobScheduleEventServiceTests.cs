using System;
using System.Threading.Tasks;
using BackgroundJobs.Module.EventServices;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using FluentAssertions;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedModule.modules.MultiTenancy.Module;
using VPortal.BackgroundJobs.Module.DomainServices;
using Xunit;

namespace BackgroundJobs.Module.Application.EventServices;

public class BackgroundJobScheduleEventServiceTests : DomainTestBase
{
  [Fact]
  public async Task HandleEventAsync_BackgroundJobsEnabled_CallsManagerMethods()
  {
    // Arrange
    var eventData = new ScheduleMsg();
    var configuration = CreateMockConfiguration();
    configuration["BackgroundJobs"].Returns("true");

    var managerService = Substitute.For<BackgroundJobManagerDomainService>(
      CreateMockLogger<BackgroundJobManagerDomainService>(),
      CreateMockDomainService(),
      configuration,
      Substitute.For<TenantSettings.Module.Helpers.MultiTenancyDomainService>(
        Microsoft
          .Extensions.Logging.LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning)
          )
          .CreateLogger<TenantSettings.Module.Helpers.MultiTenancyDomainService>(),
        CreateMockEventBus(),
        Substitute.ForPartsOf<TenantSettings.Module.Cache.TenantCacheService>(
          Substitute.For<Volo.Abp.EventBus.Distributed.IDistributedEventBus>(),
          Substitute.For<Volo.Abp.MultiTenancy.ICurrentTenant>(),
          Options.Create(new EleonMultiTenancyOptions())
        )
      ),
      CreateMockJobsRepository()
    );

    var service = new BackgroundJobScheduleEventService(
      CreateMockLogger<BackgroundJobScheduleEventService>(),
      managerService,
      configuration
    );

    // Act
    await service.HandleEventAsync(eventData);

    // Assert
    await managerService.Received(1).RunJobsByScheduledTimeAsync();
    await managerService.Received(1).CancelLongTimeJobsAsync();
  }

  [Fact]
  public async Task HandleEventAsync_BackgroundJobsDisabled_SkipsExecution()
  {
    // Arrange
    var eventData = new ScheduleMsg();
    var configuration = CreateMockConfiguration();
    configuration["BackgroundJobs"].Returns("false");

    var managerService = Substitute.For<BackgroundJobManagerDomainService>(
      CreateMockLogger<BackgroundJobManagerDomainService>(),
      CreateMockDomainService(),
      configuration,
      Substitute.For<TenantSettings.Module.Helpers.MultiTenancyDomainService>(
        Microsoft
          .Extensions.Logging.LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning)
          )
          .CreateLogger<TenantSettings.Module.Helpers.MultiTenancyDomainService>(),
        CreateMockEventBus(),
        Substitute.ForPartsOf<TenantSettings.Module.Cache.TenantCacheService>(
          Substitute.For<Volo.Abp.EventBus.Distributed.IDistributedEventBus>(),
          Substitute.For<Volo.Abp.MultiTenancy.ICurrentTenant>(),
          Options.Create(new EleonMultiTenancyOptions())
        )
      ),
      CreateMockJobsRepository()
    );

    var service = new BackgroundJobScheduleEventService(
      CreateMockLogger<BackgroundJobScheduleEventService>(),
      managerService,
      configuration
    );

    // Act
    await service.HandleEventAsync(eventData);

    // Assert
    await managerService.DidNotReceive().RunJobsByScheduledTimeAsync();
    await managerService.DidNotReceive().CancelLongTimeJobsAsync();
  }
}
