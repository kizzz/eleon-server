using System;
using System.Threading.Tasks;
using FluentAssertions;
using Messaging.Module.ETO;
using NSubstitute;
using Volo.Abp.ObjectMapping;
using Xunit;
using BackgroundJobs.Module.BackgroundJobs;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;
using NSubstitute.ExceptionExtensions;

namespace BackgroundJobs.Module.Application.BackgroundJobs;

public class BackgroundJobHubContextTests : ApplicationTestBase
{
    [Fact]
    public async Task JobCompleted_MapsEntityToEtoAndCallsHub()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(TestConstants.JobIds.Job1)
            .Build();

        var hubContext = Substitute.For<IBackgroundJobAppHubContext>();
        var objectMapper = CreateMockObjectMapper();
        var eto = new BackgroundJobEto { Id = job.Id };
        objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job).Returns(eto);

        var service = new BackgroundJobHubContext(
            CreateMockLogger<BackgroundJobHubContext>(),
            hubContext,
            objectMapper);

        // Act
        await service.JobCompleted(job);

        // Assert
        objectMapper.Received(1).Map<BackgroundJobEntity, BackgroundJobEto>(job);
        await hubContext.Received(1).JobCompleted(eto);
    }

    [Fact]
    public async Task JobCompleted_HandlesExceptions()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create().Build();
        var hubContext = Substitute.For<IBackgroundJobAppHubContext>();
        hubContext.JobCompleted(Arg.Any<BackgroundJobEto>())
            .ThrowsAsync(new Exception("Test exception"));

        var objectMapper = CreateMockObjectMapper();
        var eto = new BackgroundJobEto();
        objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job).Returns(eto);

        var logger = CreateMockLogger<BackgroundJobHubContext>();
        var service = new BackgroundJobHubContext(logger, hubContext, objectMapper);

        // Act - Should not throw, exception is captured
        await service.JobCompleted(job);

        // Assert
        await hubContext.Received(1).JobCompleted(eto);
    }
}

