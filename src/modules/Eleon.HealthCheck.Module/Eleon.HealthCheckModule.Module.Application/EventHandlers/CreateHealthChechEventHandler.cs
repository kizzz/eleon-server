using Common.EventBus.Module;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.DomainServices;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.EventHandlers;
public class CreateHealthChechEventHandler : IDistributedEventHandler<CreateHealthCheckRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<CreateHealthChechEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly HealthCheckDomainService _healthCheckDomainService;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly IObjectMapper _objectMapper;

  public CreateHealthChechEventHandler(
      IVportalLogger<CreateHealthChechEventHandler> logger,
      IResponseContext responseContext,
      HealthCheckDomainService healthCheckDomainService,
      IUnitOfWorkManager unitOfWorkManager,
      IObjectMapper objectMapper)
  {
    _logger = logger;
    _responseContext = responseContext;
    _healthCheckDomainService = healthCheckDomainService;
    _unitOfWorkManager = unitOfWorkManager;
    _objectMapper = objectMapper;
  }

  public async Task HandleEventAsync(CreateHealthCheckRequestMsg eventData)
  {

    var response = new CreateHealthCheckResponseMsg
    {
      Success = false,
    };

    try
    {
      var healthCheck = new Domain.Shared.Entities.HealthCheck(eventData.HealthCheck.Id)
      {
        InitiatorName = eventData.HealthCheck.InitiatorName,
        Type = eventData.HealthCheck.Type,
        Reports = _objectMapper.Map<List<HealthCheckReportEto>, List<HealthCheckReport>>(eventData.HealthCheck.Reports)
      };

      using var uow = _unitOfWorkManager.Begin(true);
      var result = await _healthCheckDomainService.SendAsync(healthCheck);
      await uow.SaveChangesAsync();
      await uow.CompleteAsync();

      response.Success = true;
      response.HealthCheckId = result.Id;
    }
    catch (Exception ex)
    {
      response.Error = ex.ToString();
      response.Success = false;
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
