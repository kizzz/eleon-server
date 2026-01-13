using EventBusManagement.Module;
using EventBusManagement.Module.EntityFrameworkCore;
using Logging.Module;

namespace VPortal.GatewayManagement.Module.EventBuses
{
  public class EventBusAppService : GatewayManagementBaseAppService, IEventBusAppService
  {
    private readonly IVportalLogger<EventBusAppService> logger;
    private readonly EventBusManager eventBusManager;

    public EventBusAppService(
        IVportalLogger<EventBusAppService> logger,
        EventBusManager eventBusManager)
    {
      this.logger = logger;
      this.eventBusManager = eventBusManager;
    }

    public async Task AddEventBus(EventBusDto eventBus)
    {
      try
      {
        var eventBusEntity = new EventBusEntity(eventBus.Id, eventBus.Provider, eventBus.ProviderOptions, eventBus.Status);
        await eventBusManager.AddEventBus(eventBusEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<EventBusDto>> GetEventBuses()
    {
      List<EventBusDto> result = null;
      try
      {
        var entities = await eventBusManager.GetEventBusList();
        result = ObjectMapper.Map<List<EventBusEntity>, List<EventBusDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<EventBusOptionsTemplateDto>> GetEventBusOptionsTemplates()
    {
      List<EventBusOptionsTemplateDto> result = null;
      try
      {
        var templates = await eventBusManager.GetSettingTemplates();
        result = templates.Select(x => new EventBusOptionsTemplateDto()
        {
          Provider = x.Key,
          Template = x.Value,
        }).ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
