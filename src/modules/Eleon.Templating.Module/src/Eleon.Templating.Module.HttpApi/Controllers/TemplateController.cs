using Eleon.Templating.Module.Templates;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Eleon.Templating.Module.Controllers;

[Area(ModuleRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/templating/templates")]
public class TemplateController : ModuleController, ITemplateAppService
{
  private readonly ITemplateAppService _templateAppService;

  public TemplateController(ITemplateAppService templateAppService)
  {
    _templateAppService = templateAppService;
  }

  [HttpGet("GetById")]
  public async Task<TemplateDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.GetAsync(id, cancellationToken);
  }

  [HttpGet("GetList")]
  public async Task<PagedResultDto<MinimalTemplateDto>> GetListAsync([FromQuery] GetTemplateListInput input, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.GetListAsync(input, cancellationToken);
  }

  [HttpPost("Create")]
  public async Task<TemplateDto> CreateAsync([FromBody] CreateUpdateTemplateDto input, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.CreateAsync(input, cancellationToken);
  }

  [HttpPut("Update")]
  public async Task<TemplateDto> UpdateAsync(Guid id, [FromBody] CreateUpdateTemplateDto input, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.UpdateAsync(id, input, cancellationToken);
  }

  [HttpDelete("Delete")]
  public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
  {
    await _templateAppService.DeleteAsync(id, cancellationToken);
  }

  [HttpPost("Apply")]
  public async Task<string> ApplyTemplateAsync([FromBody] ApplyTemplateInput input, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.ApplyTemplateAsync(input, cancellationToken);
  }

  [HttpGet("GetByNameAndType")]
  public async Task<TemplateDto> GetByNameAndType(string name, TemplateType type, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.GetByNameAndType(name, type, cancellationToken);
  }

  [HttpPost("Reset")]
  public async Task<TemplateDto> ResetAsync(ResetTemplateInput input, CancellationToken cancellationToken = default)
  {
    return await _templateAppService.ResetAsync(input, cancellationToken);
  }
}

