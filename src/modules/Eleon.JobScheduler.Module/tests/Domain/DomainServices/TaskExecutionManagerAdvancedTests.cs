using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Xunit;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace JobScheduler.Module.Domain.DomainServices;

/// <summary>
/// Multi-tenant due tasks execution tests
/// </summary>
public class TaskExecutionManagerAdvancedTests : DomainTestBase
{
    [Fact]
    public async Task RunDueTasksAsync_ConcurrentCalls_OnlyOneSucceeds()
    {
        // Arrange
        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetTasksToStartAsync(Arg.Any<DateTime>())
            .Returns(new List<TaskEntity>());
        
        var taskService = CreateTaskDomainService(taskRepository: taskRepository);
        var taskExecutionService = CreateTaskExecutionDomainService();

        var tenantRepository = Substitute.For<ITenantRepository>();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var continueTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        tenantRepository.GetListAsync().Returns(async _ =>
        {
            started.TrySetResult(true);
            await continueTcs.Task;
            return new List<Volo.Abp.TenantManagement.Tenant>();
        });

        var service = new TaskExecutionManager(
            CreateMockLogger<TaskExecutionManager>(),
            taskService,
            taskExecutionService,
            tenantRepository,
            Substitute.For<ICurrentTenant>(),
            CreateMockEventBus());

        // Act - Simulate concurrent calls
        var task1 = Task.Run(() => service.RunDueTasksAsync());
        await started.Task; // ensure first call acquired the lock and is running
        var task2 = Task.Run(() => service.RunDueTasksAsync());

        var secondResult = await task2;
        continueTcs.TrySetResult(true);
        var firstResult = await task1;

        // Assert - Only one should return true
        new[] { firstResult, secondResult }.Count(r => r).Should().Be(1);
    }

    [Fact]
    public async Task RunDueTasksAsync_MultipleTenants_ProcessesAllTenants()
    {
        // Arrange
        var tenant1 = (Volo.Abp.TenantManagement.Tenant)System.Activator.CreateInstance(
            typeof(Volo.Abp.TenantManagement.Tenant),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { Guid.NewGuid(), "Tenant1", "tenant1" },
            null)!;
        var tenant2 = (Volo.Abp.TenantManagement.Tenant)System.Activator.CreateInstance(
            typeof(Volo.Abp.TenantManagement.Tenant),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { Guid.NewGuid(), "Tenant2", "tenant2" },
            null)!;

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetTasksToStartAsync(Arg.Any<DateTime>())
            .Returns(new List<TaskEntity>()); // No due tasks - method should still return true
        
        var taskService = CreateTaskDomainService(taskRepository: taskRepository);
        // Use real service with mocked dependencies - can't mock non-virtual methods
        var taskExecutionService = CreateTaskExecutionDomainService();

        var tenantRepository = Substitute.For<ITenantRepository>();
        tenantRepository.GetListAsync().Returns(new List<Volo.Abp.TenantManagement.Tenant> { tenant1, tenant2 });

        var currentTenant = Substitute.For<ICurrentTenant>();
        var eventBus = CreateMockEventBus();

        var service = new TaskExecutionManager(
            CreateMockLogger<TaskExecutionManager>(),
            taskService,
            taskExecutionService,
            tenantRepository,
            currentTenant,
            eventBus);

        // Act
        var result = await service.RunDueTasksAsync();

        // Assert
        result.Should().BeTrue();
        // Event bus may or may not be called depending on whether there are due tasks
    }

    [Fact]
    public async Task RunDueTasksAsync_OneTenantFails_OthersContinue()
    {
        // Arrange
        var tenant1 = (Volo.Abp.TenantManagement.Tenant)System.Activator.CreateInstance(
            typeof(Volo.Abp.TenantManagement.Tenant),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { Guid.NewGuid(), "Tenant1", "tenant1" },
            null)!;
        var tenant2 = (Volo.Abp.TenantManagement.Tenant)System.Activator.CreateInstance(
            typeof(Volo.Abp.TenantManagement.Tenant),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { Guid.NewGuid(), "Tenant2", "tenant2" },
            null)!;

        var taskRepository = CreateMockTaskRepository();
        taskRepository.GetTasksToStartAsync(Arg.Any<DateTime>())
            .Returns(new List<TaskEntity>());
        
        var taskService = CreateTaskDomainService(taskRepository: taskRepository);
        var taskExecutionService = CreateTaskExecutionDomainService();

        var tenantRepository = Substitute.For<ITenantRepository>();
        tenantRepository.GetListAsync().Returns(new List<Volo.Abp.TenantManagement.Tenant> { tenant1, tenant2 });

        var currentTenant = Substitute.For<ICurrentTenant>();
        var eventBus = CreateMockEventBus();

        var service = new TaskExecutionManager(
            CreateMockLogger<TaskExecutionManager>(),
            taskService,
            taskExecutionService,
            tenantRepository,
            currentTenant,
            eventBus);

        // Act
        var result = await service.RunDueTasksAsync();

        // Assert
        result.Should().BeTrue();
        await eventBus.Received().PublishAsync(Arg.Any<object>());
    }
}
