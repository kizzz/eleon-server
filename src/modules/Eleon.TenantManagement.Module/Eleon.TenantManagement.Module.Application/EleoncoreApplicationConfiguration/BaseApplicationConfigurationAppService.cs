using Logging.Module;
using Microsoft.AspNetCore.Http;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.AppConfiguration;
public class BaseApplicationConfigurationAppService : TenantManagementAppService, IBaseApplicationConfigurationAppService
{
  private readonly IVportalLogger<BaseApplicationConfigurationAppService> _logger;
  private readonly EleonsoftApplicationConfigurationDomainService domainService;
  private readonly IHttpContextAccessor httpContextAccessor;

  public BaseApplicationConfigurationAppService(
      IVportalLogger<BaseApplicationConfigurationAppService> logger,
      EleonsoftApplicationConfigurationDomainService domainService,
      IHttpContextAccessor httpContextAccessor
      )
  {
    _logger = logger;
    this.domainService = domainService;
    this.httpContextAccessor = httpContextAccessor;
  }

  public async Task<EleoncoreApplicationConfigurationDto> GetBaseAsync()
  {
    try
    {
      var configuration = await domainService.GetBaseAsync(null);
      return ObjectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(configuration);
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
}
