using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Eleon.Common.Lib.UserToken;
using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Queues;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Volo.Abp.Security.Claims;
using VPortal.EventManagementModule.Module.Tests.TestBase;
using VPortal.EventManagementModule.Module.Localization;
using Xunit;

namespace VPortal.EventManagementModule.Module.Tests.Domain;

public class EventDomainServiceTests : EventManagementTestBase
{
  [Fact]
  public async Task ReceiveManyAsync_ConsumerSqlClaim_ClaimsAndAcks()
  {
    var queueId = Guid.NewGuid();
    var tenantId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = tenantId };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(new QueuePayload("hello", "token", "claims"));
    var claimed = new List<ClaimedQueueMessage>
    {
      new ClaimedQueueMessage(
          Guid.NewGuid(),
          queueId,
          0,
          1,
          "eventName",
          2,
          DateTime.UtcNow,
          "key",
          "trace",
          payloadBytes,
          "application/json",
          "utf-8")
    };

    Guid capturedLockId = Guid.Empty;
    queueEngine
        .ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Do<ClaimOptions>(o => capturedLockId = o.LockId), Arg.Any<CancellationToken>())
        .Returns(claimed);
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>()).Returns(3);

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    var result = await service.ReceiveManyAsync(queueName, 5);

    result.Status.ShouldBe(EventManagementDefaults.QueueStatuses.Ok);
    result.Messages.ShouldNotBeNull();
    result.Messages.Count.ShouldBe(1);
    result.Messages[0].Name.ShouldBe("eventName");
    result.Messages[0].Message.ShouldBe("hello");
    result.CountLeft.ShouldBe(3);

    await queueEngine.Received(1).AckAsync(
        capturedLockId,
        Arg.Is<IReadOnlyList<Guid>>(ids => ids.Count == 1 && ids.First() == claimed[0].Id),
        Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ReceiveManyAsync_ConsumerSqlClaim_NoClaims_NoAck()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid() };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(new List<ClaimedQueueMessage>());
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>()).Returns(0);

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    var result = await service.ReceiveManyAsync(queueName, 5);

    result.Messages.ShouldBeEmpty();
    await queueEngine.DidNotReceive().AckAsync(Arg.Any<Guid>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ReceiveManyAsync_ClampsMaxCount()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid() };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    ClaimOptions captured = null;
    var queueEngine = CreateQueueEngine();
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Do<ClaimOptions>(o => captured = o), Arg.Any<CancellationToken>())
        .Returns(new List<ClaimedQueueMessage>());
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>()).Returns(0);

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    await service.ReceiveManyAsync(queueName, EventManagementDefaults.MaxReceiveMessagesCount + 500);

    captured.ShouldNotBeNull();
    captured.Count.ShouldBe(EventManagementDefaults.MaxReceiveMessagesCount);
  }

  [Fact]
  public async Task ReceiveManyAsync_InvalidCount_Throws()
  {
    var service = CreateEventDomainService();
    await Should.ThrowAsync<ArgumentException>(() => service.ReceiveManyAsync("queue", 0));
  }

  [Fact]
  public async Task ReceiveManyAsync_QueueNotFound_ReturnsNotFound()
  {
    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync("missing", false).Returns((QueueEntity)null);

    var service = CreateEventDomainService(queueRepository: queueRepository);
    var result = await service.ReceiveManyAsync("missing", 1);

    result.Status.ShouldBe(EventManagementDefaults.QueueStatuses.NotFound);
    result.Messages.ShouldBeEmpty();
  }

  [Fact]
  public async Task ReceiveManyAsync_MalformedPayload_ReturnsEmptyMessage()
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
          new byte[] { 0xFF, 0xFE, 0xFD },
          "application/json",
          "utf-8")
    };
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(claimed);
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>())
        .Returns(0);

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    var result = await service.ReceiveManyAsync(queueName, 1);

    result.Messages.Count.ShouldBe(1);
    result.Messages[0].Message.ShouldBe(string.Empty);
  }

  [Fact]
  public async Task ReceiveManyAsync_ConsumerLinkedList_UsesRepository()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName };
    var messages = new List<EventEntity> { new EventEntity(Guid.NewGuid()) { Name = "event", Message = "m" } };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);
    queueRepository.DequeueManyAsync(queueId, 2).Returns(messages);

    var queueEngine = CreateQueueEngine();
    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.LinkedList, QueueEngineMode = QueueEngineMode.LinkedList });

    var result = await service.ReceiveManyAsync(queueName, 2);

    result.Messages.Count.ShouldBe(1);
    await queueRepository.Received(1).DequeueManyAsync(queueId, 2);
    await queueEngine.DidNotReceive().ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ClaimManyAsync_ReturnsLockAndPendingCount()
  {
    var queueId = Guid.NewGuid();
    var tenantId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = tenantId };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    var claimed = new List<ClaimedQueueMessage>
    {
      new ClaimedQueueMessage(Guid.NewGuid(), queueId, 0, 1, "event", 1, DateTime.UtcNow, null, null, ReadOnlyMemory<byte>.Empty, "application/json", null)
    };

    ClaimOptions captured = null;
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Do<ClaimOptions>(o => captured = o), Arg.Any<CancellationToken>())
        .Returns(claimed);
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>()).Returns(5);

    var service = CreateEventDomainService(queueRepository: queueRepository, queueEngine: queueEngine);

    var result = await service.ClaimManyAsync(queueName, 2, 30);

    result.LockId.ShouldNotBe(Guid.Empty);
    result.PendingCount.ShouldBe(5);
    result.Messages.Count.ShouldBe(1);
    captured.ShouldNotBeNull();
    captured.LockFor.TotalSeconds.ShouldBe(30);
  }

  [Fact]
  public async Task ClaimManyAsync_ClampsMaxCount()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid() };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    ClaimOptions captured = null;
    var queueEngine = CreateQueueEngine();
    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Do<ClaimOptions>(o => captured = o), Arg.Any<CancellationToken>())
        .Returns(new List<ClaimedQueueMessage>());
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>()).Returns(0);

    var service = CreateEventDomainService(queueRepository: queueRepository, queueEngine: queueEngine);

    await service.ClaimManyAsync(queueName, EventManagementDefaults.MaxReceiveMessagesCount + 1000, 30);

    captured.ShouldNotBeNull();
    captured.Count.ShouldBe(EventManagementDefaults.MaxReceiveMessagesCount);
  }


  [Fact]
  public async Task ClaimManyAsync_InvalidCount_Throws()
  {
    var service = CreateEventDomainService();
    await Should.ThrowAsync<ArgumentException>(() => service.ClaimManyAsync("queue", 0, 30));
  }

  [Fact]
  public async Task ClaimManyAsync_QueueNotFound_ReturnsNotFound()
  {
    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync("missing", false).Returns((QueueEntity)null);

    var service = CreateEventDomainService(queueRepository: queueRepository);
    var result = await service.ClaimManyAsync("missing", 1, 30);

    result.Status.ShouldBe(EventManagementDefaults.QueueStatuses.NotFound);
    result.Messages.ShouldBeEmpty();
  }

  [Fact]
  public async Task AckAsync_DelegatesToEngine()
  {
    var queueEngine = CreateQueueEngine();
    var service = CreateEventDomainService(queueEngine: queueEngine);
    var lockId = Guid.NewGuid();
    var ids = new List<Guid> { Guid.NewGuid() };

    await service.AckAsync(lockId, ids);

    await queueEngine.Received(1).AckAsync(lockId, ids, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task NackAsync_DelegatesToEngine()
  {
    var queueEngine = CreateQueueEngine();
    var service = CreateEventDomainService(queueEngine: queueEngine);
    var lockId = Guid.NewGuid();
    var messageId = Guid.NewGuid();

    await service.NackAsync(lockId, messageId, 10, 5, "error");

    await queueEngine.Received(1).NackAsync(
        lockId,
        messageId,
        Arg.Is<NackOptions>(o => o.MaxAttempts == 10 && o.Delay.TotalSeconds == 5 && o.Error == "error"),
        Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task PublishAsync_ShadowVerification_InvokesExistsMessageKey()
  {
    var queueName = "queue";
    var eventName = "event";
    var queueId = Guid.NewGuid();
    var tenantId = Guid.NewGuid();

    var queues = new List<QueueEntity>
    {
      new QueueEntity(queueId)
      {
        Name = queueName,
        TenantId = tenantId,
        Forwarding = string.Empty
      }
    };
    var queueRepository = CreateQueueRepository();

    var queueEngine = CreateQueueEngine();
    queueEngine.EnqueueManyAsync(Arg.Any<QueueKey>(), Arg.Any<IReadOnlyList<QueueMessageToEnqueue>>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);
    queueEngine.ExistsMessageKeyAsync(Arg.Any<QueueKey>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(false);

    var service = new TestEventDomainService(
        queues,
        CreateMockLogger<EventDomainService>(),
        CreateQueueDefinitionRepository(),
        queueRepository,
        CreateQueueDomainService(queueRepository),
        CreateQueueDefinitionDomainService(queueRepository),
        CreateLocalizer<EventManagementModuleResource>("key", "value"),
        Substitute.For<ICurrentPrincipalAccessor>(),
        Substitute.For<IUserTokenProvider>(),
        queueEngine,
        new QueueEngineOptions
        {
          QueueEngineMode = QueueEngineMode.SqlClaim,
          ShadowVerificationEnabled = true,
          ShadowVerificationSampleRate = 1
        });
    SetLazyServiceProvider(service, CreateMockGuidGenerator(), CreateClock(DateTime.UtcNow));

    await service.PublishAsync(queueName, eventName, "payload");

    await queueEngine.Received(1).EnqueueManyAsync(
        Arg.Any<QueueKey>(),
        Arg.Any<IReadOnlyList<QueueMessageToEnqueue>>(),
        Arg.Any<CancellationToken>());

    await queueEngine.Received(1).ExistsMessageKeyAsync(
        Arg.Is<QueueKey>(k => k.QueueId == queueId),
        Arg.Any<string>(),
        Arg.Any<CancellationToken>());
  }

  private sealed record QueuePayload(string Message, string? Token, string? Claims);

  private sealed class TestEventDomainService : EventDomainService
  {
    private readonly List<QueueEntity> _queues;

    public TestEventDomainService(
        List<QueueEntity> queues,
        IVportalLogger<EventDomainService> logger,
        IQueueDefinitionRepository queueDefinitionRepository,
        IQueueRepository queueRepository,
        QueueDomainService queueDomainService,
        QueueDefinitionDomainService queueDefinitionDomainService,
        IStringLocalizer<EventManagementModuleResource> localizer,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IUserTokenProvider userTokenProvider,
        IQueueEngine queueEngine,
        QueueEngineOptions options)
        : base(
            logger,
            queueDefinitionRepository,
            queueRepository,
            queueDomainService,
            queueDefinitionDomainService,
            localizer,
            currentPrincipalAccessor,
            userTokenProvider,
            queueEngine,
            Options.Create(options))
    {
      _queues = queues;
    }

    protected override Task<List<QueueEntity>> GetQueuesForMessageAsync(string queueName, string eventName)
    {
      return Task.FromResult(_queues);
    }
  }
}
