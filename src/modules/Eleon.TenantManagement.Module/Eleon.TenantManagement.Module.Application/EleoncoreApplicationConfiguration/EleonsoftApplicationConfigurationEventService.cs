using Common.EventBus.Module;
using EleonsoftAbp.Messages.AppConfig;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;

namespace EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.EleoncoreApplicationConfiguration;
public class EleonsoftApplicationConfigurationEventService : IDistributedEventHandler<GetBaseAppConfigRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<EleonsoftApplicationConfigurationEventService> _logger;
  private readonly EleonsoftApplicationConfigurationDomainService eleoncoreApplicationConfigurationDomainService;
  private readonly IResponseContext _responseContext;
  private readonly IObjectMapper _objectMapper;
  private readonly AbpApplicationConfigurationAppService _abpApplicationConfigurationAppService;

  public EleonsoftApplicationConfigurationEventService(
      IVportalLogger<EleonsoftApplicationConfigurationEventService> logger,
      EleonsoftApplicationConfigurationDomainService eleoncoreApplicationConfigurationDomainService,
      IResponseContext responseContext,
      IObjectMapper objectMapper,
      AbpApplicationConfigurationAppService abpApplicationConfigurationAppService)
  {
    _logger = logger;
    this.eleoncoreApplicationConfigurationDomainService = eleoncoreApplicationConfigurationDomainService;
    _responseContext = responseContext;
    _objectMapper = objectMapper;
    _abpApplicationConfigurationAppService = abpApplicationConfigurationAppService;

  }

  public async Task HandleEventAsync(GetBaseAppConfigRequestMsg eventData)
  {
    var response = new GetBaseAppConfigResponseMsg
    {
      Success = false,
      ApplicationConfiguration = new EleoncoreApplicationConfigurationValueObject()
    };
    try
    {
      var baseSettings = await eleoncoreApplicationConfigurationDomainService.GetBaseAsync(eventData.UserId);
      response.ApplicationConfiguration = baseSettings;
      response.Success = true;
    }
    catch (Exception e)
    {
      _logger.CaptureAndSuppress(e);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
