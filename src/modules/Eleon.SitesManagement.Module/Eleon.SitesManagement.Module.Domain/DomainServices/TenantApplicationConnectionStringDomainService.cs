using LiteDB;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Shared.Repositories;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.DomainServices
{
  public class TenantApplicationConnectionStringDomainService : DomainService, ITransientDependency
  {
    private readonly IVportalLogger<TenantApplicationConnectionStringDomainService> logger;
    private readonly IApplicationTenantConnectionStringRepository _applicationTenantConnectionStringRepository;

    public TenantApplicationConnectionStringDomainService(
        IVportalLogger<TenantApplicationConnectionStringDomainService> logger,
        IApplicationTenantConnectionStringRepository applicationTenantConnectionStringRepository)
    {
      this.logger = logger;
      _applicationTenantConnectionStringRepository = applicationTenantConnectionStringRepository;
    }

    public async Task<List<ApplicationTenantConnectionStringEntity>> GetConnectionStrings(Guid? tenantId)
    {
      List<ApplicationTenantConnectionStringEntity> result = null;
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          result = await _applicationTenantConnectionStringRepository.GetListAsync();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ApplicationTenantConnectionStringEntity> AddConnectionString(Guid tenantId, string applicationName, string connectionString, string status = "")
    {
      ApplicationTenantConnectionStringEntity connectionStringEntity = null;
      try
      {
        connectionStringEntity = new ApplicationTenantConnectionStringEntity(GuidGenerator.Create())
        {
          ApplicationName = applicationName,
          ConnectionString = connectionString,
          Status = status,
          TenantId = tenantId
        };

        using (CurrentTenant.Change(tenantId))
        {
          await _applicationTenantConnectionStringRepository.InsertAsync(connectionStringEntity);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return connectionStringEntity;
    }

    public async Task<ApplicationTenantConnectionStringEntity> RemoveConnectionString(Guid tenantId, string applicationName)
    {
      ApplicationTenantConnectionStringEntity removedEntity = null;
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          removedEntity = await GetConnectionStringEntityAsync(applicationName);

          if (removedEntity != null)
          {
            await _applicationTenantConnectionStringRepository.DeleteAsync(removedEntity);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
      return removedEntity;
    }

    public async Task UpdateConnectionString(Guid tenantId, string applicationName, string newConnectionString, string status = "")
    {
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          var entity = await GetConnectionStringEntityAsync(applicationName);

          if (entity != null)
          {
            entity.ConnectionString = newConnectionString;
            entity.Status = status;

            await _applicationTenantConnectionStringRepository.UpdateAsync(entity);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<ApplicationTenantConnectionStringEntity?> GetAsync(Guid? tenantId, string applicationName)
    {
      ApplicationTenantConnectionStringEntity? result = null;
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          result = await GetConnectionStringEntityAsync(applicationName);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task SetConnectionStringAsync(Guid tenantId, string applicationName, string connectionString)
    {
      try
      {
        var entity = await GetConnectionStringEntityAsync(applicationName);

        if (entity != null)
        {
          if (!string.IsNullOrEmpty(connectionString))
          {
            entity.ConnectionString = connectionString;
            await _applicationTenantConnectionStringRepository.UpdateAsync(entity);
          }
          else
          {
            await _applicationTenantConnectionStringRepository.DeleteAsync(entity);
          }
        }
        else if (!string.IsNullOrEmpty(connectionString))
        {
          entity = new ApplicationTenantConnectionStringEntity(GuidGenerator.Create())
          {
            ApplicationName = applicationName,
            ConnectionString = connectionString
          };
          await _applicationTenantConnectionStringRepository.InsertAsync(entity);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    //public async Task<List<ApplicationTenantConnectionStringEntity>> GetNonExistentConnectionStrings(string applicationName)
    //{
    //    List<ApplicationTenantConnectionStringEntity> result = null;
    //    try
    //    {
    //        var defaultConnectionStrings = configuration.GetSection("ConnectionStrings").Get<List<ApplicationTenantConnectionStringEntity>>();
    //        result = defaultConnectionStrings?.Where(x => x.Name.Equals(applicationName, StringComparison.OrdinalIgnoreCase)).ToList();
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.Capture(ex);
    //    }

    //    return result;
    //}

    private async Task<ApplicationTenantConnectionStringEntity> GetConnectionStringEntityAsync(string applicationName)
    {
      return await (await _applicationTenantConnectionStringRepository
          .GetDbSetAsync())
          .Where(x => x.ApplicationName == applicationName)
          .FirstOrDefaultAsync();
    }
  }
}


