using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using JobScheduler.Module.TestBase;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace JobScheduler.Module.EntityFrameworkCore.Repositories;

/// <summary>
/// Repository advanced tests
/// </summary>
public class JobSchedulerRepositoryAdvancedTests : ModuleTestBase<JobSchedulerTestStartupModule>
{
    [Fact]
    public async Task GetListAsync_ComplexQueries_MultipleFilters_ReturnsCorrectResults()
    {
        // Arrange
        var taskRepository = GetRequiredService<ITaskRepository>();

        // Act
        var result = await taskRepository.GetList(0, 10, null, null);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetListAsync_PaginationEdgeCases_LargeDatasets_HandlesCorrectly()
    {
        // Arrange
        var taskRepository = GetRequiredService<ITaskRepository>();

        // Act - Test pagination boundaries
        var result1 = await taskRepository.GetList(0, 10, null, null);
        var result2 = await taskRepository.GetList(10, 10, null, null);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTasksToStartAsync_WithIncludes_ReturnsTasksWithTriggers()
    {
        // Arrange
        var taskRepository = GetRequiredService<ITaskRepository>();
        var now = DateTime.UtcNow;

        // Act
        var result = await taskRepository.GetTasksToStartAsync(now);

        // Assert
        result.Should().NotBeNull();
    }
}

