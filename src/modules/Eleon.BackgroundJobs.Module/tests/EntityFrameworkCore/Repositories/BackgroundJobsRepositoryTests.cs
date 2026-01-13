using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Xunit;
using BackgroundJobs.Module.TestBase;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.EntityFrameworkCore.Repositories;

/// <summary>
/// Integration tests for BackgroundJobsRepository using EF Core with in-memory database
/// </summary>
public class BackgroundJobsRepositoryTests : ModuleTestBase<BackgroundJobsTestStartupModule>
{
    private IBackgroundJobsRepository GetRepository()
    {
        return ServiceProvider.GetRequiredService<IBackgroundJobsRepository>();
    }

    [Fact]
    public async Task GetByDateTime_ReturnsJobsWithNewStatusAndPastScheduleDate()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var now = DateTime.UtcNow;
            var pastDate = now.AddMinutes(-10);
            var futureDate = now.AddMinutes(10);

            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithStatus(BackgroundJobStatus.New)
                .WithScheduleDate(pastDate)
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithStatus(BackgroundJobStatus.New)
                .WithScheduleDate(futureDate)
                .Build();

            var job3 = BackgroundJobTestDataBuilder.Create()
                .WithStatus(BackgroundJobStatus.Executing)
                .WithScheduleDate(pastDate)
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);
            await repository.InsertAsync(job3, autoSave: true);

            // Act
            var result = await repository.GetByDateTime(now);

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(job1.Id);
            result[0].Status.Should().Be(BackgroundJobStatus.New);
            result[0].ScheduleExecutionDateUtc.Should().BeBefore(now);
        });
    }

    [Fact]
    public async Task GetRetryJobsAsync_ReturnsJobsReadyForRetry()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var now = DateTime.UtcNow;
            var pastRetryTime = now.AddMinutes(-10);
            var futureRetryTime = now.AddMinutes(10);

            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithNextRetryTime(pastRetryTime)
                .WithCurrentRetryAttempt(1)
                .WithRetrySettings(3, 5)
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithNextRetryTime(futureRetryTime)
                .WithCurrentRetryAttempt(1)
                .WithRetrySettings(3, 5)
                .Build();

            var job3 = BackgroundJobTestDataBuilder.Create()
                .WithNextRetryTime(pastRetryTime)
                .WithCurrentRetryAttempt(3) // Max reached
                .WithRetrySettings(3, 5)
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);
            await repository.InsertAsync(job3, autoSave: true);

            // Act
            var result = await repository.GetRetryJobsAsync(now);

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(job1.Id);
            result[0].NextRetryTimeUtc.Should().BeBefore(now);
            result[0].CurrentRetryAttempt.Should().BeLessThan(result[0].MaxRetryAttempts);
        });
    }

    [Fact]
    public async Task GetByType_ReturnsJobsFilteredByType()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var type1 = TestConstants.JobTypes.TestJob;
            var type2 = TestConstants.JobTypes.SystemJob;

            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithType(type1)
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithType(type1)
                .Build();

            var job3 = BackgroundJobTestDataBuilder.Create()
                .WithType(type2)
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);
            await repository.InsertAsync(job3, autoSave: true);

            // Act
            var result = await repository.GetByType(type1);

            // Assert
            result.Should().HaveCount(2);
            result.All(j => j.Type == type1).Should().BeTrue();
        });
    }

    [Fact]
    public async Task GetListAsync_AppliesAllFilters()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var type = TestConstants.JobTypes.TestJob;
            var status = BackgroundJobStatus.New;

            var job1 = BackgroundJobTestDataBuilder.Create()
                .WithType(type)
                .WithStatus(status)
                .WithDescription("Test job description")
                .Build();

            var job2 = BackgroundJobTestDataBuilder.Create()
                .WithType(type)
                .WithStatus(BackgroundJobStatus.Executing)
                .Build();

            await repository.InsertAsync(job1, autoSave: true);
            await repository.InsertAsync(job2, autoSave: true);

            // Act
            var result = await repository.GetListAsync(
                sorting: "CreationTime DESC",
                maxResultCount: 10,
                skipCount: 0,
                searchQuery: "description",
                typeFilter: new List<string> { type },
                statusFilter: new List<BackgroundJobStatus> { status });

            // Assert
            result.Key.Should().Be(1);
            result.Value.Should().HaveCount(1);
            result.Value[0].Id.Should().Be(job1.Id);
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
            var timeoutStart = now.AddMinutes(-61); // 61 minutes ago, timeout is 60

            var execution = BackgroundJobExecutionTestDataBuilder.Create()
                .WithStatus(BackgroundJobExecutionStatus.Started)
                .WithExecutionStartTime(timeoutStart)
                .Build();

            var job = BackgroundJobTestDataBuilder.Create()
                .WithStatus(BackgroundJobStatus.Executing)
                .WithTimeout(60)
                .WithExecution(execution)
                .Build();

            await repository.InsertAsync(job, autoSave: true);

            // Act
            var result = await repository.GetLongTimeExecutingJobIdsAsync();

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(job.Id);
            result[0].TimeoutInMinutes.Should().Be(60);
        });
    }

    [Fact]
    public async Task WithDetailsAsync_IncludesExecutionsAndMessages()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var repository = GetRepository();
            var execution = BackgroundJobExecutionTestDataBuilder.Create()
                .WithMessage(BackgroundJobMessageType.Info, "Test message")
                .Build();

            var job = BackgroundJobTestDataBuilder.Create()
                .WithExecution(execution)
                .Build();

            await repository.InsertAsync(job, autoSave: true);

            // Act - Use GetAsync with includeDetails: true to get executions and messages
            var result = await repository.GetAsync(job.Id, includeDetails: true);

            // Assert
            result.Should().NotBeNull();
            result.Executions.Should().HaveCount(1);
            result.Executions[0].Messages.Should().HaveCount(1);
        });
    }
}

