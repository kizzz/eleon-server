using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.UiModules
{
  public class UiModuleAppService : SitesManagementAppService, IUiModuleAppService
  {
    private readonly UiModuleManager _uiModuleManager;
    private readonly IVportalLogger<UiModuleAppService> _logger;
    private readonly ModuleSettingsManager moduleSettingsManager;

    public UiModuleAppService(UiModuleManager uiModuleManager, IVportalLogger<UiModuleAppService> logger,
        ModuleSettingsManager moduleSettingsManager)
    {
      _uiModuleManager = uiModuleManager;
      _logger = logger;
      this.moduleSettingsManager = moduleSettingsManager;
    }

    public async Task<EleoncoreModuleDto> GetAsync(Guid id)
    {
      EleoncoreModuleDto result = null;
      try
      {
        var entity = await _uiModuleManager.GetAsync(id);
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(entity);
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

    public async Task<List<EleoncoreModuleDto>> GetAllAsync()
    {
      List<EleoncoreModuleDto> result = null;
      try
      {
        var entities = await _uiModuleManager.GetAllAsync();
        result = ObjectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleDto>>(entities);
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

    public async Task<EleoncoreModuleDto> CreateAsync(EleoncoreModuleDto input)
    {
      EleoncoreModuleDto result = null;
      try
      {
        var uiModule = await _uiModuleManager.CreateAsync(
            input.DisplayName,
            input.Path,
            input.IsEnabled,
            input.Source
        );
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(uiModule);
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

    public async Task<EleoncoreModuleDto> UpdateAsync(Guid id, EleoncoreModuleDto input)
    {
      EleoncoreModuleDto result = null;
      try
      {
        var updatedUiModule = await _uiModuleManager.UpdateAsync(
            id,
            input.IsEnabled,
            input.DisplayName,
            input.Path,
            input.Source
        );

        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(updatedUiModule);
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

    public async Task DeleteAsync(Guid id)
    {
      try
      {
        await _uiModuleManager.DeleteAsync(id);
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



    public async Task<List<EleoncoreModuleDto>> GetEnabledModulesAsync()
    {
      List<EleoncoreModuleDto> result = null;
      try
      {
        var entities = await _uiModuleManager.GetEnabledModulesAsync();
        result = ObjectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleDto>>(entities);
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


    public async Task<List<EleoncoreModuleDto>> GetModulesByApplicationAsync(Guid applicationId)
    {
      List<EleoncoreModuleDto> result = new List<EleoncoreModuleDto>();
      try
      {

        var entities = await _uiModuleManager.GetModulesByApplicationIdAsync(applicationId);
        List<EleoncoreModuleDto> dtos = ObjectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleDto>>(entities);
        result.AddRange(dtos);
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
  }
}


