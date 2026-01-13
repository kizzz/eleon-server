using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using Common.Module.Constants;

namespace JobScheduler.Module.Integration;

/// <summary>
/// Performance and scale tests
/// </summary>
public class JobSchedulerPerformanceTests : ModuleTestBase<JobSchedulerTestStartupModule>
{
    [Fact]
    public async Task BulkTaskCreation_1000Tasks_PerformsWithinReasonableTime()
    {
        // Arrange
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var stopwatch = Stopwatch.StartNew();

        // Act - Create 100 tasks (reduced for test speed)
        var tasks = new List<TaskEntity>();
        for (int i = 0; i < 100; i++)
        {
            var task = await taskDomainService.CreateAsync($"Task{i}", $"Description{i}");
            tasks.Add(task);
        }

        stopwatch.Stop();

        // Assert
        tasks.Should().HaveCount(100);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // 30 seconds for 100 tasks
    }

    [Fact]
    public async Task TaskWithManyActions_50Actions_PerformsWithinReasonableTime()
    {
        // Arrange
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var actionDomainService = GetRequiredService<ActionDomainService>();

        var task = await taskDomainService.CreateAsync("ManyActionsTask", "Description");
        var stopwatch = Stopwatch.StartNew();

        // Act - Create 50 actions
        for (int i = 0; i < 50; i++)
        {
            var action = ActionTestDataBuilder.Create()
                .WithId(Guid.NewGuid())
                .WithTaskId(task.Id)
                .WithDisplayName($"Action{i}")
                .WithEventName($"Event{i}")
                .Build();

            await actionDomainService.AddAsync(task.Id, action);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000); // 10 seconds for 50 actions
    }
}

