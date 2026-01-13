using System;
using Common.Module.Constants;
using FluentAssertions;
using Xunit;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Domain.Entities;

public class BackgroundJobEntityTests
{
    [Fact]
    public void Constructor_WithId_SetsIdAndInitializesExecutions()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var job = new BackgroundJobEntity(id);

        // Assert
        job.Id.Should().Be(id);
        job.Executions.Should().NotBeNull();
        job.Executions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_Default_InitializesExecutions()
    {
        // Act
        var job = new BackgroundJobEntity();

        // Assert
        job.Executions.Should().NotBeNull();
        job.Executions.Should().BeEmpty();
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithId(TestConstants.JobIds.Job1)
            .WithType(TestConstants.JobTypes.TestJob)
            .Build();

        // Act
        var result = job.ToString();

        // Assert
        result.Should().Be($"[Job #{TestConstants.JobIds.Job1}, {TestConstants.JobTypes.TestJob}]");
    }

    [Theory]
    [InlineData(BackgroundJobStatus.New, BackgroundJobStatus.Executing, true)]
    [InlineData(BackgroundJobStatus.New, BackgroundJobStatus.Completed, true)]
    [InlineData(BackgroundJobStatus.New, BackgroundJobStatus.Errored, true)]
    [InlineData(BackgroundJobStatus.New, BackgroundJobStatus.Retring, true)]
    [InlineData(BackgroundJobStatus.New, BackgroundJobStatus.Cancelled, true)]
    [InlineData(BackgroundJobStatus.Executing, BackgroundJobStatus.Completed, true)]
    [InlineData(BackgroundJobStatus.Executing, BackgroundJobStatus.Errored, true)]
    [InlineData(BackgroundJobStatus.Executing, BackgroundJobStatus.Cancelled, true)]
    [InlineData(BackgroundJobStatus.Retring, BackgroundJobStatus.Completed, true)]
    [InlineData(BackgroundJobStatus.Retring, BackgroundJobStatus.Errored, true)]
    [InlineData(BackgroundJobStatus.Retring, BackgroundJobStatus.Cancelled, true)]
    public void UpdateStatus_ValidTransitions_UpdatesStatus(
        BackgroundJobStatus initialStatus,
        BackgroundJobStatus newStatus,
        bool shouldUpdate)
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithStatus(initialStatus)
            .Build();

        // Act
        job.UpdateStatus(newStatus);

        // Assert
        if (shouldUpdate)
        {
            job.Status.Should().Be(newStatus);
        }
    }

    [Theory]
    [InlineData(BackgroundJobStatus.Completed, BackgroundJobStatus.Executing)]
    [InlineData(BackgroundJobStatus.Completed, BackgroundJobStatus.New)]
    [InlineData(BackgroundJobStatus.Cancelled, BackgroundJobStatus.Executing)]
    [InlineData(BackgroundJobStatus.Cancelled, BackgroundJobStatus.New)]
    [InlineData(BackgroundJobStatus.Errored, BackgroundJobStatus.New)]
    public void UpdateStatus_FromFinalStatus_PreventsUpdate(
        BackgroundJobStatus initialStatus,
        BackgroundJobStatus newStatus)
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithStatus(initialStatus)
            .Build();

        // Act
        job.UpdateStatus(newStatus);

        // Assert
        job.Status.Should().Be(initialStatus);
    }

    [Fact]
    public void UpdateStatus_ToNewStatus_PreventsUpdate()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithStatus(BackgroundJobStatus.Executing)
            .Build();

        // Act
        job.UpdateStatus(BackgroundJobStatus.New);

        // Assert
        job.Status.Should().Be(BackgroundJobStatus.Executing);
    }

    [Fact]
    public void UpdateStatus_FromRetringToExecuting_PreventsUpdate()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithStatus(BackgroundJobStatus.Retring)
            .Build();

        // Act
        job.UpdateStatus(BackgroundJobStatus.Executing);

        // Assert
        job.Status.Should().Be(BackgroundJobStatus.Retring);
    }

    [Fact]
    public void UpdateStatus_FromExecutingToRetring_AllowsUpdate()
    {
        // Arrange
        var job = BackgroundJobTestDataBuilder.Create()
            .WithStatus(BackgroundJobStatus.Executing)
            .Build();

        // Act
        job.UpdateStatus(BackgroundJobStatus.Retring);

        // Assert
        job.Status.Should().Be(BackgroundJobStatus.Retring);
    }
}

