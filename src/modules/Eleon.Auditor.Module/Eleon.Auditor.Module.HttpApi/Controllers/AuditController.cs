using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
using Volo.Abp;
using VPortal.Auditor.Module;
using VPortal.Auditor.Module.Application.Contracts.Audit;
using VPortal.Infrastructure.Module.Entities;

namespace ModuleCollector.Auditor.Module.Auditor.Module.HttpApi.Controllers;

[Area(AuditorRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AuditorRemoteServiceConsts.RemoteServiceName)]
[Route("api/Auditor/Audit")]
public class AuditController : ModuleController, IAuditAppService
{
  private readonly IVportalLogger<AuditController> logger;
  private readonly IAuditAppService auditAppService;

  public AuditController(IVportalLogger<AuditController> logger, IAuditAppService auditAppService)
  {
    this.logger = logger;
    this.auditAppService = auditAppService;
  }

  [HttpPost("Create")]
  public Task<bool> CreateAsync(CreateAuditDto input)
  {

    var response = auditAppService.CreateAsync(input);


    return response;
  }

  [HttpGet("Get")]
  public Task<AuditDto> GetAsync(GetAuditDto input)
  {

    var response = auditAppService.GetAsync(input);


    return response;
  }

  [HttpGet("GetCurrentVersion")]
  public Task<DocumentVersionEntity> GetCurrentVersionAsync(GetVersionDto input)
  {

    var response = auditAppService.GetCurrentVersionAsync(input);


    return response;
  }

  [HttpPost("IncrementVersion")]
  public Task<IncrementVersionResultDto> IncrementAuditVersionAsync(IncrementVersionRequestDto input)
  {

    var response = auditAppService.IncrementAuditVersionAsync(input);


    return response;
  }
}
