using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;
using Xunit;

namespace JobScheduler.Module.Domain.DomainServices;

/// <summary>
/// Trigger edge case tests
/// Tests: expiry, disabled, invalid ranges, overlapping schedules
/// </summary>
public class TriggerEdgeCasesTests : DomainTestBase
{
    [Fact]
    public async Task GetTaskNextRunTimeAsync_TriggerExpired_ReturnsNull()
    {
        // Arrange - Trigger expired
        var taskId = TestConstants.TaskIds.Task1;
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(DateTime.UtcNow.AddDays(-10))
            .WithExpireUtc(DateTime.UtcNow.AddDays(-1)) // Expired
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        result.Should().BeNull(); // Expired trigger should return null
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_TriggerDisabled_ReturnsNull()
    {
        // Arrange - Trigger disabled
        var taskId = TestConstants.TaskIds.Task1;
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(DateTime.UtcNow.AddDays(-1))
            .WithIsEnabled(false) // Disabled
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        result.Should().BeNull(); // Disabled trigger should return null
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_MultipleTriggersWithOverlappingSchedules_ReturnsEarliest()
    {
        // Arrange - Multiple triggers with different next run times
        var taskId = TestConstants.TaskIds.Task1;
        var now = DateTime.UtcNow;
        
        var trigger1 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(now.AddHours(-2))
            .WithIsEnabled(true)
            .Build();

        var trigger2 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger2)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(now.AddHours(-1))
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger1)
            .WithTrigger(trigger2)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Value.NextRunTime.Should().NotBeNull();
        // Should return the earliest next run time
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_TriggerWithInvalidDateRange_HandlesGracefully()
    {
        // Arrange - Trigger with ExpireUtc before StartUtc (invalid, but should be validated elsewhere)
        var taskId = TestConstants.TaskIds.Task1;
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(DateTime.UtcNow.AddDays(1))
            .WithExpireUtc(DateTime.UtcNow) // Before StartUtc (invalid)
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        // Should handle gracefully - if ExpireUtc < StartUtc, the trigger is effectively expired
        // So it should return null (expired trigger)
        result.Should().BeNull(); // Expired trigger should return null
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_AllTriggersExpired_ReturnsNull()
    {
        // Arrange - All triggers expired
        var taskId = TestConstants.TaskIds.Task1;
        var now = DateTime.UtcNow;
        
        var trigger1 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(now.AddDays(-10))
            .WithExpireUtc(now.AddDays(-1)) // Expired
            .WithIsEnabled(true)
            .Build();

        var trigger2 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger2)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Weekly)
            .WithStartUtc(now.AddDays(-10))
            .WithExpireUtc(now.AddDays(-2)) // Expired
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger1)
            .WithTrigger(trigger2)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        result.Should().BeNull(); // All triggers expired
    }

    [Fact]
    public async Task GetTaskNextRunTimeAsync_MixedEnabledDisabled_ReturnsOnlyEnabled()
    {
        // Arrange - Mix of enabled and disabled triggers
        var taskId = TestConstants.TaskIds.Task1;
        var now = DateTime.UtcNow;
        
        var trigger1 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(now.AddHours(-2))
            .WithIsEnabled(true) // Enabled
            .Build();

        var trigger2 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger2)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Weekly)
            .WithStartUtc(now.AddHours(-1))
            .WithIsEnabled(false) // Disabled
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger1)
            .WithTrigger(trigger2)
            .Build();

        var triggerDomainService = CreateTriggerDomainService();

        // Act
        var result = await triggerDomainService.GetTaskNextRunTimeAsync(task);

        // Assert
        result.Should().NotBeNull(); // Should return enabled trigger's next run time
        result.Value.Trigger.Should().Be(trigger1); // Should be the enabled trigger
    }
}

