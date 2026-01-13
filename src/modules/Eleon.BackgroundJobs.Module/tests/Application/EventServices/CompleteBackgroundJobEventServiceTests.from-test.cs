using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EventHandlers;

namespace BackgroundJobs.Module.Application.EventServices;

public class CompleteBackgroundJobEventServiceTestsFromTest : DomainTestBase
{
    [Fact]
    public async Task HandleEventAsync_CallsDomainServiceCompleteExecutionAsync()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        
        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            Messages = new List<Messaging.Module.ETO.BackgroundJobTextInfoEto>
            {
                new() { Type = BackgroundJobMessageType.Info, TextMessage = "Test message" }
            },
            Result = "Success",
            CompletedBy = TestConstants.Users.TestUser,
            IsManually = true,
            TenantId = TestConstants.TenantIds.Tenant1
        };

        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithId(executionId)
            .WithStatus(BackgroundJobExecutionStatus.Completed)
            .Build();

        var domainService = CreateMockDomainService();
        domainService.CompleteExecutionAsync(
            jobId,
            executionId,
            true,
            null,
            null,
            Arg.Any<List<BackgroundJobMessageEntity>>(),
            "Success",
            TestConstants.Users.TestUser,
            true)
            .Returns(execution);

        var currentTenant = Substitute.For<ICurrentTenant>();
        var objectMapper = CreateMockObjectMapper();
        
        var service = new CompleteBackgroundJobEventService(
            CreateMockLogger<CompleteBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            objectMapper,
            currentTenant);

        // Act
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).CompleteExecutionAsync(
            jobId,
            executionId,
            true,
            null,
            null,
            Arg.Any<List<BackgroundJobMessageEntity>>(),
            "Success",
            TestConstants.Users.TestUser,
            true);
    }

    [Fact]
    public async Task HandleEventAsync_HandlesConcurrencyExceptionGracefully()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        
        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            TenantId = TestConstants.TenantIds.Tenant1
        };

        var domainService = CreateMockDomainService();
        domainService.CompleteExecutionAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<List<BackgroundJobMessageEntity>>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>())
            .Returns(Task.FromException<BackgroundJobExecutionEntity>(new AbpDbConcurrencyException()));

        var currentTenant = Substitute.For<ICurrentTenant>();
        var objectMapper = CreateMockObjectMapper();
        var logger = CreateMockLogger<CompleteBackgroundJobEventService>();
        
        var service = new CompleteBackgroundJobEventService(
            logger,
            domainService,
            CreateMockEventBus(),
            objectMapper,
            currentTenant);

        // Act - Should not throw
        await service.HandleEventAsync(eventData);

        // Assert
        await domainService.Received(1).CompleteExecutionAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<List<BackgroundJobMessageEntity>>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>());
    }

    [Fact]
    public async Task HandleEventAsync_RethrowsNonConcurrencyExceptions()
    {
        // Arrange
        var jobId = TestConstants.JobIds.Job1;
        var executionId = TestConstants.ExecutionIds.Execution1;
        
        var eventData = new BackgroundJobExecutionCompletedMsg
        {
            BackgroundJobId = jobId,
            ExecutionId = executionId,
            Status = BackgroundJobExecutionStatus.Completed,
            TenantId = TestConstants.TenantIds.Tenant1
        };

        var domainService = CreateMockDomainService();
        domainService.CompleteExecutionAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<List<BackgroundJobMessageEntity>>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>())
            .Returns(Task.FromException<BackgroundJobExecutionEntity>(new InvalidOperationException("Test exception")));

        var currentTenant = Substitute.For<ICurrentTenant>();
        var objectMapper = CreateMockObjectMapper();
        
        var service = new CompleteBackgroundJobEventService(
            CreateMockLogger<CompleteBackgroundJobEventService>(),
            domainService,
            CreateMockEventBus(),
            objectMapper,
            currentTenant);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.HandleEventAsync(eventData));
    }
}

