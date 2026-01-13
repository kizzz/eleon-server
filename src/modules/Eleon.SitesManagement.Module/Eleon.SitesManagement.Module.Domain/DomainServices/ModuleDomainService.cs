using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers;
using System.Linq.Expressions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using SharedModule.modules.Helpers.Module;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Localization;
using VPortal.SitesManagement.Module.Repositories;
using Microsoft.Extensions.Logging;

namespace VPortal.SitesManagement.Module.Microservices;

public class ModuleDomainService : DomainService
{
  private readonly DefaultApplicationsDomainService defaultApplicationsDomainService;
  private readonly IVportalLogger<ModuleDomainService> logger;
  private readonly IModulesRepository modulesRepository;
  private readonly ClientApplicationDomainService clientApplicationDomainService;
  private readonly IStringLocalizer<SitesManagementResource> localizer;

  public ModuleDomainService(
      DefaultApplicationsDomainService defaultApplicationsDomainService,
      IVportalLogger<ModuleDomainService> logger,
      IModulesRepository microserviceRepository,
      ClientApplicationDomainService clientApplicationDomainService,
      IConfiguration configuration,
      IStringLocalizer<SitesManagementResource> localizer)
  {
    this.defaultApplicationsDomainService = defaultApplicationsDomainService;
    this.logger = logger;
    this.modulesRepository = microserviceRepository;
    this.clientApplicationDomainService = clientApplicationDomainService;
    this.localizer = localizer;
    ;
  }

  public async Task<List<ApplicationModuleEntity>> GetByAppId(Guid applicationId)
  {

    List<ApplicationModuleEntity> moduleSettings = null;
    try
    {
      var application = await clientApplicationDomainService.GetAsync(applicationId);
      moduleSettings = application.Modules.ToList();
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
    return moduleSettings;
  }

  public async Task<ModuleEntity> GetAsync(Guid id)
  {
    ModuleEntity result = null;
    try
    {

      result = GetConstantModules().FirstOrDefault(m => m.Id == id) ?? await modulesRepository.GetAsync(id);
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

  public async Task<ModuleEntity> FindAsync(Guid id)
  {

    try
    {
      return GetConstantModules().FirstOrDefault(m => m.Id == id) ?? await modulesRepository.FindAsync(id);
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

  public async Task<List<ModuleEntity>> GetByAsync(Expression<Func<ModuleEntity, bool>> predicate)
  {
    try
    {
      var dbSet = await modulesRepository.GetDbSetAsync();
      var modules = await dbSet.Where(predicate).ToListAsync();

      var constantModules = GetConstantModules().Where(predicate.Compile()).ToList();
      modules.AddRange(constantModules);
      return modules;
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

  public async Task<List<ModuleEntity>> GetListAsync()
  {
    try
    {
      var dbModules = await modulesRepository
          .GetListAsync(true);
      dbModules.AddRange(GetConstantModules());
      return dbModules;
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

  public async Task<ModuleEntity> CreateAsync(string displayName, string path, bool isEnabled, string source, ModuleType moduleType)
  {
    try
    {
      var module = new ModuleEntity(GuidGenerator.Create())
      {
        DisplayName = displayName,
        Path = path,
        IsEnabled = isEnabled,
        Type = moduleType,
        Source = source,
      };

      return await modulesRepository.InsertAsync(module, true);
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

  public async Task<ModuleEntity> UpdateAsync(Guid id, string displayName, string path, bool isEnabled, string source, ModuleType moduleType)
  {
    try
    {
      var module = await GetAsync(id);

      if (module.IsSystem)
      {
        throw new UserFriendlyException(localizer["ConstnantModules:Update:Forbidden"]);
      }

      // Idempotency: check if already in desired state
      if (module.DisplayName == displayName &&
          module.Path == path &&
          module.IsEnabled == isEnabled &&
          module.Type == moduleType &&
          module.Source == source)
      {
        return module;
      }

      module.DisplayName = displayName;
      module.Path = path;
      module.IsEnabled = isEnabled;
      module.Type = moduleType;
      module.Source = source;

      try
      {
        return await modulesRepository.UpdateAsync(module, true);
      }
      catch (AbpDbConcurrencyException ex)
      {
        logger.Log.LogWarning(
          ex,
          "Concurrency conflict while updating module {ModuleId}. Waiting for desired state...",
          id);

        return await ConcurrencyExtensions.WaitForDesiredStateAsync(
          async () =>
          {
            var currentModule = await GetAsync(id);
            var isDesired =
              currentModule.DisplayName == displayName &&
              currentModule.Path == path &&
              currentModule.IsEnabled == isEnabled &&
              currentModule.Type == moduleType &&
              currentModule.Source == source;

            var details =
              $"DisplayName={currentModule.DisplayName},Path={currentModule.Path},IsEnabled={currentModule.IsEnabled},Type={currentModule.Type},Source={currentModule.Source}";

            return new ConcurrencyExtensions.ConcurrencyWaitResult<ModuleEntity>(isDesired, currentModule, details);
          },
          logger.Log,
          "UpdateModule",
          id
        );
      }
    }
    catch (AbpDbConcurrencyException)
    {
      throw; // Re-throw after handling above
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

  public async Task DeleteAsync(Guid id)
  {
    try
    {
      foreach (var module in GetConstantModules())
      {
        if (module.Id == id)
        {
          throw new UserFriendlyException(localizer["ConstnatModules:Delete:Forbidden"]);
        }
      }

      await modulesRepository.DeleteAsync(id);
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
  }

  private List<ModuleEntity> GetConstantModules()
  {
    return defaultApplicationsDomainService.GetConstantModules();
  }
}
