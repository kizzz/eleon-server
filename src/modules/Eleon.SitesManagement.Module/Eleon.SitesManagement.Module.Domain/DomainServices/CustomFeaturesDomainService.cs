using Common.EventBus.Module;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.DomainServices;


public class CustomFeaturesDomainService : DomainService
{
  private readonly IDistributedEventBus _eventBus;
  private readonly IVportalLogger<CustomFeaturesDomainService> _logger;

  public CustomFeaturesDomainService(IDistributedEventBus eventBus, IVportalLogger<CustomFeaturesDomainService> logger)
  {
    _eventBus = eventBus;
    _logger = logger;
  }

  #region Microservice Permissions

  public async Task<bool> CreateBulkForMicroserviceAsync(string sourceId, List<FeatureGroupDefinitionEto> groups, List<FeatureDefinitionEto> features)
  {
    try
    {
      await _eventBus.PublishAsync(new CreateBulkFeaturesForMicroserviceRequestMsg
      {
        SourceId = sourceId,
        Features = features,
        Groups = groups,
      });

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

  #region Feature Management

  public async Task<List<FeatureDefinitionEto>> GetFeaturesAsync()
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<List<FeatureDefinitionEto>>>(new GetFeaturesQueryMsg());
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

  public async Task<FeatureDefinitionEto> CreateAsync(FeatureDefinitionEto entity)
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<FeatureDefinitionEto>>(
          new CreateFeatureDefinitionRequestMsg { Feature = entity });
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

  public async Task<FeatureDefinitionEto> UpdateAsync(FeatureDefinitionEto entity)
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<FeatureDefinitionEto>>(
          new UpdateFeatureDefinitionRequestMsg { Feature = entity });
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
      await _eventBus.PublishAsync(new DeleteFeatureRequestMsg { Name = name });
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

  #region Feature Groups Management
  public async Task<List<FeatureGroupDefinitionEto>> GetFeatureDynamicGroupCategories()
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<List<FeatureGroupDefinitionEto>>>(new GetFeaturesGroupsQueryMsg { });
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

  public async Task<FeatureGroupDefinitionEto> CreateGroupAsync(FeatureGroupDefinitionEto entity)
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<FeatureGroupDefinitionEto>>(
          new CreateFeatureGroupDefinitionRequestMsg { Group = entity });
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

  public async Task<FeatureGroupDefinitionEto> UpdateGroupAsync(FeatureGroupDefinitionEto entity)
  {
    try
    {
      var response = await _eventBus.RequestAsync<FeaturesServiceResponseMsg<FeatureGroupDefinitionEto>>(
          new UpdateFeatureGroupDefinitionRequestMsg { Group = entity });
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
      await _eventBus.PublishAsync(new DeleteFeatureGroupRequestMsg { Name = name });
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

