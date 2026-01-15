using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventManagementModule.Module.Application.Contracts.Event;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Queues;
using NSubstitute;
using Shouldly;
using VPortal.EventManagementModule.Module.Tests.TestBase;
using Xunit;

namespace VPortal.EventManagementModule.Module.Tests.Application;

public class EventAppServiceTests : EventManagementTestBase
{
  [Fact]
  public async Task ClaimManyAsync_MapsPayloadToDto()
  {
    var queueId = Guid.NewGuid();
    var tenantId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = tenantId };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(new QueuePayload("body", "token", "claims"));
    var claimed = new List<ClaimedQueueMessage>
    {
      new ClaimedQueueMessage(
          Guid.NewGuid(),
          queueId,
          0,
          1,
          "eventName",
          3,
          DateTime.UtcNow,
          "key",
          "trace",
          payloadBytes,
          "application/json",
          "utf-8")
    };

    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(claimed);
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>())
        .Returns(2);

    var domainService = CreateEventDomainService(queueRepository: queueRepository, queueEngine: queueEngine);
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        domainService,
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService(queueRepository));

    var result = await appService.ClaimManyAsync(new ClaimMessagesRequestDto
    {
      QueueName = queueName,
      MaxCount = 5,
      LockSeconds = 10
    });

    result.QueueStatus.ShouldNotBeNull();
    result.Messages.Count.ShouldBe(1);
    result.Messages[0].Name.ShouldBe("eventName");
    result.Messages[0].Message.ShouldBe("body");
    result.Messages[0].Token.ShouldBe("token");
    result.Messages[0].Claims.ShouldBe("claims");
  }

  [Fact]
  public async Task ClaimManyAsync_MalformedPayload_ReturnsEmptyMessage()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid(), Forwarding = string.Empty };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    var claimed = new List<ClaimedQueueMessage>
    {
      new ClaimedQueueMessage(
          Guid.NewGuid(),
          queueId,
          0,
          1,
          "eventName",
          1,
          DateTime.UtcNow,
          null,
          null,
          new byte[] { 0xFF, 0x00, 0xFE },
          "application/json",
          "utf-8")
    };
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(claimed);
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>())
        .Returns(0);

    var domainService = CreateEventDomainService(queueRepository: queueRepository, queueEngine: queueEngine);
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        domainService,
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService(queueRepository));

    var result = await appService.ClaimManyAsync(new ClaimMessagesRequestDto
    {
      QueueName = queueName,
      MaxCount = 1,
      LockSeconds = 10
    });

    result.Messages.Count.ShouldBe(1);
    result.Messages[0].Message.ShouldBe(string.Empty);
  }

  [Fact]
  public async Task ClaimManyAsync_QueueNotFound_ReturnsNotFound()
  {
    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync("missing", false).Returns((QueueEntity)null);

    var domainService = CreateEventDomainService(queueRepository: queueRepository, queueEngine: CreateQueueEngine());
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        domainService,
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService(queueRepository));

    var result = await appService.ClaimManyAsync(new ClaimMessagesRequestDto
    {
      QueueName = "missing",
      MaxCount = 1,
      LockSeconds = 10
    });

    result.QueueStatus.ShouldBe(global::EventManagementModule.Module.Domain.Shared.Constants.EventManagementDefaults.QueueStatuses.NotFound);
    result.Messages.ShouldBeEmpty();
  }

  [Fact]
  public async Task ClaimManyAsync_InvalidCount_ReturnsNull()
  {
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        CreateEventDomainService(),
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService());

    var result = await appService.ClaimManyAsync(new ClaimMessagesRequestDto
    {
      QueueName = "queue",
      MaxCount = 0,
      LockSeconds = 10
    });

    result.ShouldBeNull();
  }

  [Fact]
  public async Task AckAsync_DelegatesToEngine()
  {
    var queueEngine = CreateQueueEngine();
    var domainService = CreateEventDomainService(queueEngine: queueEngine);
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        domainService,
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService());

    var lockId = Guid.NewGuid();
    var messageIds = new List<Guid> { Guid.NewGuid() };

    await appService.AckAsync(new AckRequestDto { LockId = lockId, MessageIds = messageIds });

    await queueEngine.Received(1).AckAsync(lockId, messageIds, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task NackAsync_DelegatesToEngine()
  {
    var queueEngine = CreateQueueEngine();
    var domainService = CreateEventDomainService(queueEngine: queueEngine);
    var appService = new global::EventManagementModule.Module.Application.Events.EventAppService(
        domainService,
        CreateMockLogger<global::EventManagementModule.Module.Application.Events.EventAppService>(),
        CreateQueueDomainService());

    var lockId = Guid.NewGuid();
    var messageId = Guid.NewGuid();

    await appService.NackAsync(new NackRequestDto
    {
      LockId = lockId,
      MessageId = messageId,
      MaxAttempts = 9,
      DelaySeconds = 4,
      Error = "err"
    });

    await queueEngine.Received(1).NackAsync(
        lockId,
        messageId,
        Arg.Is<NackOptions>(o => o.MaxAttempts == 9 && o.Delay.TotalSeconds == 4 && o.Error == "err"),
        Arg.Any<CancellationToken>());
  }

  private sealed record QueuePayload(string Message, string? Token, string? Claims);
}
