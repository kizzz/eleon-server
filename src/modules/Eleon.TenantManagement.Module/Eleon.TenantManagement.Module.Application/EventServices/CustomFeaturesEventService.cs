using Common.EventBus.Module;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.FeatureManagement;
using VPortal.TenantManagement.Module.DomainServices;


namespace VPortal.TenantManagement.Module.Messaging
{
  public class CustomFeaturesEventService :
      IDistributedEventHandler<CreateBulkFeaturesForMicroserviceRequestMsg>,

      // Requests that expect a response
      IDistributedEventHandler<GetFeaturesQueryMsg>,
      IDistributedEventHandler<CreateFeatureDefinitionRequestMsg>,
      IDistributedEventHandler<UpdateFeatureDefinitionRequestMsg>,
      IDistributedEventHandler<DeleteFeatureRequestMsg>,

      IDistributedEventHandler<GetFeaturesGroupsQueryMsg>,
      IDistributedEventHandler<CreateFeatureGroupDefinitionRequestMsg>,
      IDistributedEventHandler<UpdateFeatureGroupDefinitionRequestMsg>,
      IDistributedEventHandler<DeleteFeatureGroupRequestMsg>,

      ITransientDependency
  {
    private readonly IVportalLogger<CustomFeaturesEventService> _logger;
    private readonly IResponseContext _responseContext;
    private readonly CustomFeaturesDomainService _customFeaturesDomainService;

    public CustomFeaturesEventService(
        IVportalLogger<CustomFeaturesEventService> logger,
        IResponseContext responseContext,
        CustomFeaturesDomainService customFeaturesDomainService)
    {
      _logger = logger;
      _responseContext = responseContext;
      _customFeaturesDomainService = customFeaturesDomainService;
    }

    public async Task HandleEventAsync(CreateBulkFeaturesForMicroserviceRequestMsg msg)
    {
      var resp = new PermissionServiceResponseMsg<bool> { Success = false };
      try
      {
        await _customFeaturesDomainService.CreateGroupsForMicroserviceAsync(msg.SourceId, msg.Groups.Select(ToRecord).ToList());
        await _customFeaturesDomainService.CreateFeaturesForMicroserviceAsync(msg.SourceId, msg.Features.Select(ToRecord).ToList());

        resp.Success = true;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(resp);
      }
    }

    public async Task HandleEventAsync(GetFeaturesQueryMsg _)
    {
      var response = new FeaturesServiceResponseMsg<List<FeatureDefinitionEto>>() { Result = new List<FeatureDefinitionEto>() };
      try
      {
        var features = await _customFeaturesDomainService.GetFeaturesDynamic();
        response.Result = features?.Select(ToEto).ToList() ?? new List<FeatureDefinitionEto>();
        response.Success = true;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(CreateFeatureDefinitionRequestMsg msg)
    {
      var response = new FeaturesServiceResponseMsg<FeatureDefinitionEto> { Success = false, Result = null };
      try
      {
        var dto = ToRecord(msg.Feature);
        var created = await _customFeaturesDomainService.CreateAsync(dto);
        response.Success = created != null;
        response.Result = created != null ? ToEto(created) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(UpdateFeatureDefinitionRequestMsg msg)
    {
      var response = new FeaturesServiceResponseMsg<FeatureDefinitionEto> { Success = false, Result = null };
      try
      {
        var dto = ToRecord(msg.Feature);
        var updated = await _customFeaturesDomainService.UpdateAsync(dto);
        response.Success = updated != null;
        response.Result = updated != null ? ToEto(updated) : null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(DeleteFeatureRequestMsg msg)
    {
      try { await _customFeaturesDomainService.DeleteAsync(msg.Name); }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
    }

    public async Task HandleEventAsync(GetFeaturesGroupsQueryMsg _)
    {
      var response = new FeaturesServiceResponseMsg<List<FeatureGroupDefinitionEto>>() { Result = new List<FeatureGroupDefinitionEto>() };
      try
      {
        var groups = await _customFeaturesDomainService.GetFeatureDynamicGroupCategories();
        response.Result = groups?.Select(ToEto).ToList() ?? new List<FeatureGroupDefinitionEto>();
        response.Success = true;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(CreateFeatureGroupDefinitionRequestMsg msg)
    {
      var response = new FeaturesServiceResponseMsg<FeatureGroupDefinitionEto>();
      try
      {
        var dto = ToRecord(msg.Group);
        var created = await _customFeaturesDomainService.CreateGroupAsync(dto);
        response.Result = ToEto(created);
        response.Success = created != null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(UpdateFeatureGroupDefinitionRequestMsg msg)
    {
      var response = new FeaturesServiceResponseMsg<FeatureGroupDefinitionEto>();
      try
      {
        var dto = ToRecord(msg.Group);
        var updated = await _customFeaturesDomainService.UpdateGroupAsync(dto);
        response.Result = ToEto(updated);
        response.Success = updated != null;
      }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(DeleteFeatureGroupRequestMsg msg)
    {
      try { await _customFeaturesDomainService.DeleteGroupAsync(msg.Name); }
      catch (Exception e) { _logger.CaptureAndSuppress(e); }
    }


    private static FeatureGroupDefinitionRecord ToRecord(FeatureGroupDefinitionEto e)
    {
      if (e == null) return null;

      var record = new FeatureGroupDefinitionRecord(e.Id, e.Name, e.DisplayName);
      record.SetProperty("CategoryName", e.CategoryName);
      record.SetProperty("Dynamic", e.IsDynamic);
      record.SetProperty(PermissionConstants.SourceIdPropertyName, e.SourceId);
      return record;
    }

    private static FeatureGroupDefinitionEto ToEto(FeatureGroupDefinitionRecord d) => d == null ? null : new()
    {
      Id = d.Id,
      Name = d.Name,
      DisplayName = d.DisplayName,
      CategoryName = d.GetProperty("CategoryName", string.Empty),
      IsDynamic = d.GetProperty("Dynamic", false),
      SourceId = d.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty)
    };

    private static FeatureDefinitionRecord ToRecord(FeatureDefinitionEto e)
    {
      if (e == null) return null;
      var record = new FeatureDefinitionRecord(e.Id, e.GroupName, e.Name, e.ParentName, e.DisplayName, e.Description, e.DefaultValue, e.IsVisibleToClients, e.IsAvailableToHost, e.AllowedProviders, e.ValueType);
      record.SetProperty("Dynamic", e.IsDynamic);
      record.SetProperty(PermissionConstants.SourceIdPropertyName, e.SourceId);
      return record;
    }

    private static FeatureDefinitionEto ToEto(FeatureDefinitionRecord d) => d == null ? null : new()
    {
      Id = d.Id,
      GroupName = d.GroupName,
      Name = d.Name,
      ParentName = d.ParentName,
      DisplayName = d.DisplayName,
      Description = d.Description,
      DefaultValue = d.DefaultValue,
      IsVisibleToClients = d.IsVisibleToClients,
      IsAvailableToHost = d.IsAvailableToHost,
      AllowedProviders = d.AllowedProviders,
      ValueType = d.ValueType,
      IsDynamic = d.GetProperty("Dynamic", false),
      SourceId = d.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty),
    };
  }
}
