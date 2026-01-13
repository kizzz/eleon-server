using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.DomainServices
{
  public class ResourceDomainService : DomainService
  {
    private readonly ModuleDomainService modulesDomainService;
    private readonly IVportalLogger<ResourceDomainService> logger;
    private readonly ClientApplicationDomainService clientApplicationDomainService;

    public ResourceDomainService(
        ModuleDomainService modulesDomainService,
        IVportalLogger<ResourceDomainService> logger,
        ClientApplicationDomainService clientApplicationDomainService)
    {
      this.modulesDomainService = modulesDomainService;
      this.logger = logger;
      this.clientApplicationDomainService = clientApplicationDomainService;
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
        return await modulesDomainService.GetByAsync(c => c.Type == ModuleType.Server || c.Type == ModuleType.Resource);
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

    public async Task<ModuleEntity> CreateAsync(string displayName, string path, bool isEnabled, string source, ModuleType type)
    {
      try
      {
        return await modulesDomainService.CreateAsync(displayName, path, isEnabled, source, type);
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

    public async Task<ModuleEntity> UpdateAsync(Guid id, string displayName, string path, bool isEnabled, string source, ModuleType type)
    {
      ModuleEntity result = null;
      try
      {
        result = await modulesDomainService.UpdateAsync(id, displayName, path, isEnabled, source, type);
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

        await modulesDomainService.GetByAsync(m => (m.Type == ModuleType.Resource || m.Type == ModuleType.Server) && moduleIds.Contains(m.Id));
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

    public async Task<List<ModuleEntity>> GetEnabledAsync()
    {
      List<ModuleEntity> result = null;
      try
      {
        result = await modulesDomainService.GetByAsync(m => (m.Type == ModuleType.Resource || m.Type == ModuleType.Server) && m.IsEnabled);
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


