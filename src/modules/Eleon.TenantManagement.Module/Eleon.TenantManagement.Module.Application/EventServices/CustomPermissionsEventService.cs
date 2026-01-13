using AutoMapper.Internal.Mappers;
using Common.EventBus.Module;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.CustomPermissions;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.PermissionManagement;
using VPortal.TenantManagement.Module.CustomPermissions;
using VPortal.TenantManagement.Module.DomainServices;


namespace VPortal.TenantManagement.Module.Messaging
{
  public class CustomPermissionsEventService :
      // microservice permission management
      IDistributedEventHandler<CreateBulkPermissionsForMicroserviceRequestMsg>,

      // permision management
      IDistributedEventHandler<GetPermissionsQueryMsg>,
      IDistributedEventHandler<CreatePermissionRequestMsg>,
      IDistributedEventHandler<UpdatePermissionRequestMsg>,
      IDistributedEventHandler<DeletePermissionRequestMsg>,

      // permission group management
      IDistributedEventHandler<GetPermissionGroupsQueryMsg>,
      IDistributedEventHandler<CreatePermissionGroupRequestMsg>,
      IDistributedEventHandler<UpdatePermissionGroupRequestMsg>,
      IDistributedEventHandler<DeletePermissionGroupRequestMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CustomPermissionsEventService> _logger;
    private readonly IResponseContext _response;
    private readonly CustomPermissionDomainService _app;

    public CustomPermissionsEventService(
        IVportalLogger<CustomPermissionsEventService> logger,
        IResponseContext responseContext,
        CustomPermissionDomainService app)
    {
      _logger = logger;
      _response = responseContext;
      _app = app;
    }

    public async Task HandleEventAsync(CreateBulkPermissionsForMicroserviceRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<bool> { Success = false };
      try
      {
        var groups = msg.Groups.Select(ToRecord).ToList();
        var permissions = msg.Permissions.Select(ToRecord).ToList();

        var created =
            await _app.CreateGroupsForMicroserviceAsync(msg.SourceId, groups) &&
            await _app.CreatePermissionsForMicroserviceAsync(msg.SourceId, permissions);
        resp.Success = created;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }


    public async Task HandleEventAsync(GetPermissionsQueryMsg _)
    {
      var resp = new PermissionServiceResponseMsg<List<PermissionDefinitionEto>>
      { Success = true, Result = new() };
      try
      {
        var perms = await _app.GetPermissionsDynamic();
        resp.Result = perms?.Select(FromRecord).ToList() ?? new();
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); resp.Success = false; }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(CreatePermissionRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<PermissionDefinitionEto> { Success = false };
      try
      {
        var created = await _app.CreateAsync(ToRecord(msg.Permission), msg.Permission.Order);
        resp.Success = created != null;
        resp.Result = created != null ? FromRecord(created) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(UpdatePermissionRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<PermissionDefinitionEto> { Success = false };
      try
      {
        var updated = await _app.UpdateAsync(ToRecord(msg.Permission));
        resp.Success = updated != null;
        resp.Result = updated != null ? FromRecord(updated) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(DeletePermissionRequestMsg msg)
    {
      try { await _app.DeleteAsync(msg.Name); }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
    }

    public async Task HandleEventAsync(GetPermissionGroupsQueryMsg _)
    {
      var resp = new PermissionServiceResponseMsg<List<PermissionGroupDefinitionEto>>
      { Success = true, Result = new() };
      try
      {
        var groups = await _app.GetPermissionDynamicGroupCategories();
        resp.Result = groups?.Select(FromRecord).ToList() ?? new();
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); resp.Success = false; }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(CreatePermissionGroupRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<PermissionGroupDefinitionEto> { Success = false };
      try
      {
        var created = await _app.CreateGroupAsync(ToRecord(msg.Group));
        resp.Success = created != null;
        resp.Result = created != null ? FromRecord(created) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(UpdatePermissionGroupRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<PermissionGroupDefinitionEto> { Success = false };
      try
      {
        var updated = await _app.UpdateGroupAsync(ToRecord(msg.Group));
        resp.Success = updated != null;
        resp.Result = updated != null ? FromRecord(updated) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _response.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(DeletePermissionGroupRequestMsg msg)
    {
      try { await _app.DeleteGroupAsync(msg.Name); }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
    }

    private static PermissionDefinitionRecord ToRecord(PermissionDefinitionEto e)
    {
      if (e == null) return null;

      var record = new PermissionDefinitionRecord(e.Id, e.GroupName, e.Name, e.ParentName, e.DisplayName, e.IsEnabled, e.MultiTenancySide, e.Providers, e.StateCheckers);
      record.SetProperty("Order", e.Order);
      record.SetProperty("Dynamic", e.Dynamic);
      record.SetProperty(PermissionConstants.SourceIdPropertyName, e.SourceId);

      return record;
    }

    private static PermissionGroupDefinitionRecord ToRecord(PermissionGroupDefinitionEto e)
    {
      var record = new PermissionGroupDefinitionRecord(e.Id, e.Name, e.DisplayName);
      record.SetProperty("CategoryName", e.CategoryName);
      record.SetProperty("Order", e.Order);
      record.SetProperty("Dynamic", e.Dynamic);
      record.SetProperty(PermissionConstants.SourceIdPropertyName, e.SourceId);

      return record;
    }

    private static PermissionDefinitionEto FromRecord(PermissionDefinitionRecord e)
    {
      if (e == null) return null;

      var eto = new PermissionDefinitionEto
      {
        Id = e.Id,
        GroupName = e.GroupName,
        Name = e.Name,
        ParentName = e.ParentName,
        DisplayName = e.DisplayName,
        IsEnabled = e.IsEnabled,
        MultiTenancySide = e.MultiTenancySide,
        Providers = e.Providers,
        StateCheckers = e.StateCheckers,
        Order = e.GetProperty("Order", 0),
        Dynamic = e.GetProperty("Dynamic", false),
        SourceId = e.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty)
      };

      return eto;
    }

    private static PermissionGroupDefinitionEto FromRecord(PermissionGroupDefinitionRecord e)
    {
      var eto = new PermissionGroupDefinitionEto
      {
        Id = e.Id,
        Name = e.Name,
        DisplayName = e.DisplayName,
        CategoryName = e.GetProperty("CategoryName", string.Empty),
        Order = e.GetProperty("Order", 0),
        Dynamic = e.GetProperty("Dynamic", false),
        SourceId = e.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty)
      };
      return eto;
    }
  }
}
