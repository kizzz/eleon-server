using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Application.Contracts.Queue;
using EventManagementModule.Module.Domain.Shared.Entities;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Queue;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace VPortal.EventManagementModule.Module.Application.Events;

[Authorize]
[ExposeServices(typeof(IQueueAppService))]
[Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
public class QueueAppService : EventManagementAppService, IQueueAppService
{
  private readonly QueueDomainService queueDomainService;
  private readonly QueueDefinitionDomainService queueDefinitionDomainService;
  private readonly IVportalLogger<QueueAppService> logger;

  public QueueAppService(
      QueueDomainService queueDomainService,
      QueueDefinitionDomainService queueDefinitionDomainService,
      IVportalLogger<QueueAppService> logger)
  {
    this.queueDomainService = queueDomainService;
    this.queueDefinitionDomainService = queueDefinitionDomainService;
    this.logger = logger;
  }
  public async Task<QueueDto> GetAsync(QueueRequestDto request)
  {
    try
    {
      var entity = await queueDomainService.GetAsync(request.QueueName);
      return ObjectMapper.Map<QueueEntity, QueueDto>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<List<QueueDto>> GetAllAsync()
  {
    try
    {
      var entities = await queueDomainService.GetPagedListAsync();
      var result = ObjectMapper.Map<List<QueueEntity>, List<QueueDto>>(entities.Value);
      return result;
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<QueueDto> CreateAsync(CreateQueueRequestDto input)
  {
    try
    {
      var entity = await queueDomainService.CreateAsync(
              input.Name,
              input.DisplayName,
              input.Forwarding,
              input.MessagesLimit,
              ensureCreated: false);
      var result = ObjectMapper.Map<QueueEntity, QueueDto>(entity);
      return result;
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<QueueDto> EnsureCreatedAsync(CreateQueueRequestDto input)
  {
    try
    {
      var entity = await queueDomainService.CreateAsync(
              input.Name,
              input.DisplayName,
              input.Forwarding,
              input.MessagesLimit,
              ensureCreated: true);
      var result = ObjectMapper.Map<QueueEntity, QueueDto>(entity);
      return result;
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<QueueDto> UpdateAsync(UpdateQueueRequestDto input)
  {
    QueueDto result = null;
    try
    {
      var entity = await queueDomainService.UpdateAsync(
              input.Name,
              input.NewName,
              input.DisplayName,
              input.Forwarding,
              input.MessagesLimit);
      result = ObjectMapper.Map<QueueEntity, QueueDto>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
    return result;
  }
  public async Task DeleteAsync(QueueRequestDto request)
  {
    try
    {
      await queueDomainService.DeleteAsync(request.QueueName);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task ClearAsync(QueueRequestDto request)
  {
    try
    {
      await queueDomainService.ClearAsync(request.QueueName);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task<PagedResultDto<QueueDto>> GetListAsync(QueuesListRequestDto input)
  {
    try
    {
      var response = await queueDomainService.GetPagedListAsync(
          sorting: input.Sorting,
          maxResultCount: input.MaxResultCount,
          skipCount: input.SkipCount);

      return new PagedResultDto<QueueDto>(response.Key, ObjectMapper.Map<List<QueueEntity>, List<QueueDto>>(response.Value));
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  //public async Task<List<QueueDto>> GetAllByDefinitionIdAsync(Guid definitionId)
  //{
  //    try
  //    {
  //        var entities = await queueDefinitionDomainService.GetQueuesAsync(definitionId);
  //        return ObjectMapper.Map<List<QueueEntity>, List<QueueDto>>(entities);
  //    }
  //    catch (Exception e)
  //    {
  //        logger.Capture(e);
  //        throw;
  //    }
  //    finally
  //    {
  //    }
  //}
}
