using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.HealthCheckModule.Module;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.HttpApi.Controllers;

[Area(HealthCheckRemoteServiceConsts.ModuleName)]
[RemoteService(Name = HealthCheckRemoteServiceConsts.RemoteServiceName)]
[Route("api/HealthChecks/HealthCheck")]
public class HealthCheckController : HealthCheckModuleController, IHealthCheckAppService
{
  private readonly IVportalLogger<HealthCheckController> _logger;
  private readonly IHealthCheckAppService _healthCheckAppService;

  public HealthCheckController(
      IVportalLogger<HealthCheckController> logger,
      IHealthCheckAppService healthCheckAppService)
  {
    _logger = logger;
    _healthCheckAppService = healthCheckAppService;
  }

  [HttpPost("AddReport")]
  public async Task<HealthCheckReportDto> AddReportAsync(AddHealthCheckReportDto request)
  {

    try
    {
      return await _healthCheckAppService.AddReportAsync(request);
    }
    finally
    {
    }
  }

  [HttpPost("AddReportBulk")]
  public async Task<bool> AddReportBulkAsync(AddHealthCheckReportBulkDto request)
  {

    try
    {
      return await _healthCheckAppService.AddReportBulkAsync(request);
    }
    finally
    {
    }
  }

  [HttpPost("Create")]
  public async Task<HealthCheckDto> CreateAsync(CreateHealthCheckDto request)
  {

    try
    {
      return await _healthCheckAppService.CreateAsync(request);
    }
    finally
    {
    }
  }

  [HttpGet("GetById")]
  public async Task<FullHealthCheckDto> GetByIdAsync(Guid id)
  {

    try
    {
      return await _healthCheckAppService.GetByIdAsync(id);
    }
    finally
    {
    }
  }

  [HttpGet("GetList")]
  public async Task<PagedResultDto<HealthCheckDto>> GetListAsync(HealthCheckRequestDto request)
  {

    try
    {
      return await _healthCheckAppService.GetListAsync(request);
    }
    finally
    {
    }
  }

  [HttpPost("Send")]
  public async Task<HealthCheckDto> SendAsync(SendHealthCheckDto request)
  {

    try
    {
      return await _healthCheckAppService.SendAsync(request);
    }
    finally
    {
    }
  }
}
