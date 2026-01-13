using Common.EventBus.Module;
using EventBus.MassTransit.Module;
using Logging.Module;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;

namespace EventBus.Nats
{
  public class NatsConnectionWrapper : IAsyncDisposable
  {
    private const int PoolSize = 5;
    private NatsConnectionPool? connectionPool;
    private bool _disposed;
    private readonly ILogger<NatsConnectionWrapper> logger;
    private readonly ResponseContext context;
    private readonly EventContextManager eventContextManager;
    private readonly NatsOpts natsConnectionOptions;
    private readonly CancellationTokenSource stopTokenSource = new CancellationTokenSource();
    private Task listeningTask;

    public NatsConnectionWrapper(
        ILogger<NatsConnectionWrapper> logger,
        ResponseContext context,
        EventContextManager eventContextManager,
        NatsOpts natsConnectionOptions)
    {
      this.logger = logger;
      this.context = context;
      this.eventContextManager = eventContextManager;
      this.natsConnectionOptions = natsConnectionOptions;
    }

    public void StartListening(string subject, Func<string, string, Task> messageHandler)
    {
      EnsureConnection();

      logger.LogDebug("{0}.{1} started", nameof(NatsConnectionWrapper), nameof(StartListening));
      try
      {
        listeningTask = Listen(subject, messageHandler);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "{0}.{1} errored", nameof(NatsConnectionWrapper), nameof(StartListening));
        throw;
      }
      finally
      {
        logger.LogDebug("{0}.{1} finished", nameof(NatsConnectionWrapper), nameof(StartListening));
      }
    }

    public INatsConnection GetConnection()
    {
      EnsureConnection();
      return connectionPool!.GetConnection();
    }

    private async Task Listen(string subject, Func<string, string, Task> messageHandler)
    {
      await using var conn = connectionPool!.GetConnection();
      await foreach (var msg in conn.SubscribeAsync<string>(subject, cancellationToken: stopTokenSource.Token))
      {
        try
        {
          if (msg.Headers != null
              && msg.Headers.TryGetValue(EventContextConsts.EventContextHeaderName, out var contextHeader))
          {
            eventContextManager.UnwrapEventContext(contextHeader.First()!);
          }

          if (eventContextManager.IsCurrentEventDuplicate())
          {
            return;
          }

          context.SetContext(new NatsDistributedEventBusResponder<string>(msg));

          await messageHandler(msg.Subject, msg.Data);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "{0}.{1} errored", nameof(NatsConnectionWrapper), nameof(Listen));
        }
      }
    }

    private void EnsureConnection()
    {
      if (connectionPool is not null)
      {
        return;
      }

      connectionPool = new NatsConnectionPool(PoolSize, natsConnectionOptions);
      logger.LogDebug("Created NATS connection: {Connection}", connectionPool);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;

      await stopTokenSource.CancelAsync();

      if (listeningTask != null)
      {
        await listeningTask;
      }

      if (connectionPool != null)
      {
        await connectionPool.DisposeAsync();
      }
    }
  }
}
