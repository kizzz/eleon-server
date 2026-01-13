using System;
using System.Threading.Tasks;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.EventHandlers;

namespace BackgroundJobs.Module.Application.EventServices;

public class CreateBackgroundJobEventServiceTests : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_CallsDomainServiceCreateAsync()
    {
        // Arrange
        var eventData = new CreateBackgroundJobMsg
        {
            Id = TestConstants.JobIds.Job1,
            Type = TestConstants.JobTypes.TestJob,
            TenantId = TestConstants.TenantIds.Tenant1
        };

        var entity = BackgroundJobTestDataBuilder.Create()
            .WithId(eventData.Id)
            .Build();

        var domainService = CreateMockDomainService();
        domainService.CreateAsync(
            Arg.Any<Guid?>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>())
            .Returns(entity);

        var currentTenant = Substitute.For<ICurrentTenant>();
        var objectMapper = CreateMockObjectMapper();
        
        var service = new CreateBackgroundJobEventService(
            CreateMockLogger<CreateBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            objectMapper,
            currentTenant);

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).CreateAsync(
            Arg.Any<Guid?>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>());
    }
}

