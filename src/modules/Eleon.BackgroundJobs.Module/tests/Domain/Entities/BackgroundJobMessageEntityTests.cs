using System;
using Common.Module.Constants;
using FluentAssertions;
using Xunit;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Domain.Entities;

public class BackgroundJobMessageEntityTests
{
    [Fact]
    public void Constructor_WithId_SetsId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var message = new BackgroundJobMessageEntity(id);

        // Assert
        message.Id.Should().Be(id);
    }

    [Fact]
    public void Constructor_Default_CreatesInstance()
    {
        // Act
        var message = new BackgroundJobMessageEntity();

        // Assert
        message.Should().NotBeNull();
    }

    [Theory]
    [InlineData(BackgroundJobMessageType.Info)]
    [InlineData(BackgroundJobMessageType.Error)]
    [InlineData(BackgroundJobMessageType.Warn)]
    public void MessageType_CanBeSet(BackgroundJobMessageType messageType)
    {
        // Arrange
        var message = BackgroundJobMessageTestDataBuilder.Create()
            .WithType(messageType)
            .Build();

        // Assert
        message.MessageType.Should().Be(messageType);
    }

    [Fact]
    public void TextMessage_CanBeSet()
    {
        // Arrange
        var text = "Test message";

        // Act
        var message = BackgroundJobMessageTestDataBuilder.Create()
            .WithText(text)
            .Build();

        // Assert
        message.TextMessage.Should().Be(text);
    }

    [Fact]
    public void CreationTime_CanBeSet()
    {
        // Arrange
        var creationTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var message = BackgroundJobMessageTestDataBuilder.Create()
            .WithCreationTime(creationTime)
            .Build();

        // Assert
        message.CreationTime.Should().Be(creationTime);
    }

    [Fact]
    public void TenantId_CanBeSet()
    {
        // Arrange
        var tenantId = TestConstants.TenantIds.Tenant1;

        // Act
        var message = BackgroundJobMessageTestDataBuilder.Create()
            .WithTenantId(tenantId)
            .Build();

        // Assert
        message.TenantId.Should().Be(tenantId);
    }
}

