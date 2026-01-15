using Eleon.Common.Lib.Helpers;
using Eleon.Common.Lib.UserToken;
using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Security.Claims;
using EventManagementModule.Module.Domain.Shared.Queues;
using VPortal.EventManagementModule.Module.Localization;

namespace EventManagementModule.Domain.EventServices;
public class EventDomainService : DomainService
{
  private readonly IVportalLogger<EventDomainService> logger;
  private readonly IQueueDefinitionRepository queueDefenitionRepository;
  private readonly IQueueRepository queueRepository;
  private readonly QueueDomainService queueDomainService;
  private readonly QueueDefinitionDomainService queueDefinitionDomainService;
  private readonly IStringLocalizer<EventManagementModuleResource> localizer;
  private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
  private readonly IUserTokenProvider _userTokenProvider;
  private readonly IQueueEngine _queueEngine;
  private readonly QueueEngineOptions _queueEngineOptions;

  public EventDomainService(
      IVportalLogger<EventDomainService> logger,
      IQueueDefinitionRepository queueDefinitionRepository,
      IQueueRepository queueRepository,
      QueueDomainService queueDomainService,
      QueueDefinitionDomainService queueDefinitionDomainService,
      IStringLocalizer<EventManagementModuleResource> localizer,
      ICurrentPrincipalAccessor currentPrincipalAccessor,
      IUserTokenProvider userTokenProvider,
      IQueueEngine queueEngine,
      IOptions<QueueEngineOptions> queueEngineOptions)
  {
    this.logger = logger;
    this.queueDefenitionRepository = queueDefinitionRepository;
    this.queueRepository = queueRepository;
    this.queueDomainService = queueDomainService;
    this.queueDefinitionDomainService = queueDefinitionDomainService;
    this.localizer = localizer;
    _currentPrincipalAccessor = currentPrincipalAccessor;
    _userTokenProvider = userTokenProvider;
    _queueEngine = queueEngine;
    _queueEngineOptions = queueEngineOptions.Value;
  }

  public async Task<KeyValuePair<long, List<EventEntity>>> GetPagedListAsync(
     Guid queueId, // todo (may be should to use queue name instead of id ???)
     string sorting = null,
     int maxResultCount = int.MaxValue,
     int skipCount = 0)
  {
    KeyValuePair<long, List<EventEntity>> result = new KeyValuePair<long, List<EventEntity>>();
    try
    {
      var queue = await queueRepository.GetAsync(queueId, false);
      result = new KeyValuePair<long, List<EventEntity>>(
          (long)queue.Count,
          await queueRepository.GetMessagesListAsync(queueId, sorting, maxResultCount, skipCount)
          );
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }

  private async Task<List<QueueDefinitionEntity>> GetDefinitionsForMessageAsync(string queueName, string eventName)
  {
    var dbSet = await queueDefenitionRepository.GetDbSetAsync();

    // using where condition for taking less results
    // because every queue that not contains event name does not cotaint this event
    var queues = await dbSet.Where(d => d.TenantId == CurrentTenant.Id).Where(d => d.Name == queueName || d.Messages.Contains(eventName) || d.Messages.Contains("*")).ToListAsync();

    List<QueueDefinitionEntity> result = new();

    foreach (var queue in queues)
    {
      var eventNames = queue.Messages?.Split(";") ?? [];

      if (eventNames.Contains("*"))
      {
        result.Add(queue);
      }
      else if (eventNames.Contains(eventName))
      {
        result.Add(queue);
      }
      else if (queue.Name == queueName)
      {
        result.Add(queue);
      }
    }

    if (!result.Any(q => q.Name == queueName))
    {
      result.Add(await queueDefinitionDomainService.EnsureCreatedAsync(EventManagementDefaults.SystemQueueName, "", EventManagementDefaults.DefaultSystemQueueLimit));
    }

    return result.DistinctBy(x => x.Name).ToList();

  }

  protected virtual async Task<List<QueueEntity>> GetQueuesForMessageAsync(string queueName, string eventName)
  {
    var queues = (
        await queueRepository.GetDbSetAsync())
        .Where(x => x.Name == queueName || x.Forwarding.Contains(eventName) || x.Forwarding.Contains(EventManagementDefaults.ForwardingAll)) // optimization for taking less results
        .ToListAsync();

    var result = new List<QueueEntity>();
    foreach (var queue in await queues)
    {
      var forwarding = queue.Forwarding?.Split(EventManagementDefaults.ForwardingSeparator) ?? [];
      if (forwarding.Contains(EventManagementDefaults.ForwardingAll))
      {
        result.Add(queue);
      }
      else if (forwarding.Contains(eventName))
      {
        result.Add(queue);
      }
      else if (queue.Name == queueName)
      {
        result.Add(queue);
      }
    }

    if (!result.Any(q => q.Name == queueName))
    {
      result.Add(await queueDomainService.GetSystemAsync());
    }

    return result.DistinctBy(x => x.Name).ToList();
  }

  public async Task PublishAsync(string queueName, string eventName, string message)
  {

    try
    {
      var queues = await GetQueuesForMessageAsync(queueName, eventName);

      foreach (var queue in queues)
      {
        try
        {
          var eventEntity = new EventEntity(GuidGenerator.Create())
          {
            Name = eventName,
            Message = message,
          };

          var claims = _currentPrincipalAccessor.Principal != null
            ? ClaimsPrincipalJsonHelper.Serialize(_currentPrincipalAccessor.Principal)
            : null;

          eventEntity.SetProperty("Claims", claims);
          eventEntity.SetProperty("Token", _userTokenProvider.Token);

          if (_queueEngineOptions.QueueEngineMode is QueueEngineMode.LinkedList or QueueEngineMode.DualWrite)
          {
            await queueRepository.EnqueueAsync(queue.Id, eventEntity);
          }

          if (_queueEngineOptions.QueueEngineMode is QueueEngineMode.SqlClaim or QueueEngineMode.DualWrite)
          {
            var payload = new QueuePayload(message, _userTokenProvider.Token, claims);
            var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload);
            var messageKey = eventEntity.Id.ToString("N");

            await _queueEngine.EnqueueManyAsync(
                new QueueKey(queue.Id, queue.TenantId, 0),
                new[]
                {
                  new QueueMessageToEnqueue(
                      eventName,
                      payloadBytes,
                      "application/json",
                      "utf-8",
                      messageKey,
                      null)
                },
                CancellationToken.None);

            if (_queueEngineOptions.ShadowVerificationEnabled)
            {
              var sampleRate = Math.Max(1, _queueEngineOptions.ShadowVerificationSampleRate);
              if (Random.Shared.Next(sampleRate) == 0)
              {
                var exists = await _queueEngine.ExistsMessageKeyAsync(
                    new QueueKey(queue.Id, queue.TenantId, 0),
                    messageKey,
                    CancellationToken.None);

                if (!exists)
                {
                  logger.Log.LogError("Shadow verification failed for queue {QueueId} messageKey {MessageKey}", queue.Id, messageKey);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          logger.Log.LogError("Failed to enqueu event {eventName} to queue {queueName}", eventName, queue?.Name);
          logger.CaptureAndSuppress(ex);
        }
      }
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<(List<EventEntity> Messages, string Status, int CountLeft)> ReceiveManyAsync(string queueName, int count)
  {

    if (count <= 0)
    {
      logger.Log.LogWarning("Invalid recieve messages count - {0}", count);
      throw new ArgumentException(localizer["Event:RecieveCount:MustBeGratherThanZero"], nameof(count));
    }

    if (count > EventManagementDefaults.MaxReceiveMessagesCount)
    {
      logger.Log.LogWarning("Recieve messages count ({0}) was automaticaly changed by max value ({1})",
          count, EventManagementDefaults.MaxReceiveMessagesCount);
      count = EventManagementDefaults.MaxReceiveMessagesCount;
    }

    (List<EventEntity> Messages, string Status, int CountLeft) result = (null, null, 0);
    try
    {
      var queue = await queueDomainService.GetAsync(queueName);

      if (queue == null)
      {
        result = (new List<EventEntity>(), EventManagementDefaults.QueueStatuses.NotFound, 0);
      }
      else if (_queueEngineOptions.ConsumerMode == ConsumerMode.SqlClaim)
      {
        var lockId = Guid.NewGuid();
        var claimed = await _queueEngine.ClaimManyAsync(
            new QueueKey(queue.Id, queue.TenantId, 0),
            new ClaimOptions(count, lockId, TimeSpan.FromSeconds(EventManagementDefaults.DefaultClaimLockSeconds)),
            CancellationToken.None);

        var messages = claimed.Select(c => MapClaimedToEventEntity(c, queue.TenantId)).ToList();
        if (claimed.Count > 0)
        {
          await _queueEngine.AckAsync(lockId, claimed.Select(c => c.Id).ToList(), CancellationToken.None);
        }

        var pendingCount = await _queueEngine.GetPendingCountAsync(
            new QueueKey(queue.Id, queue.TenantId, 0),
            CancellationToken.None);

        result = (messages, EventManagementDefaults.QueueStatuses.Ok, (int)Math.Min(int.MaxValue, pendingCount));
      }
      else
      {
        var messages = await queueRepository.DequeueManyAsync(queue.Id, count);

        queue = await queueDomainService.GetAsync(queueName);

        result = (messages, EventManagementDefaults.QueueStatuses.Ok, queue.Count);
      }
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task<(Guid LockId, List<ClaimedQueueMessage> Messages, string Status, int PendingCount)> ClaimManyAsync(string queueName, int count, int lockSeconds)
  {
    if (count <= 0)
    {
      logger.Log.LogWarning("Invalid recieve messages count - {0}", count);
      throw new ArgumentException(localizer["Event:RecieveCount:MustBeGratherThanZero"], nameof(count));
    }

    if (count > EventManagementDefaults.MaxReceiveMessagesCount)
    {
      logger.Log.LogWarning("Recieve messages count ({0}) was automaticaly changed by max value ({1})",
          count, EventManagementDefaults.MaxReceiveMessagesCount);
      count = EventManagementDefaults.MaxReceiveMessagesCount;
    }

    var queue = await queueDomainService.GetAsync(queueName);
    if (queue == null)
    {
      return (Guid.Empty, new List<ClaimedQueueMessage>(), EventManagementDefaults.QueueStatuses.NotFound, 0);
    }

    var lockId = Guid.NewGuid();
    var claimed = await _queueEngine.ClaimManyAsync(
        new QueueKey(queue.Id, queue.TenantId, 0),
        new ClaimOptions(count, lockId, TimeSpan.FromSeconds(lockSeconds)),
        CancellationToken.None);

    var pendingCount = await _queueEngine.GetPendingCountAsync(
        new QueueKey(queue.Id, queue.TenantId, 0),
        CancellationToken.None);

    return (lockId, claimed.ToList(), EventManagementDefaults.QueueStatuses.Ok, (int)Math.Min(int.MaxValue, pendingCount));
  }

  public Task AckAsync(Guid lockId, IReadOnlyList<Guid> messageIds)
  {
    return _queueEngine.AckAsync(lockId, messageIds, CancellationToken.None);
  }

  public Task NackAsync(Guid lockId, Guid messageId, int maxAttempts, int delaySeconds, string error)
  {
    return _queueEngine.NackAsync(lockId, messageId, new NackOptions(maxAttempts, TimeSpan.FromSeconds(delaySeconds), error), CancellationToken.None);
  }

  private static EventEntity MapClaimedToEventEntity(ClaimedQueueMessage claimed, Guid? tenantId)
  {
    QueuePayload? payload = null;
    try
    {
      payload = JsonSerializer.Deserialize<QueuePayload>(claimed.Payload.Span);
    }
    catch
    {
      // ignore malformed payloads, keep raw message empty
    }

    var entity = new EventEntity(claimed.Id)
    {
      Name = claimed.Name,
      Message = payload?.Message ?? string.Empty,
      TenantId = tenantId
    };

    if (!string.IsNullOrWhiteSpace(payload?.Claims))
    {
      entity.SetProperty("Claims", payload.Claims);
    }

    if (!string.IsNullOrWhiteSpace(payload?.Token))
    {
      entity.SetProperty("Token", payload.Token);
    }

    return entity;
  }

  private sealed record QueuePayload(string Message, string? Token, string? Claims);

  public async Task<EventEntity> GetAsync(Guid messageId)
  {

    try
    {
      var dbContext = await queueRepository.GetDbContextAsync();
      var message = await dbContext.Set<EventEntity>().FirstOrDefaultAsync(m => m.Id == messageId);
      return message ?? throw new EntityNotFoundException(typeof(EventEntity), messageId);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
