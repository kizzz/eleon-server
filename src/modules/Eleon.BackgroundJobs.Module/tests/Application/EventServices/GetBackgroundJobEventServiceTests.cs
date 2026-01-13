using System;
using System.Threading.Tasks;
using Common.EventBus.Module;
using FluentAssertions;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.ObjectMapping;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EventHandlers;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.Application.EventServices;

public class GetBackgroundJobEventServiceTests : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_RetrievesJobAndResponds()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var eventData = new GetBackgroundJobEtoMsg
        {
            BackgroundJobId = jobId
        };

        var entity = BackgroundJobTestDataBuilder.Create()
            .WithId(jobId)
            .Build();

        var repository = CreateMockJobsRepository();
        repository.GetAsync(jobId).Returns(entity);

        var responseContext = Substitute.For<IResponseContext>();
        var objectMapper = CreateMockObjectMapper();
        var eto = new BackgroundJobEto { Id = jobId };
        objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(entity).Returns(eto);

        var service = new GetBackgroundJobEventService(
            CreateMockLogger<GetBackgroundJobEventService>(),
            repository,
            responseContext,
            objectMapper);

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await repository.Received(1).GetAsync(jobId);
        await responseContext.Received(1).RespondAsync(
            Arg.Is<BackgroundJobEtoGotMsg>(msg => msg.Success && msg.BackgroundJob.Id == jobId));
    }
}

