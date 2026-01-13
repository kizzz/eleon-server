using Logging.Module;
using Microsoft.AspNetCore.Http;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.AppConfiguration;
public class EleoncoreApplicationConfigurationAppService : SitesManagementAppService, IEleoncoreApplicationConfigurationAppService
{
  private readonly IVportalLogger<EleoncoreApplicationConfigurationAppService> _logger;
  private readonly EleoncoreApplicationConfigurationDomainService domainService;
  private readonly IHttpContextAccessor httpContextAccessor;

  public EleoncoreApplicationConfigurationAppService(
      IVportalLogger<EleoncoreApplicationConfigurationAppService> logger,
      EleoncoreApplicationConfigurationDomainService domainService,
      IHttpContextAccessor httpContextAccessor
      )
  {
    _logger = logger;
    this.domainService = domainService;
    this.httpContextAccessor = httpContextAccessor;
  }

  public async Task<EleoncoreApplicationConfigurationDto> Get()
  {

    EleoncoreApplicationConfigurationDto result = null;
    try
    {
      var referer = httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
      string basicPath = null;

      if (Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
      {
        basicPath = refererUri.AbsolutePath; // This gives just the path, like /apps/admin
      }

      if (basicPath != null)
      {
        basicPath = basicPath.Split("/")
            .Take(3)
            .JoinAsString("/");
      }
      EleoncoreApplicationConfigurationValueObject configuration = await domainService.GetAsync(basicPath);

      result = ObjectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(configuration);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  /// <summary>
  /// AppId is the base path of this application
  /// </summary>
  /// <param name="appId"></param>
  /// <returns></returns>
  public async Task<EleoncoreApplicationConfigurationDto> GetByAppIdAsync(string appId)
  {

    EleoncoreApplicationConfigurationDto result = null;
    try
    {

      if (appId != null)
      {
        appId = appId.Split("/")
            .Take(3)
            .JoinAsString("/");
      }
      EleoncoreApplicationConfigurationValueObject configuration = await domainService.GetAsync(appId);

      result = ObjectMapper.Map<EleoncoreApplicationConfigurationValueObject, EleoncoreApplicationConfigurationDto>(configuration);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }
}


