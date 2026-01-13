using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Managers
{
  public class UiModuleManager : DomainService
  {

    private readonly IConfiguration configuration;
    private readonly ModuleDomainService modulesDomainService;
    private readonly ClientApplicationDomainService clientApplicationDomainService;
    private readonly IVportalLogger<UiModuleManager> logger;

    public UiModuleManager(
        ModuleDomainService modulesDomainService,
        ClientApplicationDomainService clientApplicationDomainService,
        IConfiguration configuration,
        IVportalLogger<UiModuleManager> logger)
    {
      this.configuration = configuration;
      this.modulesDomainService = modulesDomainService;
      this.clientApplicationDomainService = clientApplicationDomainService;
      this.logger = logger;
    }

    public async Task<bool> GetSafeMode()
    {

      bool result = false;
      try
      {
        result = configuration.GetValue<bool>("IsSafeMode");
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
    public async Task<ModuleEntity> GetAsync(Guid id)
    {
      ModuleEntity result = null;
      try
      {
        result = await modulesDomainService.GetAsync(id);
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

    public async Task<List<ModuleEntity>> GetAllAsync()
    {
      try
      {
        return await modulesDomainService.GetByAsync(c => c.Type == ModuleType.Client);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return null;
    }

    public async Task<ModuleEntity> CreateAsync(string displayName, string path, bool isEnabled, string source)
    {
      try
      {
        return await modulesDomainService.CreateAsync(displayName, path, isEnabled, source, ModuleType.Client);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return null;
    }

    public async Task<ModuleEntity> UpdateAsync(Guid id, bool isEnabled, string displayName, string path, string source)
    {
      ModuleEntity result = null;
      try
      {
        result = await modulesDomainService.UpdateAsync(id, displayName, path, isEnabled, source, ModuleType.Client);
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
        await modulesDomainService.DeleteAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<List<ModuleEntity>> GetModulesByApplicationIdAsync(Guid applicationId)
    {
      List<ModuleEntity> result = null;
      try
      {
        var application = await clientApplicationDomainService.GetAsync(applicationId);
        var moduleIds = application.Modules.Select(m => m.Id);

        await modulesDomainService.GetByAsync(m => m.Type == ModuleType.Client && moduleIds.Contains(m.Id));
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


    public async Task<List<ModuleEntity>> GetEnabledModulesAsync()
    {
      List<ModuleEntity> result = null;
      try
      {
        result = await modulesDomainService.GetByAsync(m => m.Type == ModuleType.Client && m.IsEnabled);
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
  }
}


