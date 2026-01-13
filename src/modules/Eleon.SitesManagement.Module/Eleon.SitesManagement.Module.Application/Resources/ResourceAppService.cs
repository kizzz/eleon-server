using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Resources
{
  public class ResourceAppService : SitesManagementAppService, IResourceAppService
  {
    private readonly IVportalLogger<ResourceAppService> logger;
    private readonly ResourceDomainService resourceDomainService;

    public ResourceAppService(
        IVportalLogger<ResourceAppService> logger,
        ResourceDomainService resourceDomainService)
    {
      this.logger = logger;
      this.resourceDomainService = resourceDomainService;
    }

    public async Task<EleoncoreModuleDto> GetAsync(Guid id)
    {
      EleoncoreModuleDto result = null;
      try
      {
        var entity = await resourceDomainService.GetAsync(id);
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<List<EleoncoreModuleDto>> GetAllAsync()
    {
      List<EleoncoreModuleDto> result = null;
      try
      {
        var entities = await resourceDomainService.GetAllAsync();
        result = ObjectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<EleoncoreModuleDto> CreateAsync(EleoncoreModuleDto input)
    {

      EleoncoreModuleDto result = null;
      try
      {
        var entity = await resourceDomainService.CreateAsync(
            input.DisplayName,
            input.Path,
            input.IsEnabled,
            input.Source,
            input.Type
            );
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(entity);
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

    public async Task<EleoncoreModuleDto> UpdateAsync(EleoncoreModuleDto input)
    {

      EleoncoreModuleDto result = null;
      try
      {
        var entity = await resourceDomainService.UpdateAsync(
            input.Id,
            input.DisplayName,
            input.Path,
            input.IsEnabled,
            input.Source,
            input.Type
            );
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(entity);
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
        await resourceDomainService.DeleteAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}


