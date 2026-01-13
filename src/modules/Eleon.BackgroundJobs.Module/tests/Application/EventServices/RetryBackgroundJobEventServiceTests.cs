using System;
using System.Threading.Tasks;
using BackgroundJobs.Module.EventServices;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.DomainServices;
using Xunit;

namespace BackgroundJobs.Module.Application.EventServices;

public class RetryBackgroundJobEventServiceTests : DomainTestBase
{
  [Fact]
  public async Task HandleEventAsync_CallsDomainServiceRetryJob()
  {
    // Arrange
    var jobId = TestConstants.JobIds.Job1;
    var eventData = new RetryBackgroundJobMsg
    {
      JobId = jobId,
      StartExecutionParams = "new params",
      TimeoutInMinutes = 120,
      MaxRetryAttempts = 5,
      RetryInMinutes = 10,
    };

    var domainService = CreateMockDomainService();
    domainService
      .RetryJob(
        jobId,
        eventData.StartExecutionParams,
        eventData.StartExecutionExtraParams,
        eventData.TimeoutInMinutes,
        eventData.MaxRetryAttempts,
        eventData.RetryInMinutes,
        eventData.OnFailureRecepients
      )
      .Returns(true);

    var uow = CreateMockUnitOfWork(true);
    uow.SaveChangesAsync().Returns(Task.CompletedTask);
    uow.CompleteAsync().Returns(Task.CompletedTask);

    // Create a mock of IUnitOfWorkManager (now that service accepts IUnitOfWorkManager)
    var uowManager = CreateMockUnitOfWorkManager();
    uowManager.Begin(true).Returns(uow);

    var currentTenant = Substitute.For<ICurrentTenant>();
    var objectMapper = CreateMockObjectMapper();
    var guidGenerator = CreateMockGuidGenerator();

    var service = new RetryBackgroundJobEventService(
      CreateMockLogger<RetryBackgroundJobEventService>(),
      CreateMockEventBus(),
      objectMapper,
      currentTenant,
      domainService,
      CreateMockJobsRepository(),
      guidGenerator,
      uowManager
    );

    // Act
    await service.HandleEventAsync(eventData);

    // Assert
    await domainService
      .Received(1)
      .RetryJob(
        jobId,
        eventData.StartExecutionParams,
        eventData.StartExecutionExtraParams,
        eventData.TimeoutInMinutes,
        eventData.MaxRetryAttempts,
        eventData.RetryInMinutes,
        eventData.OnFailureRecepients
      );
  }
}
