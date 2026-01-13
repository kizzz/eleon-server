using Common.EventBus.Module;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.DomainServices;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.EventHandlers;
public class StartHealthCheckEventHandler : IDistributedEventHandler<StartHealthCheckMsg>, ITransientDependency
{
  private readonly IVportalLogger<StartHealthCheckMsg> _logger;
  private readonly HealthCheckDomainService _healthCheckDomainService;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly IResponseContext _responseContext;

  public StartHealthCheckEventHandler(
      IVportalLogger<StartHealthCheckMsg> logger,
      HealthCheckDomainService healthCheckDomainService,
      IUnitOfWorkManager unitOfWorkManager,
      IResponseContext responseContext)
  {
    _logger = logger;
    _healthCheckDomainService = healthCheckDomainService;
    _unitOfWorkManager = unitOfWorkManager;
    _responseContext = responseContext;
  }

  public async Task HandleEventAsync(StartHealthCheckMsg eventData)
  {
    var response = new StartHealthCheckResponseMsg();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true);

      var healthCheck = await _healthCheckDomainService.CreateAsync(eventData.Type, eventData.InitiatorName, false);
      response.HealthCheckId = healthCheck.Id;

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
