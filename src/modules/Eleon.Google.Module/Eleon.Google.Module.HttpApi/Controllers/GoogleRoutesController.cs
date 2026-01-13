using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Google.Module.Geocoding;
using VPortal.Google.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.Google.Module.Google.Module.Application.Contracts.OptimizeRoute;
using ModuleCollector.Google.Module.Google.Module.Application.Contracts.Route;
using Logging.Module;

namespace ModuleCollector.Google.Module.Google.Module.HttpApi.Controllers;

[Area(GoogleRemoteServiceConsts.ModuleName)]
[RemoteService(Name = GoogleRemoteServiceConsts.RemoteServiceName)]
[Route("api/Google/Geocoding")]
public class GoogleRoutesController : GoogleController, IRouteAppService
{
  private readonly IVportalLogger<GoogleRoutesController> logger;
  private readonly IRouteAppService routeAppService;

  public GoogleRoutesController(
      IVportalLogger<GoogleRoutesController> logger,
      IRouteAppService routeAppService)
  {
    this.logger = logger;
    this.routeAppService = routeAppService;
  }

  [HttpPost("OptimizeRoute")]
  public async Task<OptimizedToursDto> OptimizeRouteAsync(OptimizeToursRequestDto model)
  {

    var response = await routeAppService.OptimizeRouteAsync(model);


    return response;
  }
}
