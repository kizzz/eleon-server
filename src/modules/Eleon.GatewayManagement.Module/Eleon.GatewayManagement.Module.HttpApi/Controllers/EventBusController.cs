using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatewayManagement.Module.Proxies;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Proxies;
using VPortal.GatewayManagement.Module.EventBuses;
using System.Collections.Generic;

namespace GatewayManagement.Module.Controllers
{
  [Area(GatewayManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GatewayManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/GatewayManagement/EventBuses")]
  public class EventBusController : GatewayManagementBaseController, IEventBusAppService
  {
    private readonly IEventBusAppService appService;
    private readonly IVportalLogger<EventBusController> logger;

    public EventBusController(
        IEventBusAppService appService,
        IVportalLogger<EventBusController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("AddEventBus")]
    public async Task AddEventBus(EventBusDto eventBus)
    {

      await appService.AddEventBus(eventBus);

    }

    [HttpGet("GetEventBuses")]
    public async Task<List<EventBusDto>> GetEventBuses()
    {

      var buses = await appService.GetEventBuses();

      return buses;
    }

    [HttpGet("GetEventBusOptionsTemplates")]
    public async Task<List<EventBusOptionsTemplateDto>> GetEventBusOptionsTemplates()
    {

      var buses = await appService.GetEventBusOptionsTemplates();

      return buses;
    }
  }
}
