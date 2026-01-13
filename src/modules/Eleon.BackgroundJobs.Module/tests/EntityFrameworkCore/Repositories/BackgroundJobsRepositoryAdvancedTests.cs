using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.EntityFrameworkCore.Repositories;

/// <summary>
/// Advanced repository scenario tests
/// </summary>
public class BackgroundJobsRepositoryAdvancedTests : ModuleTestBase<BackgroundJobsTestStartupModule>
{
    private IBackgroundJobsRepository GetRepository()
    {
        return ServiceProvider.GetRequiredService<IBackgroundJobsRepository>();
    }

    [Fact]
    public async Task GetListAsync_ComplexQueryWithMultipleFilters_ReturnsCorrectResults()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var now = DateTime.UtcNow;

            // Create jobs with various properties
            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job1)
                .WithType(TestConstants.JobTypes.TestJob)
                .WithStatus(BackgroundJobStatus.New)
                .WithScheduleDate(now.AddMinutes(-5))
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job2)
                .WithType(TestConstants.JobTypes.SystemJob)
                .WithStatus(BackgroundJobStatus.Executing)
                .WithScheduleDate(now.AddMinutes(-10))
                .Build();

            var job3 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job3)
                .WithType(TestConstants.JobTypes.TestJob)
                .WithStatus(BackgroundJobStatus.Completed)
                .WithScheduleDate(now.AddMinutes(-15))
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);
            await repository.InsertAsync(job3, autoSave: true);

            // Act - Complex query with type and status filters
            var result = await repository.GetListAsync(
                sorting: "CreationTime DESC",
                maxResultCount: 10,
                skipCount: 0,
                searchQuery: null,
                creationDateFilterStart: null,
                creationDateFilterEnd: null,
                lastExecutionDateFilterStart: null,
                lastExecutionDateFilterEnd: null,
                typeFilter: new List<string> { TestConstants.JobTypes.TestJob },
                statusFilter: new List<BackgroundJobStatus> { BackgroundJobStatus.New, BackgroundJobStatus.Completed });

            // Assert
            result.Value.Should().HaveCount(2);
            result.Value.Should().OnlyContain(j => j.Type == TestConstants.JobTypes.TestJob);
            result.Value.Should().OnlyContain(j => j.Status == BackgroundJobStatus.New || j.Status == BackgroundJobStatus.Completed);
        });
    }

    [Fact]
    public async Task GetListAsync_PaginationEdgeCases_HandlesCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();

            // Create 15 jobs
            for (int i = 0; i < 15; i++)
            {
                var job = BackgroundJobTestDataBuilder.Create()
                    .WithId(Guid.NewGuid())
                    .WithType(TestConstants.JobTypes.TestJob)
                    .Build();
                await repository.InsertAsync(job, autoSave: true);
            }

            // Act - First page
            var page1 = await repository.GetListAsync(
                sorting: "CreationTime ASC",
                maxResultCount: 10,
                skipCount: 0);

            // Act - Second page
            var page2 = await repository.GetListAsync(
                sorting: "CreationTime ASC",
                maxResultCount: 10,
                skipCount: 10);

            // Assert
            page1.Value.Should().HaveCount(10);
            page2.Value.Should().HaveCount(5);
            page1.Key.Should().Be(15);
            page2.Key.Should().Be(15);
            // No overlap
            page1.Value.Select(j => j.Id).Should().NotIntersectWith(page2.Value.Select(j => j.Id));
        });
    }

    [Fact]
    public async Task GetListAsync_EmptyResultSet_ReturnsEmpty()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();

            // Act - Query with filter that matches nothing
            var result = await repository.GetListAsync(
                sorting: "CreationTime DESC",
                maxResultCount: 10,
                skipCount: 0,
                searchQuery: null,
                creationDateFilterStart: null,
                creationDateFilterEnd: null,
                lastExecutionDateFilterStart: null,
                lastExecutionDateFilterEnd: null,
                typeFilter: new List<string> { "NonExistentType" },
                statusFilter: null);

            // Assert
            result.Value.Should().BeEmpty();
            result.Key.Should().Be(0);
        });
    }

    [Fact]
    public async Task GetListAsync_DateRangeFilters_ReturnsCorrectResults()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var now = DateTime.UtcNow;
            var startDate = now.AddDays(-5);
            var endDate = now.AddDays(-1);

            // Create jobs with different creation dates
            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job1)
                .Build();
            job1.CreationTime = now.AddDays(-3); // Within range

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job2)
                .Build();
            job2.CreationTime = now.AddDays(-7); // Outside range

            var job3 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job3)
                .Build();
            job3.CreationTime = now.AddDays(-2); // Within range

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);
            await repository.InsertAsync(job3, autoSave: true);

            // Act
            var result = await repository.GetListAsync(
                sorting: "CreationTime DESC",
                maxResultCount: 10,
                skipCount: 0,
                searchQuery: null,
                creationDateFilterStart: startDate,
                creationDateFilterEnd: endDate,
                lastExecutionDateFilterStart: null,
                lastExecutionDateFilterEnd: null,
                typeFilter: null,
                statusFilter: null);

            // Assert
            result.Value.Should().HaveCount(2);
            result.Value.Should().OnlyContain(j => j.CreationTime >= startDate && j.CreationTime <= endDate);
        });
    }

    [Fact]
    public async Task GetLongTimeExecutingJobIdsAsync_ReturnsJobsExceedingTimeout()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var now = DateTime.UtcNow;

            var execution1 = BackgroundJobExecutionTestDataBuilder.Create()
                .WithStatus(BackgroundJobExecutionStatus.Started)
                .WithExecutionStartTime(now.AddHours(-2)) // Started 2 hours ago
                .Build();

            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job1)
                .WithStatus(BackgroundJobStatus.Executing)
                .WithTimeout(60) // 60 minute timeout
                .WithExecution(execution1)
                .Build();

            var execution2 = BackgroundJobExecutionTestDataBuilder.Create()
                .WithStatus(BackgroundJobExecutionStatus.Started)
                .WithExecutionStartTime(now.AddMinutes(-30)) // Started 30 minutes ago
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job2)
                .WithStatus(BackgroundJobStatus.Executing)
                .WithTimeout(60) // 60 minute timeout
                .WithExecution(execution2)
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);

            // Act
            var result = await repository.GetLongTimeExecutingJobIdsAsync();

            // Assert
            result.Should().Contain(j => j.Id == job1.Id);
            result.Should().NotContain(j => j.Id == job2.Id);
            result.First(j => j.Id == job1.Id).TimeoutInMinutes.Should().Be(60);
        });
    }

    [Fact]
    public async Task GetListAsync_WithParentChildRelationships_QueriesCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();

            var parentJob = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job1)
                .Build();

            var childJob = BackgroundJobTestDataBuilder.Create()
                .WithId(TestConstants.JobIds.Job2)
                .WithParentJobId(TestConstants.JobIds.Job1)
                .Build();

            await repository.InsertAsync(parentJob, autoSave: true);
            await repository.InsertAsync(childJob, autoSave: true);

            // Act - Query all jobs
            var result = await repository.GetListAsync(
                sorting: "CreationTime DESC",
                maxResultCount: 10,
                skipCount: 0);

            // Assert
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(j => j.Id == parentJob.Id);
            result.Value.Should().Contain(j => j.Id == childJob.Id && j.ParentJobId == parentJob.Id);
        });
    }

    [Fact]
    public async Task ConcurrentRepositoryOperations_MultipleQueriesSimultaneously_HandlesCorrectly()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();

            // Create jobs
            for (int i = 0; i < 50; i++)
            {
                var job = BackgroundJobTestDataBuilder.Create()
                    .WithId(Guid.NewGuid())
                    .WithType(TestConstants.JobTypes.TestJob)
                    .Build();
                await repository.InsertAsync(job, autoSave: true);
            }

            // Act - Concurrent queries
            var tasks = new List<Task<KeyValuePair<long, List<BackgroundJobEntity>>>>
            {
                repository.GetListAsync("CreationTime DESC", 10, 0),
                repository.GetListAsync("CreationTime ASC", 10, 0),
                repository.GetListAsync("CreationTime DESC", 20, 0),
                repository.GetByType(TestConstants.JobTypes.TestJob)
                    .ContinueWith(t => new KeyValuePair<long, List<BackgroundJobEntity>>(t.Result.Count, t.Result))
            };

            var results = await Task.WhenAll(tasks);

            // Assert - All queries should complete successfully
            results.Should().AllSatisfy(r => r.Value.Should().NotBeNull());
        });
    }
}

