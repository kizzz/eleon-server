using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using Eleonsoft.Host.Test.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;
using Xunit;
using Xunit.Sdk;

namespace Eleonsoft.Host.Test.Integration;

/// <summary>
/// Optional real database integration tests for JobScheduler -> BackgroundJobs event bus flow.
/// </summary>
public class BackgroundJobsJobSchedulerRealDbIntegrationTests : CrossModuleRealDbTestBase
{
    private const string DefaultActionParams = "{}";
    private const string DefaultEventName = "JobScheduler.IntegrationTest.RealDb";

    [Fact]
    public async Task RealDb_EventBus_Integration_Works_ForSmallLoad()
    {
        var configuration = GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new SkipException("Set ELEON_TEST_REAL_DB_CONNECTION_STRING to enable real DB integration tests.");
        }

        const int taskCount = 3;
        var tasks = new List<TaskEntity>();

        await WithUnitOfWorkAsync(async () =>
        {
            for (var i = 0; i < taskCount; i++)
            {
                tasks.Add(await CreateActiveTaskAsync($"RealDbTask-{i}", DefaultEventName));
            }
        });

        await WithUnitOfWorkAsync(RunDueTasksAsync);

        var actionExecutions = await WithUnitOfWorkAsync(async () =>
        {
            var executions = new List<ActionExecutionEntity>();
            foreach (var task in tasks)
            {
                var taskExecutions = await GetTaskExecutionsAsync(task.Id);
                taskExecutions.Should().ContainSingle();
                executions.AddRange(taskExecutions.Single().ActionExecutions);
            }

            return executions;
        });

        var jobIds = actionExecutions.Select(x => x.JobId!.Value).ToList();
        await WaitForBackgroundJobsAsync(jobIds);

        foreach (var jobId in jobIds)
        {
            await WithUnitOfWorkAsync(async () => await CompleteBackgroundJobAsync(jobId));
        }

        await WithUnitOfWorkAsync(async () =>
        {
            var actionExecutionRepository = GetRequiredService<IActionExecutionRepository>();
            foreach (var execution in actionExecutions)
            {
                var refreshed = await actionExecutionRepository.GetAsync(execution.Id);
                refreshed.Status.Should().Be(JobSchedulerActionExecutionStatus.Completed);
            }
        });
    }

    private async Task<TaskEntity> CreateActiveTaskAsync(string name, string eventName)
    {
        var taskService = GetRequiredService<TaskDomainService>();
        var actionService = GetRequiredService<ActionDomainService>();
        var triggerService = GetRequiredService<TriggerDomainService>();

        var task = await taskService.CreateAsync(name, $"Integration test task {name}");

        var action = new ActionEntity(Guid.NewGuid())
        {
            TaskId = task.Id,
            DisplayName = $"{name}-Action",
            EventName = eventName,
            ActionParams = DefaultActionParams,
            ActionExtraParams = string.Empty,
            ParamsFormat = TextFormat.Json,
            RetryInterval = TimeSpan.FromMinutes(1),
            MaxRetryAttempts = 1,
            TimeoutInMinutes = 5,
            OnFailureRecepients = string.Empty,
        };

        await actionService.AddAsync(task.Id, action);

        var trigger = new TriggerEntity(Guid.NewGuid())
        {
            TaskId = task.Id,
            Name = $"{name}-Trigger",
            IsEnabled = true,
            StartUtc = DateTime.UtcNow.AddMinutes(-1),
            PeriodType = TimePeriodType.Daily,
            Period = 1,
            RepeatTask = false,
        };

        await triggerService.AddAsync(trigger);

        task.IsActive = true;
        await taskService.UpdateTask(task);

        return task;
    }

    private async Task<IReadOnlyList<TaskExecutionEntity>> GetTaskExecutionsAsync(Guid taskId)
    {
        var taskExecutionRepository = GetRequiredService<ITaskExecutionRepository>();
        var result = await taskExecutionRepository.GetListAsync(taskId, 0, int.MaxValue, "CreationTime desc");
        return result.Value;
    }

    private async Task RunDueTasksAsync()
    {
        var manager = GetRequiredService<TaskExecutionManager>();
        var result = await manager.RunDueTasksAsync();
        result.Should().BeTrue();
    }

    private async Task WaitForBackgroundJobsAsync(IReadOnlyCollection<Guid> jobIds)
    {
        var pending = new HashSet<Guid>(jobIds);
        var timeoutAt = DateTime.UtcNow.AddSeconds(10);

        while (pending.Count > 0 && DateTime.UtcNow < timeoutAt)
        {
            var foundIds = await WithUnitOfWorkAsync(async () =>
            {
                var jobRepository = GetRequiredService<IBackgroundJobsRepository>();
                var found = new List<Guid>();

                foreach (var jobId in pending)
                {
                    var job = await jobRepository.FindAsync(jobId, includeDetails: true);
                    if (job != null)
                    {
                        found.Add(jobId);
                    }
                }

                return found;
            });

            foreach (var jobId in foundIds)
            {
                pending.Remove(jobId);
            }

            if (pending.Count > 0)
            {
                await Task.Delay(200);
            }
        }

        pending.Should().BeEmpty("background jobs should be created by event bus");
    }

    private async Task CompleteBackgroundJobAsync(Guid jobId)
    {
        var domainService = GetRequiredService<BackgroundJobDomainService>();
        var execution = await domainService.StartExecutionAsync(jobId);
        await domainService.MarkExecutionStartedAsync(jobId, execution.Id);
        await domainService.CompleteExecutionAsync(
            jobId,
            execution.Id,
            successfully: true,
            retryExecutionParams: null,
            retryExecutionExtraParams: null,
            messages: new List<BackgroundJobMessageEntity>(),
            result: "OK",
            completedBy: "IntegrationTest",
            manually: false);
    }
}
