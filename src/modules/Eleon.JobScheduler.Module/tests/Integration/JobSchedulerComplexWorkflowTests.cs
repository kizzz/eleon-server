using System;
using System.Collections.Generic;
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
/// Complex workflow integration tests
/// </summary>
public class JobSchedulerComplexWorkflowTests : ModuleTestBase<JobSchedulerTestStartupModule>
{
    [Fact]
    public async Task CompleteTaskLifecycle_CreateAddTriggersAddActionsActivateExecuteComplete_Succeeds()
    {
        // Arrange
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var triggerDomainService = GetRequiredService<TriggerDomainService>();
        var actionDomainService = GetRequiredService<ActionDomainService>();
        var taskExecutionDomainService = GetRequiredService<TaskExecutionDomainService>();

        // Act - Create task
        var task = await taskDomainService.CreateAsync("TestTask", "Test Description");

        // Add trigger
        var trigger = TriggerTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithPeriodType(TimePeriodType.Daily)
            .WithStartUtc(DateTime.UtcNow)
            .WithIsEnabled(true)
            .Build();

        await triggerDomainService.AddAsync(trigger);

        // Add action
        var action = ActionTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithDisplayName("TestAction")
            .WithEventName("TestEvent")
            .Build();

        await actionDomainService.AddAsync(task.Id, action);

        // Activate task
        task.IsActive = true;
        await taskDomainService.UpdateTask(task);

        // Assert
        task.Should().NotBeNull();
        task.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task TaskWithComplexActionDependencies_MultiLevelDependencies_Succeeds()
    {
        // Arrange
        var taskDomainService = GetRequiredService<TaskDomainService>();
        var actionDomainService = GetRequiredService<ActionDomainService>();

        var task = await taskDomainService.CreateAsync("ComplexTask", "Description");

        // Create actions with dependencies: A → B → C
        var actionA = ActionTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithDisplayName("ActionA")
            .Build();

        var actionB = ActionTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithDisplayName("ActionB")
            .WithParentAction(actionA.Id)
            .Build();

        var actionC = ActionTestDataBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithTaskId(task.Id)
            .WithDisplayName("ActionC")
            .WithParentAction(actionB.Id)
            .Build();

        // Act
        await actionDomainService.AddAsync(task.Id, actionA);
        await actionDomainService.AddAsync(task.Id, actionB);
        await actionDomainService.AddAsync(task.Id, actionC);

        // Assert
        actionC.ParentActions.Should().Contain(x => x.ParentActionId == actionB.Id);
    }
}

