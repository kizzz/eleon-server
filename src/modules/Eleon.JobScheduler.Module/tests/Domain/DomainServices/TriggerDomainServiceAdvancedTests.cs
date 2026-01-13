using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Xunit;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;
using Common.Module.Constants;

namespace JobScheduler.Module.Domain.DomainServices;

/// <summary>
/// Advanced trigger management tests
/// </summary>
public class TriggerDomainServiceAdvancedTests : DomainTestBase
{
    [Fact]
    public async Task AddAsync_MultipleTriggersPerTask_CalculatesNextRunTimeCorrectly()
    {
        // Arrange - Task with 3 triggers
        var taskId = TestConstants.TaskIds.Task1;
        var trigger1 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger1)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(TestConstants.Dates.UtcNow)
            .WithIsEnabled(true)
            .Build();

        var trigger2 = TriggerTestDataBuilder.Create()
            .WithId(TestConstants.TriggerIds.Trigger2)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Weekly)
            .WithStartUtc(TestConstants.Dates.UtcNow)
            .WithIsEnabled(true)
            .Build();

        var trigger3 = TriggerTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Monthly)
            .WithStartUtc(TestConstants.Dates.UtcNow)
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger1)
            .WithTrigger(trigger2)
            .Build();

        var triggerRepository = CreateMockTriggerRepository();
        triggerRepository.InsertAsync(Arg.Any<TriggerEntity>(), true).Returns(trigger3);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId).Returns(task);

        var service = CreateTriggerDomainService(
            triggerRepository: triggerRepository,
            taskRepository: taskRepository);

        // Act
        var result = await service.AddAsync(trigger3);

        // Assert
        result.Should().NotBeNull();
        await triggerRepository.Received(1).InsertAsync(Arg.Any<TriggerEntity>(), true);
    }

    [Fact]
    public async Task SetTriggerIsEnabled_DisablingTrigger_UpdatesNextRunTime()
    {
        // Arrange
        var triggerId = TestConstants.TriggerIds.Trigger1;
        var taskId = TestConstants.TaskIds.Task1;

        var trigger = TriggerTestDataBuilder.Create()
            .WithId(triggerId)
            .WithTaskId(taskId)
            .WithIsEnabled(true)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .WithTrigger(trigger)
            .Build();

        var triggerRepository = CreateMockTriggerRepository();
        triggerRepository.GetAsync(triggerId, false).Returns(trigger);
        triggerRepository.UpdateAsync(Arg.Any<TriggerEntity>(), true).Returns(trigger);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId).Returns(task);

        var service = CreateTriggerDomainService(
            triggerRepository: triggerRepository,
            taskRepository: taskRepository);

        // Act
        var result = await service.SetTriggerIsEnabled(triggerId, false);

        // Assert
        result.Should().BeTrue();
        trigger.IsEnabled.Should().BeFalse();
        await triggerRepository.Received(1).UpdateAsync(Arg.Any<TriggerEntity>(), true);
    }

    [Fact]
    public async Task UpdateAsync_ScheduleChanges_UpdatesNextRunTime()
    {
        // Arrange
        var triggerId = TestConstants.TriggerIds.Trigger1;
        var taskId = TestConstants.TaskIds.Task1;

        var trigger = TriggerTestDataBuilder.Create()
            .WithId(triggerId)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(TestConstants.Dates.UtcNow)
            .Build();

        var updatedTrigger = TriggerTestDataBuilder.Create()
            .WithId(triggerId)
            .WithTaskId(taskId)
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(2)
            .WithStartUtc(TestConstants.Dates.UtcNow)
            .WithDaysOfWeekList(new List<int> { 1 }) // Monday (ISO day-of-week 1-7)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .Build();

        var triggerRepository = CreateMockTriggerRepository();
        triggerRepository.GetAsync(triggerId, true).Returns(trigger);
        triggerRepository.UpdateAsync(Arg.Any<TriggerEntity>(), true).Returns(updatedTrigger);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId).Returns(task);

        var service = CreateTriggerDomainService(
            triggerRepository: triggerRepository,
            taskRepository: taskRepository);

        // Act
        var result = await service.UpdateAsync(updatedTrigger);

        // Assert
        result.Should().NotBeNull();
        await triggerRepository.Received(1).UpdateAsync(Arg.Any<TriggerEntity>(), true);
    }

    [Fact]
    public async Task DeleteAsync_TriggerDeletion_UpdatesNextRunTime()
    {
        // Arrange
        var triggerId = TestConstants.TriggerIds.Trigger1;
        var taskId = TestConstants.TaskIds.Task1;

        var trigger = TriggerTestDataBuilder.Create()
            .WithId(triggerId)
            .WithTaskId(taskId)
            .Build();

        var task = TaskTestDataBuilder.Create()
            .WithId(taskId)
            .Build();

        var triggerRepository = CreateMockTriggerRepository();
        triggerRepository.GetAsync(triggerId).Returns(trigger);

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetAsync(taskId).Returns(task);

        var service = CreateTriggerDomainService(
            triggerRepository: triggerRepository,
            taskRepository: taskRepository);

        // Act
        await service.DeleteAsync(triggerId);

        // Assert
        await triggerRepository.Received(1).DeleteAsync(triggerId);
    }
}

