using System;
using Common.Module.Constants;
using FluentAssertions;
using Xunit;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Domain.Entities;

public class BackgroundJobExecutionEntityTests
{
    [Fact]
    public void Constructor_WithId_SetsIdAndInitializesMessages()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var execution = new BackgroundJobExecutionEntity(id);

        // Assert
        execution.Id.Should().Be(id);
        execution.Messages.Should().NotBeNull();
        execution.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_Default_InitializesMessages()
    {
        // Act
        var execution = new BackgroundJobExecutionEntity();

        // Assert
        execution.Messages.Should().NotBeNull();
        execution.Messages.Should().BeEmpty();
    }

    [Theory]
    [InlineData(BackgroundJobExecutionStatus.Starting, BackgroundJobExecutionStatus.Started, true)]
    [InlineData(BackgroundJobExecutionStatus.Started, BackgroundJobExecutionStatus.Completed, true)]
    [InlineData(BackgroundJobExecutionStatus.Started, BackgroundJobExecutionStatus.Errored, true)]
    [InlineData(BackgroundJobExecutionStatus.Started, BackgroundJobExecutionStatus.Cancelled, true)]
    public void UpdateStatus_ValidTransitions_UpdatesStatus(
        BackgroundJobExecutionStatus initialStatus,
        BackgroundJobExecutionStatus newStatus,
        bool shouldUpdate)
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(initialStatus)
            .Build();

        // Act
        execution.UpdateStatus(newStatus);

        // Assert
        if (shouldUpdate)
        {
            execution.Status.Should().Be(newStatus);
        }
    }

    [Theory]
    [InlineData(BackgroundJobExecutionStatus.Cancelled, BackgroundJobExecutionStatus.Started)]
    [InlineData(BackgroundJobExecutionStatus.Cancelled, BackgroundJobExecutionStatus.Completed)]
    [InlineData(BackgroundJobExecutionStatus.Errored, BackgroundJobExecutionStatus.Started)]
    [InlineData(BackgroundJobExecutionStatus.Errored, BackgroundJobExecutionStatus.Completed)]
    public void UpdateStatus_FromFinalStatus_PreventsUpdate(
        BackgroundJobExecutionStatus initialStatus,
        BackgroundJobExecutionStatus newStatus)
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(initialStatus)
            .Build();

        // Act
        execution.UpdateStatus(newStatus);

        // Assert
        execution.Status.Should().Be(initialStatus);
    }

    [Fact]
    public void UpdateStatus_FromCompletedToNonErrored_PreventsUpdate()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Completed)
            .Build();

        // Act
        execution.UpdateStatus(BackgroundJobExecutionStatus.Started);

        // Assert
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Completed);
    }

    [Fact]
    public void UpdateStatus_FromCompletedToErrored_AllowsUpdate()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Completed)
            .Build();

        // Act
        execution.UpdateStatus(BackgroundJobExecutionStatus.Errored);

        // Assert
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Errored);
    }

    [Fact]
    public void UpdateStatus_ToStarting_PreventsUpdate()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .Build();

        // Act
        execution.UpdateStatus(BackgroundJobExecutionStatus.Starting);

        // Assert
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Started);
    }

    [Fact]
    public void UpdateStatus_ToStarted_SetsExecutionStartTimeUtc()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Starting)
            .WithExecutionStartTime(DateTime.MinValue)
            .Build();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        execution.UpdateStatus(BackgroundJobExecutionStatus.Started);

        // Assert
        execution.Status.Should().Be(BackgroundJobExecutionStatus.Started);
        execution.ExecutionStartTimeUtc.Should().BeCloseTo(beforeUpdate, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(BackgroundJobExecutionStatus.Completed)]
    [InlineData(BackgroundJobExecutionStatus.Errored)]
    [InlineData(BackgroundJobExecutionStatus.Cancelled)]
    public void UpdateStatus_ToFinalStatus_SetsExecutionEndTimeUtc(BackgroundJobExecutionStatus finalStatus)
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create()
            .WithStatus(BackgroundJobExecutionStatus.Started)
            .WithExecutionEndTime(null)
            .Build();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        execution.UpdateStatus(finalStatus);

        // Assert
        execution.Status.Should().Be(finalStatus);
        execution.ExecutionEndTimeUtc.Should().HaveValue();
        execution.ExecutionEndTimeUtc.Value.Should().BeCloseTo(beforeUpdate, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(BackgroundJobMessageType.Info)]
    [InlineData(BackgroundJobMessageType.Error)]
    [InlineData(BackgroundJobMessageType.Warn)]
    public void AddMessage_WithType_AddsMessage(BackgroundJobMessageType messageType)
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create().Build();
        var messageText = "Test message";

        // Act
        execution.AddMessage(messageType, messageText);

        // Assert
        execution.Messages.Should().HaveCount(1);
        execution.Messages[0].MessageType.Should().Be(messageType);
        execution.Messages[0].TextMessage.Should().Be(messageText);
        execution.Messages[0].CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AddMessage_WithCustomDateTime_UsesProvidedDateTime()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create().Build();
        var customDateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var messageText = "Test message";

        // Act
        execution.AddMessage(BackgroundJobMessageType.Info, messageText, customDateTime);

        // Assert
        execution.Messages.Should().HaveCount(1);
        execution.Messages[0].CreationTime.Should().Be(customDateTime);
    }

    [Fact]
    public void AddMessage_MultipleMessages_AddsAll()
    {
        // Arrange
        var execution = BackgroundJobExecutionTestDataBuilder.Create().Build();

        // Act
        execution.AddMessage(BackgroundJobMessageType.Info, "Message 1");
        execution.AddMessage(BackgroundJobMessageType.Warn, "Message 2");
        execution.AddMessage(BackgroundJobMessageType.Error, "Message 3");

        // Assert
        execution.Messages.Should().HaveCount(3);
        execution.Messages[0].TextMessage.Should().Be("Message 1");
        execution.Messages[1].TextMessage.Should().Be("Message 2");
        execution.Messages[2].TextMessage.Should().Be("Message 3");
    }

    [Fact]
    public void AddMessage_WithNullMessagesList_InitializesList()
    {
        // Arrange
        var execution = new BackgroundJobExecutionEntity(Guid.NewGuid());
        execution.Messages = null;

        // Act
        execution.AddMessage(BackgroundJobMessageType.Info, "Test");

        // Assert
        execution.Messages.Should().NotBeNull();
        execution.Messages.Should().HaveCount(1);
    }
}

