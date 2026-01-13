using Common.EventBus.Module;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
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
public class AddReportEventHandler : IDistributedEventHandler<AddHealthCheckReportMsg>, IDistributedEventHandler<AddHealthCheckReportBulkMsg>, ITransientDependency
{
  private readonly IVportalLogger<AddReportEventHandler> _logger;
  private readonly HealthCheckDomainService _healthCheckDomainService;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly IResponseContext _responseContext;
  private readonly IObjectMapper _objectMapper;

  public AddReportEventHandler(IVportalLogger<AddReportEventHandler> logger,
      HealthCheckDomainService healthCheckDomainService,
      IUnitOfWorkManager unitOfWorkManager,
      IResponseContext responseContext,
      IObjectMapper objectMapper
      )
  {
    _logger = logger;
    _healthCheckDomainService = healthCheckDomainService;
    _unitOfWorkManager = unitOfWorkManager;
    _responseContext = responseContext;
    _objectMapper = objectMapper;
  }

  public async Task HandleEventAsync(AddHealthCheckReportMsg eventData)
  {
    var response = new AddHealthCheckReportResponseMsg();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true);

      response.ReportId = await AddReportAsync(eventData.HealthCheckReport);

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();

      response.IsSuccess = true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(AddHealthCheckReportBulkMsg eventData)
  {
    var response = new AddHealthCheckReportBulkResponseMsg();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true);

      foreach (var report in eventData.HealthCheckReports)
      {
        response.ReportIds.Add(await AddReportAsync(report));
      }

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();

      response.IsSuccess = true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  private async Task<Guid> AddReportAsync(HealthCheckReportEto eventData)
  {
    var result = await _healthCheckDomainService.AddReportAsync(
        eventData.HealthCheckId,
        eventData.ServiceName,
        eventData.ServiceVersion,
        eventData.UpTime,
        eventData.CheckName,
        eventData.Status,
        eventData.Message,
        eventData.IsPublic,
        _objectMapper.Map<List<ReportExtraInformationEto>, List<ReportExtraInformation>>(eventData.ExtraInformation));
    return result.Id;
  }
}
