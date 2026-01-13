using Common.EventBus.Module;
using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TenantSettings.Module.Cache;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Validation;
using VPortal.EventManagementModule.Module.Localization;

namespace EventManagementModule.Domain.EventServices;
public class QueueDefinitionDomainService : DomainService
{
  private readonly IVportalLogger<QueueDefinitionDomainService> _logger;
  private readonly IStringLocalizer<EventManagementModuleResource> _localizer;
  private readonly IQueueRepository queueRepository;
  private readonly IQueueDefinitionRepository queueDefenitionRepository;
  private readonly IDistributedEventBus eventBus;
  private readonly TenantCacheService _tenantCacheService;

  public QueueDefinitionDomainService(
        IVportalLogger<QueueDefinitionDomainService> logger,
        IStringLocalizer<EventManagementModuleResource> localizer,
        IQueueRepository queueRepository,
        IQueueDefinitionRepository queueDefenitionRepository,
        IDistributedEventBus eventBus,
        TenantCacheService tenantCacheService
        )
  {
    this._logger = logger;
    this._localizer = localizer;
    this.queueRepository = queueRepository;
    this.queueDefenitionRepository = queueDefenitionRepository;
    this.eventBus = eventBus;
    _tenantCacheService = tenantCacheService;
  }

  public async Task<QueueDefinitionEntity> GetAsync(Guid id)
  {
    try
    {
      return await queueDefenitionRepository.GetAsync(id);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<QueueDefinitionEntity> GetAsync(string name)
  {
    QueueDefinitionEntity? result = null;
    try
    {
      result = await (await queueDefenitionRepository.GetDbSetAsync())
          .FirstOrDefaultAsync(qd => qd.TenantId == CurrentTenant.Id && qd.Name == name);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }

  public async Task<List<QueueEntity>> GetQueuesAsync(Guid definitionId)
  {
    var result = new List<QueueEntity>();
    try
    {
      var tenantIds = await GetAllTenantIdsAsync();
      foreach (var tenantId in tenantIds)
      {
        using (CurrentTenant.Change(tenantId))
        {
          var queue = await (await queueRepository.GetDbSetAsync()).FirstOrDefaultAsync(q => q.QueueDefinitionId == definitionId);

          if (queue == null)
          {
            result.Add(new QueueEntity(GuidGenerator.Create())
            {
              TenantId = tenantId,
              QueueDefinitionId = definitionId
            });
          }
          else
          {
            result.Add(queue);
          }
        }
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }


  public async Task<QueueDefinitionEntity> CreateAsync(string name, string messages, int limit = 0)
  {
    QueueDefinitionEntity result = null;
    try
    {
      result = await (await queueDefenitionRepository.GetDbSetAsync())
          .FirstOrDefaultAsync(q => q.TenantId == CurrentTenant.Id && q.Name == name);
      if (result != null)
      {
        throw new AbpValidationException(_localizer["Queue:AlreadyExists"]);
      }

      result = await ValidateAndCreateAsync(name, messages, limit);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }

  public async Task<QueueDefinitionEntity> EnsureCreatedAsync(string name, string messages, int limit = 0)
  {
    QueueDefinitionEntity result = null;
    try
    {
      result = await (await queueDefenitionRepository.GetDbSetAsync())
          .FirstOrDefaultAsync(q => q.TenantId == CurrentTenant.Id && q.Name == name);
      result ??= await ValidateAndCreateAsync(name, messages, limit);

      bool updated = false;
      if (result.Messages != messages)
      {
        result.Messages = messages;
        updated = true;
      }

      if (result.MessagesLimit != limit)
      {
        result.MessagesLimit = limit;
        updated = true;
      }

      if (updated)
      {
        await queueDefenitionRepository.UpdateAsync(result);
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }

  public async Task<QueueDefinitionEntity> UpdateAsync(Guid id, int limit = 0)
  {
    QueueDefinitionEntity result = null;
    try
    {
      var entity = await queueDefenitionRepository.GetAsync(id, true);
      if (entity == null)
      {
        throw new AbpValidationException(_localizer["Queue:NotFound"]);
      }

      ValidateLimit(limit);

      entity.MessagesLimit = limit;

      var tenantIds = await GetAllTenantIdsAsync();
      foreach (var tenantId in tenantIds)
      {
        using (CurrentTenant.Change(tenantId))
        {
          var queue = await (await queueRepository.GetDbSetAsync()).FirstOrDefaultAsync(q => q.QueueDefinitionId == entity.Id);
          if (queue != null)
          {
            await queueRepository.SetMessagesLimitAsync(queue.Id, limit);
          }
        }
      }

      result = await queueDefenitionRepository.UpdateAsync(entity);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
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
      var entity = await queueDefenitionRepository.GetAsync(id);

      await queueDefenitionRepository.HardDeleteAsync(entity);
      var tenantIds = await GetAllTenantIdsAsync();
      foreach (var tenantId in tenantIds)
      {
        using (CurrentTenant.Change(tenantId))
        {
          var queue = await (await queueRepository.GetDbSetAsync()).FirstOrDefaultAsync(q => q.QueueDefinitionId == entity.Id);
          if (queue != null)
          {
            await queueRepository.HardDeleteAsync(queue);
          }
        }
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<KeyValuePair<long, List<QueueDefinitionEntity>>> GetPagedListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0)
  {
    KeyValuePair<long, List<QueueDefinitionEntity>> result = new();
    try
    {
      result = new KeyValuePair<long, List<QueueDefinitionEntity>>(
      await (await queueDefenitionRepository.GetDbSetAsync()).Where(q => q.TenantId == CurrentTenant.Id).CountAsync(),
          await queueDefenitionRepository.GetCustomListAsync(CurrentTenant.Id, sorting, maxResultCount, skipCount, false)
          );
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }

  private async Task<List<Guid?>> GetAllTenantIdsAsync()
  {
    var response = await _tenantCacheService.GetTenantsAsync();
    //var response = await eventBus.RequestAsync<AllTenantsGotMsg>(new GetAllTenantsMsg());

    var currentTenantId = CurrentTenant.Id?.ToString();
    var ids = response
        //.Tenants
        .Where(t => t.ParentId == currentTenantId)
        .Select(t => (Guid?)t.Id)
        .Concat([CurrentTenant.Id])
        .Distinct()
        .ToList();
    return ids;
  }

  private async Task<QueueDefinitionEntity> ValidateAndCreateAsync(string name, string messages, int limit = 0)
  {
    ValidateName(name);
    ValidateLimit(limit);
    return await queueDefenitionRepository.InsertAsync(
        new QueueDefinitionEntity(GuidGenerator.Create())
        {
          TenantId = CurrentTenant.Id,
          Name = name,
          Messages = messages,
          MessagesLimit = limit,
        }, true);
  }

  private void ValidateLimit(int limit)
  {
    if (!(limit == 0 || (EventManagementDefaults.MinMessagesLimit <= limit && limit <= EventManagementDefaults.MaxMessagesLimit)))
    {
      throw new AbpValidationException(_localizer["Queue:Error:MessagesLimitInvalid"]);
    }
  }

  private void ValidateName(string name)
  {
    if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_:]+$"))
    {
      throw new AbpValidationException(_localizer["Queue:Error:NameInvalid"]);
    }
  }
}
