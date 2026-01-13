using ModuleCollector.Google.Module.Google.Module.Application.Contracts.Route;

namespace ModuleCollector.Google.Module.Google.Module.Application.Contracts.OptimizeRoute;

public interface IRouteAppService
{
  Task<OptimizedToursDto> OptimizeRouteAsync(OptimizeToursRequestDto model);
}
