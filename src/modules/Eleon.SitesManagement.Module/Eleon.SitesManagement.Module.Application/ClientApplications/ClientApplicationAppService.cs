using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Microservices;
using Location = ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations.Location;

namespace VPortal.SitesManagement.Module.Services
{
  [Authorize]
  [ExposeServices(typeof(IClientApplicationAppService))]
  public class ClientApplicationAppService : SitesManagementAppService, IClientApplicationAppService
  {
    private readonly ClientApplicationDomainService _clientApplicationManager;
    private readonly IVportalLogger<ClientApplicationAppService> _logger;
    private readonly ModuleDomainService moduleDomainService;
    private readonly ModuleSettingsManager moduleSettingsManager;
    private readonly IDistributedEventBus eventBus;

    public ClientApplicationAppService(
        ClientApplicationDomainService clientApplicationManager,
        IVportalLogger<ClientApplicationAppService> logger,
        ModuleDomainService moduleDomainService,
        ModuleSettingsManager moduleSettingsManager,
        IDistributedEventBus eventBus)
    {
      _clientApplicationManager = clientApplicationManager;
      _logger = logger;
      this.moduleDomainService = moduleDomainService;
      this.moduleSettingsManager = moduleSettingsManager;
      this.eventBus = eventBus;
    }

    [AllowAnonymous]
    public async Task<ClientApplicationDto> GetAsync(Guid id)
    {
      ClientApplicationDto result = null;
      try
      {
        var entity = await _clientApplicationManager.GetAsync(id);
        result = ObjectMapper.Map<ApplicationEntity, ClientApplicationDto>(entity);
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

    [AllowAnonymous]
    public async Task<List<ClientApplicationDto>> GetByTenantIdAsync(Guid? tenantId)
    {
      List<ClientApplicationDto> result = null;
      try
      {
        var entities = await _clientApplicationManager.GetByTenantIdAsync(tenantId);
        result = ObjectMapper.Map<List<ApplicationEntity>, List<ClientApplicationDto>>(entities);
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

    [AllowAnonymous]
    public async Task<List<FullClientApplicationDto>> GetAllAsync()
    {
      List<FullClientApplicationDto> result = null;
      try
      {
        var entities = await _clientApplicationManager.GetAllAsync();
        result = ObjectMapper.Map<List<ApplicationEntity>, List<FullClientApplicationDto>>(entities);

        foreach (var app in result)
        {
          var appModuleSettings = await moduleDomainService.GetByAppId(app.Id);
          app.Modules = ObjectMapper.Map<List<ApplicationModuleEntity>, List<ApplicationModuleDto>>(appModuleSettings);
        }
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

    public async Task<ClientApplicationDto> CreateAsync(CreateClientApplicationDto input)
    {
      ClientApplicationDto result = null;
      try
      {
        var properties = input.Properties == null ? null : ObjectMapper.Map<List<ClientApplicationPropertyDto>, List<ApplicationPropertyEntity>>(input.Properties);

        var clientApplication = await _clientApplicationManager.CreateAsync(
            input.Name,
            input.Path,
            input.Source,
            input.IsEnabled,
            input.IsAuthenticationRequired,
            input.FrameworkType,
            input.StyleType,
            input.ClientApplicationType,
            input.Icon,
            parentId: input.ParentId,
            appType: input.AppType,
            properties: properties
        );
        result = ObjectMapper.Map<ApplicationEntity, ClientApplicationDto>(clientApplication);

        await moduleSettingsManager.RefreshAsync();
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

    public async Task<ClientApplicationDto> UpdateAsync(Guid id, UpdateClientApplicationDto input)
    {
      ClientApplicationDto result = null;
      try
      {
        var clientApplication = await _clientApplicationManager.GetAsync(id);
        if (clientApplication == null)
        {
          throw new EntityNotFoundException(typeof(ApplicationEntity), id);
        }

        var properties = input.Properties == null ? null : ObjectMapper.Map<List<ClientApplicationPropertyDto>, List<ApplicationPropertyEntity>>(input.Properties);

        var updatedClientApplication = await _clientApplicationManager.UpdateAsync(
            clientApplication,
            input.Name,
            input.Path,
            input.Source,
            input.IsEnabled,
            input.IsAuthenticationRequired,
            input.FrameworkType,
            input.StyleType,
            input.ClientApplicationType,
            isDefault: input.IsDefault,
            input.Icon,
            properties: properties
        );

        result = ObjectMapper.Map<ApplicationEntity, ClientApplicationDto>(updatedClientApplication);
        await moduleSettingsManager.RefreshAsync();
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

    public async Task UseDedicatedDatabaseAsync(Guid id, bool useDedicateDb)
    {
      try
      {
        var clientApplication = await _clientApplicationManager.GetAsync(id);
        if (clientApplication == null)
        {
          throw new EntityNotFoundException(typeof(ApplicationEntity), id);
        }

        if (clientApplication.UseDedicatedDatabase != useDedicateDb)
        {
          var updatedClientApplication = await _clientApplicationManager.UpdateAsync(
              clientApplication,
              clientApplication.Name,
              clientApplication.Path,
              clientApplication.Source,
              clientApplication.IsEnabled,
              clientApplication.IsAuthenticationRequired,
              clientApplication.FrameworkType,
              clientApplication.StyleType,
              clientApplication.ClientApplicationType,
              clientApplication.IsDefault,
              clientApplication.Icon,
              useDedicateDb
          );

          var repsponse = await eventBus.RequestAsync<UseDedicatedGotMsg>(new UseDedicatedMsg
          {
            ApplicationName = updatedClientApplication.Name,
            UseDedicated = updatedClientApplication.UseDedicatedDatabase
          });

          if (!repsponse.Success)
          {
            Logger.LogError("Cannot save client application settings because use dedicated response was not successful");
            throw new Exception("Cannot save client application settings because use dedicated response was not successful");
          }

          await moduleSettingsManager.RefreshAsync();
        }
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task DeleteAsync(Guid id)
    {
      try
      {
        await _clientApplicationManager.DeleteAsync(id);
        await moduleSettingsManager.RefreshAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }
    }

    [AllowAnonymous]
    public async Task<List<ClientApplicationDto>> GetEnabledApplicationsAsync()
    {
      List<ClientApplicationDto> result = null;
      try
      {
        var entities = await _clientApplicationManager.GetEnabledApplicationsAsync();
        result = ObjectMapper.Map<List<ApplicationEntity>, List<ClientApplicationDto>>(entities);
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

    public async Task<bool> AddModuleToApplication(ApplicationModuleDto addApplicationModuleDto)
    {

      bool result = false;
      try
      {
        var applicationModulesEntity = ObjectMapper.Map<ApplicationModuleDto, ApplicationModuleEntity>(addApplicationModuleDto);

        await _clientApplicationManager.AddBulkModulesToApplication(new List<ApplicationModuleEntity>()
                {
                    applicationModulesEntity,
                });

        result = true;
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

    public async Task<bool> AddBulkModulesToApplication(List<ApplicationModuleDto> applicationModuleDtos)
    {
      bool result = false;
      try
      {
        var applicationModulesEntities = ObjectMapper.Map<List<ApplicationModuleDto>, List<ApplicationModuleEntity>>(applicationModuleDtos);
        result = await _clientApplicationManager.AddBulkModulesToApplication(applicationModulesEntities);
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

    public async Task<bool> RemoveModuleFromApplication(Guid applicationId, Guid moduleId)
    {

      bool result = false;
      try
      {
        var entity = await _clientApplicationManager.RemoveModuleFromApplication(applicationId, moduleId);
        result = true;
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
    [AllowAnonymous]
    public async Task<ModuleSettingsDto> GetSetting()
    {
      ModuleSettingsDto result = null;
      try
      {
        var msg = await moduleSettingsManager.GetAsync();
        result = ObjectMapper.Map<ModuleSettingsGotMsg, ModuleSettingsDto>(msg);
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

    public async Task<ClientApplicationDto> GetSiteByHostnameAsync(string hostname)
    {
      try
      {
        var app = await moduleSettingsManager.GetSiteByHostnameAsync(hostname);
        var result = ObjectMapper.Map<ApplicationEntity, ClientApplicationDto>(app);
        return result;
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

    [AllowAnonymous]
    public async Task<ModuleSettingsDto> GetAppsSettingsBySiteIdAsync(Guid siteId)
    {
      ModuleSettingsDto result = null;
      try
      {
        var msg = await moduleSettingsManager.GetBySiteIdAsync(siteId);
        result = ObjectMapper.Map<ModuleSettingsGotMsg, ModuleSettingsDto>(msg);
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

    [AllowAnonymous]
    public async Task<List<Location>> GetLocationsAsync()
    {

      try
      {
        return await moduleSettingsManager.GetLocationsAsync();
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return null;
    }

    [AllowAnonymous]
    public async Task<List<Location>> GetLocationsBySiteIdAsync(Guid siteId)
    {

      try
      {
        return await moduleSettingsManager.GetLocationsBySiteIdAsync(siteId);
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return null;
    }

    [AllowAnonymous]
    public async Task<ClientApplicationDto> GetDefaultApplicationAsync()
    {

      try
      {
        var entity = await _clientApplicationManager.GetDefaultApplicationAsync();
        return ObjectMapper.Map<ApplicationEntity, ClientApplicationDto>(entity);
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return null;
    }
  }
}


