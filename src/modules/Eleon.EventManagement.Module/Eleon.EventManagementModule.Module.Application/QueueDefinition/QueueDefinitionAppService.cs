using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Application.Contracts.Queue;
using EventManagementModule.Module.Application.Contracts.QueueDefenition;
using EventManagementModule.Module.Domain.Shared.Entities;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using VPortal.EventManagementModule.Module;

namespace EventManagementModule.Module.Application.QueueDefenition;

[Authorize]
public class QueueDefinitionAppService : EventManagementAppService, IQueueDefinitionAppService
{
  private readonly IVportalLogger<QueueDefinitionAppService> logger;
  private readonly QueueDefinitionDomainService queueDefinitionDomainService;

  public QueueDefinitionAppService(
      IVportalLogger<QueueDefinitionAppService> logger,
      QueueDefinitionDomainService queueDefinitionDomainService)
  {
    this.logger = logger;
    this.queueDefinitionDomainService = queueDefinitionDomainService;
  }

  public async Task<QueueDefinitionDto> CreateAsync(CreateQueueDefinitionRequestDto input)
  {
    QueueDefinitionDto result = null;
    try
    {
      var entity = await queueDefinitionDomainService.CreateAsync(
          input.Name,
          input.Messages,
          input.MessagesLimit);
      result = ObjectMapper.Map<QueueDefinitionEntity, QueueDefinitionDto>(entity);
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

  public async Task DeleteAsync(Guid id)
  {

    try
    {
      await queueDefinitionDomainService.DeleteAsync(id);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
  }

  public async Task<QueueDefinitionDto> EnsureCreatedAsync(CreateQueueDefinitionRequestDto input)
  {
    QueueDefinitionDto result = null;
    try
    {
      var entity = await queueDefinitionDomainService.EnsureCreatedAsync(
          input.Name,
          input.Messages,
          input.MessagesLimit);
      result = ObjectMapper.Map<QueueDefinitionEntity, QueueDefinitionDto>(entity);
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

  public async Task<List<QueueDefinitionDto>> GetAllAsync()
  {
    List<QueueDefinitionDto> result = null;
    try
    {
      var queueDefinitions = await queueDefinitionDomainService.GetPagedListAsync();
      return ObjectMapper.Map<List<QueueDefinitionEntity>, List<QueueDefinitionDto>>(queueDefinitions.Value);
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

  public async Task<QueueDefinitionDto> GetAsync(Guid id)
  {
    QueueDefinitionDto result = null;
    try
    {
      var entity = await queueDefinitionDomainService.GetAsync(id);
      result = ObjectMapper.Map<QueueDefinitionEntity, QueueDefinitionDto>(entity);
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

  public async Task<List<QueueDto>> GetAllQueuesByDefenitionIdAsync(Guid defenitionId)
  {
    List<QueueDto> result = null;
    try
    {
      var entity = await queueDefinitionDomainService.GetQueuesAsync(defenitionId);
      result = ObjectMapper.Map<List<QueueEntity>, List<QueueDto>>(entity);
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

  public async Task<PagedResultDto<QueueDefinitionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
  {
    PagedResultDto<QueueDefinitionDto> result = null;
    try
    {
      var response = await queueDefinitionDomainService.GetPagedListAsync(
          sorting: input.Sorting,
          maxResultCount: input.MaxResultCount,
          skipCount: input.SkipCount);
      var dtos = ObjectMapper.Map<List<QueueDefinitionEntity>, List<QueueDefinitionDto>>(response.Value);
      result = new PagedResultDto<QueueDefinitionDto>(response.Key, dtos);
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

  public async Task<QueueDefinitionDto> UpdateAsync(UpdateQueueDefinitionRequestDto input)
  {
    QueueDefinitionDto result = null;
    try
    {
      var entity = await queueDefinitionDomainService.UpdateAsync(input.Id, input.MessagesLimit);
      result = ObjectMapper.Map<QueueDefinitionEntity, QueueDefinitionDto>(entity);
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
}
