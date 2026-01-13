using System;
using System.Threading.Tasks;
using FluentAssertions;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using NSubstitute;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Application.EventServices;
using VPortal.BackgroundJobs.Module.DomainServices;
using Common.Module.Constants;

namespace BackgroundJobs.Module.Application.EventServices;

public class MargJobExecutionStartedEventHandlerTests : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_CallsDomainServiceMarkExecutionStartedAsync()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        var eventData = new MarkJobExecutionStartedMsg
        {
            JobId = jobId,
            ExecutionId = executionId
        };

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .Build();

        var logger = CreateMockLogger<MargJobExecutionStartedEventHandler>();
        var domainService = CreateMockDomainService();

        // Set up the mock method return value - use the same pattern as BackgroundJobManagerDomainServiceTests
        // which successfully uses Substitute.For with .Returns()
        domainService.MarkExecutionStartedAsync(jobId, executionId).Returns(execution);

        var service = new MargJobExecutionStartedEventHandler(
            logger,
            domainService);

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).MarkExecutionStartedAsync(jobId, executionId);
    }

    [Fact]
    public async Task HandleEventAsync_HandlesExceptions()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        var eventData = new MarkJobExecutionStartedMsg
        {
            JobId = jobId,
            ExecutionId = executionId
        };

        var domainService = CreateMockDomainService();
        domainService.MarkExecutionStartedAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>())
            .Returns(Task.FromException<VPortal.BackgroundJobs.Module.Entities.BackgroundJobExecutionEntity>(new Exception("Test exception")));

        var logger = CreateMockLogger<MargJobExecutionStartedEventHandler>();
        var service = new MargJobExecutionStartedEventHandler(
            logger,
            domainService);

        // Act - Should not throw, exception is captured and suppressed
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).MarkExecutionStartedAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>());
    }
}

