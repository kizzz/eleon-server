using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eleon.Common.Lib.UserToken;
using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Queues;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using NSubstitute;
using Shouldly;
using Volo.Abp.Security.Claims;
using VPortal.EventManagementModule.Module.Tests.TestBase;
using VPortal.EventManagementModule.Module.Localization;
using Xunit;

namespace VPortal.EventManagementModule.Module.Tests.Stress;

public class EventQueueStressTests : EventManagementTestBase
{
  [Fact]
  [Trait("Category", "Manual")]
  public async Task ReceiveManyAsync_HighVolume_ClaimsAndAcks()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid(), Forwarding = string.Empty };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>())
        .Returns(0);

    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(callInfo =>
        {
          var options = callInfo.ArgAt<ClaimOptions>(1);
          var list = Enumerable.Range(0, Math.Min(5, options.Count))
              .Select(_ => new ClaimedQueueMessage(
                  Guid.NewGuid(),
                  queueId,
                  0,
                  1,
                  "event",
                  1,
                  DateTime.UtcNow,
                  null,
                  null,
                  ReadOnlyMemory<byte>.Empty,
                  "application/json",
                  null))
              .ToList();
          return list;
        });

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    var iterations = GetIntEnv("EVENT_QUEUE_STRESS_ITERATIONS", 1000);
    for (var i = 0; i < iterations; i++)
    {
      var result = await service.ReceiveManyAsync(queueName, 5);
      result.Status.ShouldBe(global::EventManagementModule.Module.Domain.Shared.Constants.EventManagementDefaults.QueueStatuses.Ok);
      result.Messages.Count.ShouldBeGreaterThan(0);
    }

    await queueEngine.Received(iterations).ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>());
    await queueEngine.Received(iterations).AckAsync(Arg.Any<Guid>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  [Trait("Category", "Manual")]
  public async Task ReceiveManyAsync_ParallelLoad_ClaimsAndAcks()
  {
    var queueId = Guid.NewGuid();
    var queueName = "queue";
    var queue = new QueueEntity(queueId) { Name = queueName, TenantId = Guid.NewGuid(), Forwarding = string.Empty };

    var queueRepository = CreateQueueRepository();
    queueRepository.FindByNameAsync(queueName, false).Returns(queue);

    var queueEngine = CreateQueueEngine();
    queueEngine.GetPendingCountAsync(Arg.Any<QueueKey>(), Arg.Any<CancellationToken>())
        .Returns(0);

    queueEngine.ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>())
        .Returns(callInfo =>
        {
          var options = callInfo.ArgAt<ClaimOptions>(1);
          var list = Enumerable.Range(0, Math.Min(10, options.Count))
              .Select(_ => new ClaimedQueueMessage(
                  Guid.NewGuid(),
                  queueId,
                  0,
                  1,
                  "event",
                  1,
                  DateTime.UtcNow,
                  null,
                  null,
                  ReadOnlyMemory<byte>.Empty,
                  "application/json",
                  null))
              .ToList();
          return list;
        });

    var service = CreateEventDomainService(
        queueRepository: queueRepository,
        queueEngine: queueEngine,
        options: new QueueEngineOptions { ConsumerMode = ConsumerMode.SqlClaim, QueueEngineMode = QueueEngineMode.SqlClaim });

    var iterations = GetIntEnv("EVENT_QUEUE_LOAD_ITERATIONS", 2000);
    var parallelism = GetIntEnv("EVENT_QUEUE_LOAD_PARALLELISM", 8);
    var tasks = new List<Task>();
    using var throttle = new SemaphoreSlim(parallelism, parallelism);

    for (var i = 0; i < iterations; i++)
    {
      await throttle.WaitAsync();
      tasks.Add(Task.Run(async () =>
      {
        try
        {
          var result = await service.ReceiveManyAsync(queueName, 10);
          result.Status.ShouldBe(global::EventManagementModule.Module.Domain.Shared.Constants.EventManagementDefaults.QueueStatuses.Ok);
        }
        finally
        {
          throttle.Release();
        }
      }));
    }

    await Task.WhenAll(tasks);

    await queueEngine.Received(iterations).ClaimManyAsync(Arg.Any<QueueKey>(), Arg.Any<ClaimOptions>(), Arg.Any<CancellationToken>());
    await queueEngine.Received(iterations).AckAsync(Arg.Any<Guid>(), Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  [Trait("Category", "Manual")]
  public async Task PublishAsync_ParallelLoad_Enqueues()
  {
    var queueId = Guid.NewGuid();
    var queues = new List<QueueEntity>
    {
      new QueueEntity(queueId) { Name = "queue", TenantId = Guid.NewGuid(), Forwarding = string.Empty }
    };

    var queueEngine = CreateQueueEngine();
    queueEngine.EnqueueManyAsync(Arg.Any<QueueKey>(), Arg.Any<IReadOnlyList<QueueMessageToEnqueue>>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

    var service = new TestEventDomainService(
        queues,
        CreateMockLogger<EventDomainService>(),
        CreateQueueDefinitionRepository(),
        CreateQueueRepository(),
        CreateQueueDomainService(),
        CreateQueueDefinitionDomainService(),
        CreateLocalizer<EventManagementModuleResource>("key", "value"),
        Substitute.For<ICurrentPrincipalAccessor>(),
        Substitute.For<IUserTokenProvider>(),
        queueEngine,
        new QueueEngineOptions
        {
          QueueEngineMode = QueueEngineMode.SqlClaim,
          ShadowVerificationEnabled = false
        });
    SetLazyServiceProvider(service, CreateMockGuidGenerator(), CreateClock(DateTime.UtcNow));

    var iterations = GetIntEnv("EVENT_QUEUE_PUBLISH_ITERATIONS", 2000);
    var parallelism = GetIntEnv("EVENT_QUEUE_PUBLISH_PARALLELISM", 8);
    var tasks = new List<Task>();
    using var throttle = new SemaphoreSlim(parallelism, parallelism);

    for (var i = 0; i < iterations; i++)
    {
      await throttle.WaitAsync();
      tasks.Add(Task.Run(async () =>
      {
        try
        {
          await service.PublishAsync("queue", "event", "payload");
        }
        finally
        {
          throttle.Release();
        }
      }));
    }

    await Task.WhenAll(tasks);

    await queueEngine.Received(iterations).EnqueueManyAsync(
        Arg.Any<QueueKey>(),
        Arg.Any<IReadOnlyList<QueueMessageToEnqueue>>(),
        Arg.Any<CancellationToken>());
  }

  private static int GetIntEnv(string key, int fallback)
  {
    var raw = Environment.GetEnvironmentVariable(key);
    return int.TryParse(raw, out var value) && value > 0 ? value : fallback;
  }

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
        Microsoft.Extensions.Localization.IStringLocalizer<EventManagementModuleResource> localizer,
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
            Microsoft.Extensions.Options.Options.Create(options))
    {
      _queues = queues;
    }

    protected override Task<List<QueueEntity>> GetQueuesForMessageAsync(string queueName, string eventName)
    {
      return Task.FromResult(_queues);
    }
  }
}
