using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedModule.modules.MultiTenancy.Module;
using TenantSettings.Module.Helpers;
using Volo.Abp.MultiTenancy;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.EventHandlers;
using VPortal.BackgroundJobs.Module.Messages;

namespace BackgroundJobs.Module.Application.EventServices;

public class ScheduleNotificationEventHandlerTests : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_ProcessesNotificationJobs()
    {
        // Arrange
        var eventData = new ScheduleNotificationEvent();
        var jobs = new List<VPortal.BackgroundJobs.Module.Entities.BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithType("SendBulkNotification")
                .Build()
        };

        var domainService = CreateMockDomainService();
        domainService.GetByType(Arg.Any<string>()).Returns(jobs);

        var multiTenancyService = CreateMultiTenancyService();

        var eventBus = CreateMockEventBus();
        var currentTenant = Substitute.For<ICurrentTenant>();

        var service = new ScheduleNotificationEventHandler(
            CreateMockLogger<ScheduleNotificationEventHandler>(),
            domainService,
            eventBus,
            multiTenancyService,
            currentTenant);

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).GetByType(Arg.Any<string>());
        await eventBus.Received().PublishAsync(
            Arg.Is<object>(msg => msg.GetType().Name.Contains("BackgroundJobExecutedMsg")),
            Arg.Any<bool>(),
            Arg.Any<bool>());
    }

    [Fact]
    public async Task HandleEventAsync_HandlesExceptions()
    {
        // Arrange
        var eventData = new ScheduleNotificationEvent();
        var domainService = CreateMockDomainService();
        domainService.GetByType(Arg.Any<string>())
            .Returns(Task.FromException<List<VPortal.BackgroundJobs.Module.Entities.BackgroundJobEntity>>(new Exception("Test exception")));

        var multiTenancyService = CreateMultiTenancyService();

        var eventBus = CreateMockEventBus();
        var currentTenant = Substitute.For<ICurrentTenant>();

        var eventHandlerLogger = CreateMockLogger<ScheduleNotificationEventHandler>();
        var service = new ScheduleNotificationEventHandler(
            eventHandlerLogger,
            domainService,
            eventBus,
            multiTenancyService,
            currentTenant);

        // Act - Should not throw, exception is captured
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).GetByType(Arg.Any<string>());
    }
}
