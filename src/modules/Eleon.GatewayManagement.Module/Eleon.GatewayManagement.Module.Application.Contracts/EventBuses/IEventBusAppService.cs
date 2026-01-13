using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.GatewayManagement.Module.EventBuses
{
  public interface IEventBusAppService : IApplicationService
  {
    Task<List<EventBusDto>> GetEventBuses();
    Task<List<EventBusOptionsTemplateDto>> GetEventBusOptionsTemplates();
    Task AddEventBus(EventBusDto eventBus);
  }
}
