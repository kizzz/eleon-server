using Common.EventBus.Module;
using EleonsoftAbp.Messages.AppConfig;
using Logging.Module;
using Messaging.Module.ETO;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using NATS.Client.JetStream.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Repositories;

namespace VPortal.SitesManagement.Module.EventHandlers;

public class ApplicationConfigurationEnrichmentEventHandler
    : IDistributedEventHandler<EnrichApplicationsConfigurationRequestMsg>,
      ITransientDependency
{
  private readonly IVportalLogger<ApplicationConfigurationEnrichmentEventHandler> _logger;
  private readonly ModuleSettingsManager _moduleSettingsManager;
  private readonly IClientApplicationRepository _clientApplicationRepository;
  private readonly IResponseContext _responseContext;
  private readonly ICurrentTenant _currentTenant;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public ApplicationConfigurationEnrichmentEventHandler(
      IVportalLogger<ApplicationConfigurationEnrichmentEventHandler> logger,
      ModuleSettingsManager moduleSettingsManager,
      IClientApplicationRepository clientApplicationRepository,
      IResponseContext responseContext,
      ICurrentTenant currentTenant,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _logger = logger;
    _moduleSettingsManager = moduleSettingsManager;
    _clientApplicationRepository = clientApplicationRepository;
    _responseContext = responseContext;
    _currentTenant = currentTenant;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(EnrichApplicationsConfigurationRequestMsg eventData)
  {

    var response = new EnrichedApplicationConfigurationResponseMsg
    {
      IsSuccess = false,
      Applications = [],
      ErrorMessage = string.Empty,
      TenantId = eventData.TenantId
    };

    try
    {
      using (_currentTenant.Change(eventData.TenantId))
      {
        using var uow = _unitOfWorkManager.Begin(requiresNew: true);
        var settings = await _moduleSettingsManager.GetAsync();
        var clientApps = settings.ClientApplications;
        response.IsSuccess = true;
        response.Applications = clientApps;
        response.ErrorMessage = string.Empty;
      }
    }
    catch (Exception ex)
    {
      response.IsSuccess = false;
      response.ErrorMessage = ex.Message;
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
