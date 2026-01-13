using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.DocMessageLog.Module;
using VPortal.Infrastructure.Module;
using VPortal.Infrastructure.Module.AuditLogs;

namespace VPortal.Infrastructure.Feature.Module.Controllers
{
  [Area(SystemLogModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SystemLogModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/auditLog/auditLogs")]
  public class AuditLogController : SystemLogModuleController, IAuditLogAppService
  {
    private readonly IAuditLogAppService appService;
    private readonly IVportalLogger<AuditLogController> _logger;

    public AuditLogController(
        IVportalLogger<AuditLogController> logger,
        IAuditLogAppService appService)
    {
      _logger = logger;
      this.appService = appService;
    }

    [HttpPost("AddAudit")]
    public async Task<bool> AddAuditAsync(AuditLogDto input)
    {

      try
      {
        return await appService.AddAuditAsync(input);
      }
      finally
      {
      }
    }

    [HttpGet("GetAuditLogById")]
    public async Task<AuditLogDto> GetAuditLogById(Guid id)
    {

      var response = await appService.GetAuditLogById(id);


      return response;
    }

    [HttpPost("GetAuditLogList")]
    public async Task<PagedResultDto<AuditLogHeaderDto>> GetAuditLogList(AuditLogListRequestDto input)
    {

      var response = await appService.GetAuditLogList(input);


      return response;
    }

    [HttpGet("GetEntityChangeById")]
    public async Task<EntityChangeDto> GetEntityChangeById(Guid id)
    {

      var response = await appService.GetEntityChangeById(id);


      return response;
    }

    [HttpPost("GetEntityChangeList")]
    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangeList(EntityChangeListRequestDto input)
    {

      var response = await appService.GetEntityChangeList(input);


      return response;
    }
  }
}
