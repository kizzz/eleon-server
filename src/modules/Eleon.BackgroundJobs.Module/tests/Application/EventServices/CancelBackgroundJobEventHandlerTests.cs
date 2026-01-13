using System;
using System.Threading.Tasks;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Application.EventServices;
using VPortal.BackgroundJobs.Module.DomainServices;

namespace BackgroundJobs.Module.Application.EventServices;

public class CancelBackgroundJobEventHandlerTests : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_CallsDomainServiceCancelJobAsync()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var eventData = new CancelBackgroundJobMsg
        {
            JobId = jobId,
            CancelledBy = TestConstants.Users.TestUser,
            IsManually = true,
            CancelledMessage = "Test cancellation"
        };

        var domainService = CreateMockDomainService();
        domainService.CancelJobAsync(
            jobId,
            eventData.CancelledBy,
            eventData.IsManually,
            eventData.CancelledMessage)
            .Returns(Task.CompletedTask);

        var service = new CancelBackgroundJobEventHandler(
            domainService,
            CreateMockLogger<CancelBackgroundJobEventHandler>());

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).CancelJobAsync(
            jobId,
            eventData.CancelledBy,
            eventData.IsManually,
            eventData.CancelledMessage);
    }

    [Fact]
    public async Task HandleEventAsync_HandlesExceptions()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var eventData = new CancelBackgroundJobMsg
        {
            JobId = jobId,
            CancelledBy = TestConstants.Users.TestUser,
            IsManually = true,
            CancelledMessage = "Test cancellation"
        };

        var domainService = CreateMockDomainService();
        domainService.CancelJobAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>())
            .Returns(Task.FromException(new Exception("Test exception")));

        var logger = CreateMockLogger<CancelBackgroundJobEventHandler>();
        var service = new CancelBackgroundJobEventHandler(
            domainService,
            logger);

        // Act - Should not throw, exception is captured and suppressed
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).CancelJobAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>());
    }
}

