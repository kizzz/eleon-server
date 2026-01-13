using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.DomainServices;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;
using VPortal.HealthCheckModule.Module;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.HealthCheck;
public class HealthCheckAppService : HealthCheckModuleAppService, IHealthCheckAppService
{
  private readonly HealthCheckDomainService _healthCheckDomainService;
  private readonly IVportalLogger<HealthCheckAppService> _logger;

  public HealthCheckAppService(
      HealthCheckDomainService healthCheckDomainService,
      IVportalLogger<HealthCheckAppService> logger)
  {
    _healthCheckDomainService = healthCheckDomainService;
    _logger = logger;
  }

  public async Task<HealthCheckReportDto> AddReportAsync(AddHealthCheckReportDto request)
  {

    try
    {
      var report = await _healthCheckDomainService.AddReportAsync(
          request.HealthCheckId,
          request.ServiceName,
          request.ServiceVersion,
          request.UpTime,
          request.CheckName,
          request.Status,
          request.Message,
          request.IsPublic,
          ObjectMapper.Map<List<ReportExtraInformationDto>, List<ReportExtraInformation>>(request.ExtraInformation)
          );

      var result = ObjectMapper.Map<HealthCheckReport, HealthCheckReportDto>(report);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while adding health check report.");
      throw;
    }
    finally
    {
    }
  }

  public async Task<bool> AddReportBulkAsync(AddHealthCheckReportBulkDto request)
  {

    try
    {
      var result = true;
      foreach (var report in request.Reports)
      {
        try
        {
          await _healthCheckDomainService.AddReportAsync(
              report.HealthCheckId,
              report.ServiceName,
              report.ServiceVersion,
              report.UpTime,
              report.CheckName,
              report.Status,
              report.Message,
              report.IsPublic,
              ObjectMapper.Map<List<ReportExtraInformationDto>, List<ReportExtraInformation>>(report.ExtraInformation)
              );
        }
        catch (Exception ex)
        {
          _logger.CaptureAndSuppress(ex, "An error occurred while adding health check report in bulk operation.");
          result = false;
        }
      }

      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while adding health check report.");
      throw;
    }
    finally
    {
    }
  }

  public async Task<HealthCheckDto> CreateAsync(CreateHealthCheckDto request)
  {

    try
    {
      var healthCheck = await _healthCheckDomainService.CreateAsync(
          request.Type,
          request.InitiatorName,
          false
          );

      var result = ObjectMapper.Map<Domain.Shared.Entities.HealthCheck, HealthCheckDto>(healthCheck);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while adding health check report.");
      throw;
    }
    finally
    {
    }
  }

  public async Task<HealthCheckDto> SendAsync(SendHealthCheckDto request)
  {
    try
    {
      var entity = ObjectMapper.Map<SendHealthCheckDto, Domain.Shared.Entities.HealthCheck>(request);

      var healthCheck = await _healthCheckDomainService.SendAsync(entity);

      var result = ObjectMapper.Map<Domain.Shared.Entities.HealthCheck, HealthCheckDto>(healthCheck);
      return result;
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

  public async Task<FullHealthCheckDto> GetByIdAsync(Guid id)
  {

    try
    {
      var healthCheck = await _healthCheckDomainService.GetAsync(id);

      var result = ObjectMapper.Map<Domain.Shared.Entities.HealthCheck, FullHealthCheckDto>(healthCheck);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while adding health check report.");
      throw;
    }
    finally
    {
    }
  }

  public async Task<PagedResultDto<HealthCheckDto>> GetListAsync(HealthCheckRequestDto request)
  {

    try
    {
      var healthChecks = await _healthCheckDomainService.GetListAsync(request.Sorting, request.SkipCount, request.MaxResultCount, request.Type, request.Initiator, request.MinTime, request.MaxTime);
      var result = ObjectMapper.Map<List<Domain.Shared.Entities.HealthCheck>, List<HealthCheckDto>>(healthChecks.Value);
      return new PagedResultDto<HealthCheckDto>(healthChecks.Key, result);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while adding health check report.");
      throw;
    }
    finally
    {
    }
  }
}
