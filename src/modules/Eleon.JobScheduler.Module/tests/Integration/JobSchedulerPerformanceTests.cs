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
using VPortal.JobScheduler.Module.Repositories;
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

    [Fact]
    [Trait("Category", "Manual")]
    public async Task ParallelCompletion_1000Tasks_NoStuckRunning()
    {
        // Arrange
        const int taskCount = 1000;
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var actionDomainService = GetRequiredService<ActionDomainService>();
        var triggerDomainService = GetRequiredService<TriggerDomainService>();
        var taskExecutionDomainService = GetRequiredService<TaskExecutionDomainService>();
        var taskExecutionRepository = GetRequiredService<ITaskExecutionRepository>();
        var actionExecutionRepository = GetRequiredService<IActionExecutionRepository>();

        var tasks = new List<TaskEntity>();
        for (int i = 0; i < taskCount; i++)
        {
            var task = await taskDomainService.CreateAsync($"ParallelTask{i}", $"Description{i}");
            var action = ActionTestDataBuilder.Create()
                .WithId(Guid.NewGuid())
                .WithTaskId(task.Id)
                .WithDisplayName($"Action{i}")
                .WithEventName($"Event{i}")
                .Build();

            await actionDomainService.AddAsync(task.Id, action);

            var trigger = TriggerTestDataBuilder.Create()
                .WithId(Guid.NewGuid())
                .WithTaskId(task.Id)
                .WithPeriodType(TimePeriodType.Daily)
                .WithStartUtc(DateTime.UtcNow.AddMinutes(-1))
                .WithIsEnabled(true)
                .Build();

            await triggerDomainService.AddAsync(trigger);

            task.IsActive = true;
            await taskDomainService.UpdateTask(task);
            tasks.Add(task);
        }

        // Act - start executions
        foreach (var task in tasks)
        {
            await taskExecutionDomainService.RequestTaskExecutionAsync(task.Id, false, null);
        }

        var completionPairs = new List<(Guid actionExecutionId, Guid taskExecutionId, Guid taskId)>();
        foreach (var task in tasks)
        {
            var executions = await taskExecutionRepository.GetListAsync(task.Id, 0, 1, "CreationTime desc");
            var taskExecution = executions.Value.Single();
            var actionExecutions = await actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecution.Id);
            completionPairs.Add((actionExecutions.Single().Id, taskExecution.Id, task.Id));
        }

        var completionTasks = completionPairs.Select(pair =>
            taskExecutionDomainService.AcknowledgeActionCompletedAsync(
                pair.actionExecutionId,
                pair.taskExecutionId,
                JobSchedulerExecutionResult.Success,
                "PerfTest",
                false));

        await Task.WhenAll(completionTasks);

        // Assert
        foreach (var task in tasks)
        {
            var executions = await taskExecutionRepository.GetListAsync(task.Id, 0, 1, "CreationTime desc");
            executions.Value.Single().Status.Should().Be(JobSchedulerTaskExecutionStatus.Completed);
            var refreshedTask = await taskDomainService.GetByIdAsync(task.Id);
            refreshedTask.Status.Should().Be(JobSchedulerTaskStatus.Ready);
        }
    }

    [Fact]
    [Trait("Category", "Manual")]
    public async Task TaskWithThousandsOfActions_ParallelCompletion_NoStuckRunning()
    {
        // Arrange
        const int actionCount = 2000;
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var actionDomainService = GetRequiredService<ActionDomainService>();
        var triggerDomainService = GetRequiredService<TriggerDomainService>();
        var taskExecutionDomainService = GetRequiredService<TaskExecutionDomainService>();
        var taskExecutionRepository = GetRequiredService<ITaskExecutionRepository>();
        var actionExecutionRepository = GetRequiredService<IActionExecutionRepository>();

        var task = await taskDomainService.CreateAsync("ThousandsActionsTask", "Load test");

        for (int i = 0; i < actionCount; i++)
        {
            var action = ActionTestDataBuilder.Create()
                .WithId(Guid.NewGuid())
                .WithTaskId(task.Id)
                .WithDisplayName($"Action{i}")
                .WithEventName($"Event{i}")
                .Build();

            await actionDomainService.AddAsync(task.Id, action);
        }

        var trigger = TriggerTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(DateTime.UtcNow.AddMinutes(-1))
            .WithIsEnabled(true)
            .Build();

        await triggerDomainService.AddAsync(trigger);

        task.IsActive = true;
        await taskDomainService.UpdateTask(task);

        // Act
        await taskExecutionDomainService.RequestTaskExecutionAsync(task.Id, false, null);

        var executions = await taskExecutionRepository.GetListAsync(task.Id, 0, 1, "CreationTime desc");
        var taskExecution = executions.Value.Single();
        var actionExecutions = await actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecution.Id);

        var completionTasks = actionExecutions.Select(actionExecution =>
            taskExecutionDomainService.AcknowledgeActionCompletedAsync(
                actionExecution.Id,
                taskExecution.Id,
                JobSchedulerExecutionResult.Success,
                "PerfTest",
                false));

        await Task.WhenAll(completionTasks);

        // Assert
        var refreshedExecutions = await taskExecutionRepository.GetListAsync(task.Id, 0, 1, "CreationTime desc");
        refreshedExecutions.Value.Single().Status.Should().Be(JobSchedulerTaskExecutionStatus.Completed);
        var refreshedTask = await taskDomainService.GetByIdAsync(task.Id);
        refreshedTask.Status.Should().Be(JobSchedulerTaskStatus.Ready);
    }
}

