using Common.EventBus.Module;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.DomainServices;

public class CustomPermissionDomainService : DomainService
{
  private readonly IDistributedEventBus _eventBus;
  private readonly IVportalLogger<CustomPermissionDomainService> _logger;

  public CustomPermissionDomainService(
      IDistributedEventBus eventBus,
      IVportalLogger<CustomPermissionDomainService> logger)
  {
    _eventBus = eventBus;
    _logger = logger;
  }

  #region Microservice Permissions
  public async Task<bool> CreateBulkForMicroserviceAsync(string sourceId, List<PermissionGroupDefinitionEto> groups, List<PermissionDefinitionEto> permissions)
  {
    try
    {
      await _eventBus.PublishAsync(new CreateBulkPermissionsForMicroserviceRequestMsg { Permissions = permissions, Groups = groups, SourceId = sourceId });
      return true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }
  #endregion

  #region Permission Management

  public async Task<List<PermissionDefinitionEto>> GetPermissionsAsync()
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<List<PermissionDefinitionEto>>>(
          new GetPermissionsQueryMsg());

      return response.Result;
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

  public async Task<PermissionDefinitionEto> CreateAsync(PermissionDefinitionEto permission)
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<PermissionDefinitionEto>>(
          new CreatePermissionRequestMsg { Permission = permission });

      return response.Result;
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

  public async Task<PermissionDefinitionEto> UpdateAsync(PermissionDefinitionEto permission)
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<PermissionDefinitionEto>>(
          new UpdatePermissionRequestMsg { Permission = permission });

      return response.Result;
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

  public async Task DeleteAsync(string name)
  {
    try
    {
      await _eventBus.PublishAsync(new DeletePermissionRequestMsg { Name = name });
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

  #endregion

  #region Permission Groups Management

  public async Task<List<PermissionGroupDefinitionEto>> GetPermissionGroupsAsync()
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<List<PermissionGroupDefinitionEto>>>(
          new GetPermissionGroupsQueryMsg());

      return response.Result;
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

  public async Task<PermissionGroupDefinitionEto> CreateGroupAsync(PermissionGroupDefinitionEto group)
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<PermissionGroupDefinitionEto>>(
          new CreatePermissionGroupRequestMsg { Group = group });

      return response.Result;
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

  public async Task<PermissionGroupDefinitionEto> UpdateGroupAsync(PermissionGroupDefinitionEto group)
  {
    try
    {
      var response = await _eventBus.RequestAsync<PermissionServiceResponseMsg<PermissionGroupDefinitionEto>>(
          new UpdatePermissionGroupRequestMsg { Group = group });

      return response.Result;
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

  public async Task DeleteGroupAsync(string name)
  {
    try
    {
      await _eventBus.PublishAsync(new DeletePermissionGroupRequestMsg { Name = name });
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

  #endregion
}

