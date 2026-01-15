using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Application.Contracts.Event;
using EventManagementModule.Module.Application.Contracts.Queue;
using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Event;
using System.IO;
using System.Linq;
using System.Text.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using VPortal.EventManagementModule.Module;
using EventManagementModule.Module.Domain.Shared.Queues;

namespace EventManagementModule.Module.Application.Events;

[Authorize]
[ExposeServices(typeof(IEventAppService))]
[Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
public class EventAppService : EventManagementAppService, IEventAppService
{
  private readonly EventDomainService eventDomainService;
  private readonly IVportalLogger<EventAppService> _logger;
  private readonly QueueDomainService queueDomainService;

  public EventAppService(
      EventDomainService eventDomainService,
      IVportalLogger<EventAppService> logger,
      QueueDomainService queueDomainService)
  {
    this.eventDomainService = eventDomainService;
    this._logger = logger;
    this.queueDomainService = queueDomainService;
  }

  public async Task PublishAsync(PublishMessageRequestDto input)
  {
    try
    {
      await eventDomainService.PublishAsync(
          input.QueueName,
          input.Name,
          input.Message);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task<RecieveMessagesResponseDto> ReceiveManyAsync(string queueName, int maxCount)
  {
    RecieveMessagesResponseDto result = null;
    try
    {
      var (messages, status, count) = await eventDomainService.ReceiveManyAsync(queueName, maxCount);
      var dtos = ObjectMapper.Map<List<EventEntity>, List<FullEventDto>>(messages);
      result = new RecieveMessagesResponseDto
      {
        Messages = dtos,
        QueueStatus = status,
        MessagesLeft = count
      };
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
    return result;
  }

  public async Task<ClaimMessagesResponseDto> ClaimManyAsync(ClaimMessagesRequestDto input)
  {
    ClaimMessagesResponseDto result = null;
    try
    {
      var (lockId, messages, status, pending) = await eventDomainService.ClaimManyAsync(
          input.QueueName,
          input.MaxCount,
          input.LockSeconds);

      var dtos = messages.Select(MapClaimedMessage).ToList();
      result = new ClaimMessagesResponseDto
      {
        LockId = lockId,
        QueueStatus = status,
        PendingCount = pending,
        Messages = dtos
      };
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
    return result;
  }

  public async Task AckAsync(AckRequestDto input)
  {
    try
    {
      await eventDomainService.AckAsync(input.LockId, input.MessageIds);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task NackAsync(NackRequestDto input)
  {
    try
    {
      await eventDomainService.NackAsync(input.LockId, input.MessageId, input.MaxAttempts, input.DelaySeconds, input.Error);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task PublishErrorAsync(string message)
  {
    try
    {
      await queueDomainService.GetErrorAsync(); // creates error queue if not exists
      await eventDomainService.PublishAsync(
          EventManagementDefaults.ErrorQueueName,
          EventManagementDefaults.ErrorEventName,
          message);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task<PagedResultDto<EventDto>> GetListAsync(MessagesPagedAndSortedResultRequestDto input)
  {
    PagedResultDto<EventDto> result = null;
    try
    {
      var response = await eventDomainService.GetPagedListAsync(
              input.QueueId,
              sorting: input.Sorting,
              maxResultCount: input.MaxResultCount,
              skipCount: input.SkipCount);
      var dtos = ObjectMapper.Map<List<EventEntity>, List<EventDto>>(response.Value);
      result = new PagedResultDto<EventDto>(response.Key, dtos);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }
    finally
    {
    }
    return result;
  }

  public async Task<FullEventDto> DownloadMessageAsync(Guid messageId)
  {
    try
    {
      var message = await eventDomainService.GetAsync(messageId);
      var messageBody = message.Message ?? string.Empty;
      var messageName = $"{message.Name}_{message.CreationTime:yyyy-MM-dd_HH-mm-ss}.txt";
      return ObjectMapper.Map<EventEntity, FullEventDto>(message);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  private static ClaimedMessageDto MapClaimedMessage(ClaimedQueueMessage message)
  {
    QueuePayload? payload = null;
    try
    {
      payload = JsonSerializer.Deserialize<QueuePayload>(message.Payload.Span);
    }
    catch
    {
    }

    return new ClaimedMessageDto
    {
      Id = message.Id,
      Name = message.Name,
      Message = payload?.Message ?? string.Empty,
      Token = payload?.Token,
      Claims = payload?.Claims,
      Attempts = message.Attempts,
      CreatedUtc = message.CreatedUtc,
      MessageKey = message.MessageKey,
      TraceId = message.TraceId
    };
  }

  private sealed record QueuePayload(string Message, string? Token, string? Claims);
}
