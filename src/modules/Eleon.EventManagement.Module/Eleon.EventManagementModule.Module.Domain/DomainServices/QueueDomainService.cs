using EventManagementModule.Module.Domain.Shared.Constants;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Validation;
using VPortal.EventManagementModule.Module.Localization;

namespace EventManagementModule.Domain.EventServices;
public class QueueDomainService : DomainService
{
  private readonly IVportalLogger<QueueDomainService> _logger;
  private readonly IQueueRepository queueRepository;
  private readonly IStringLocalizer<EventManagementModuleResource> _localizer;
  // private readonly QueueDefinitionDomainService queueDefinitionDomainService;

  public QueueDomainService(
      IVportalLogger<QueueDomainService> logger,
      IQueueRepository messageRepository,
      IStringLocalizer<EventManagementModuleResource> localizer
      // QueueDefinitionDomainService queueDefinitionDomainService
      )
  {
    this._logger = logger;
    this.queueRepository = messageRepository;
    this._localizer = localizer;
    // this.queueDefinitionDomainService = queueDefinitionDomainService;
  }

  public async Task<QueueEntity> GetSystemAsync()
  {
    try
    {
      var result = await CreateAsync(
          EventManagementDefaults.SystemQueueName,
          EventManagementDefaults.SystemQueueDisplayName,
          string.Empty,
          EventManagementDefaults.DefaultSystemQueueLimit,
          ensureCreated: true);
      return result;
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

  public async Task<QueueEntity> GetErrorAsync()
  {
    try
    {
      var result = await CreateAsync(
          EventManagementDefaults.ErrorQueueName,
          EventManagementDefaults.ErrorQueueName,
          string.Empty,
          EventManagementDefaults.DefaultSystemQueueLimit,
          ensureCreated: true);
      return result;
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

  public async Task<QueueEntity?> GetAsync(string name)
  {
    try
    {
      return await queueRepository.FindByNameAsync(name, false);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return null;
  }

  public async Task<KeyValuePair<long, List<QueueEntity>>> GetPagedListAsync(
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0)
  {
    KeyValuePair<long, List<QueueEntity>> result = new KeyValuePair<long, List<QueueEntity>>();
    try
    {
      result = new KeyValuePair<long, List<QueueEntity>>(
          await queueRepository.GetCountAsync(),
          await queueRepository.GetPagedListAsync(skipCount, maxResultCount, sorting, false)
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

  public async Task<QueueEntity> CreateAsync(string name, string displayName, string forwarding, int limit, bool ensureCreated = false)
  {
    try
    {
      // var queueDefinition = await queueDefinitionDomainService.EnsureCreatedAsync(name, string.Empty, EventManagementDefaults.DefaultSystemQueueLimit);

      var result = await queueRepository.FindByNameAsync(name);
      if (result != null)
      {
        if (ensureCreated)
        {
          return result;
        }

        throw new AbpValidationException(_localizer["Queue:AlreadyExists"]);
      }

      result = await ValidateAndCreateAsync(name, displayName, forwarding, limit); // queueDefinition.Id, queueDefinition.MessagesLimit
      return result;
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

  public async Task<QueueEntity> UpdateAsync(string name, string newName, string displayName, string forwarding, int limit = 0)
  {
    try
    {
      var queue = await GetAsync(name) ?? throw new EntityNotFoundException(typeof(QueueEntity));

      if (!EventManagementDefaults.IsSystemQueue(name))
      {
        ValidateName(newName);
        queue.Name = newName;
        queue.DisplayName = displayName ?? string.Empty;
        queue.Forwarding = ValidateForwarding(forwarding);

        queue = await queueRepository.UpdateAsync(queue, true);
      }

      ValidateLimit(name, limit);
      await queueRepository.SetMessagesLimitAsync(queue.Id, limit);

      return queue;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return null;
  }

  public async Task DeleteAsync(string name)
  {
    try
    {
      if (EventManagementDefaults.IsSystemQueue(name))
      {
        throw new AbpAuthorizationException("Forbidden to remove system queues");
      }

      var queue = await GetAsync(name) ?? throw new EntityNotFoundException(typeof(QueueEntity));

      await queueRepository.HardDeleteAsync(queue);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task ClearAsync(string name)
  {

    try
    {
      var queue = await GetAsync(name);
      await queueRepository.ClearAsync(queue.Id);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  private async Task<QueueEntity> ValidateAndCreateAsync(
      string name,
      string displayName,
      string forwarding,
      int limit = 0
      // Guid definitionId,
      )
  {
    ValidateName(name);
    ValidateLimit(name, limit);
    forwarding = ValidateForwarding(forwarding);
    return await queueRepository.InsertAsync(
        new QueueEntity(GuidGenerator.Create())
        {
          Name = name,
          DisplayName = displayName,
          Forwarding = forwarding,
          MessagesLimit = limit,
          QueueDefinitionId = Guid.NewGuid(), // remove index for unique defenition id
        }, true);
  }

  private void ValidateLimit(string name, int limit)
  {
    if (EventManagementDefaults.IsSystemQueue(name))
    {
      if (!(EventManagementDefaults.MinSystemQueueMessagesLimit <= limit && limit <= EventManagementDefaults.MaxMessagesLimit)) // limit == 0 || unlimited queues forbidden
      {
        throw new AbpValidationException([new ValidationResult(_localizer["QueueDefinition:ValidationError:Limit:Invalid"])]);
      }

      return;
    }

    if (!(EventManagementDefaults.MinMessagesLimit <= limit && limit <= EventManagementDefaults.MaxMessagesLimit))
    {
      throw new AbpValidationException([new ValidationResult(_localizer["QueueDefinition:ValidationError:Limit:Invalid"])]);
    }
  }

  private void ValidateName(string name)
  {
    if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_:]+$"))
    {
      throw new AbpValidationException([new ValidationResult(_localizer["QueueDefinition:ValidationError:Name:Invalid"])]);
    }
  }

  private string ValidateForwarding(string forwarding)
  {
    if (string.IsNullOrWhiteSpace(forwarding))
      return string.Empty;

    var parts = forwarding
        .Split(EventManagementDefaults.ForwardingSeparator, StringSplitOptions.RemoveEmptyEntries)
        .Select(p => p.Trim())
        .Where(p => !string.IsNullOrEmpty(p))
        .ToList();

    // Validate allowed characters for each part
    foreach (var part in parts)
    {
      if (!System.Text.RegularExpressions.Regex.IsMatch(part, @"^[a-zA-Z0-9_:]+$") && part != "*")
        throw new AbpValidationException([new ValidationResult(_localizer["QueueDefinition:ValidationError:Forwarding:Invalid"])]);
    }

    // If any part is exactly "*", return "*"
    if (parts.Any(p => p == EventManagementDefaults.ForwardingAll))
    {
      return EventManagementDefaults.ForwardingAll;
    }

    return string.Join(EventManagementDefaults.ForwardingSeparator, parts);
  }
}
