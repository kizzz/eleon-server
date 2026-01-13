using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Eleon.Templating.Module.Samples;

[Area(ModuleRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/module/example")]
public class ExampleController : ModuleController, ISampleAppService
{
  private readonly ISampleAppService _sampleAppService;

  public ExampleController(ISampleAppService sampleAppService)
  {
    _sampleAppService = sampleAppService;
  }

  [HttpGet]
  public async Task<SampleDto> GetAsync()
  {
    return await _sampleAppService.GetAsync();
  }

  [HttpGet]
  [Route("authorized")]
  [Authorize]
  public async Task<SampleDto> GetAuthorizedAsync()
  {
    return await _sampleAppService.GetAsync();
  }
}
