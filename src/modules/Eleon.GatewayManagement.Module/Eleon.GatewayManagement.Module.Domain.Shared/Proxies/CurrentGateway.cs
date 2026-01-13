using System.Threading;
using Volo.Abp.DependencyInjection;
using VPortal.GatewayManagement.Module.Domain.Shared.GatewayCallInterception;

namespace GatewayManagement.Module.Proxies
{
  public class CurrentGateway : IScopedDependency
  {
    public CurrentGateway()
    {
    }

    static AsyncLocal<GatewayAppServiceCallOptions> _asyncLocalOptions = new();
    private GatewayAppServiceCallOptions _options;
    public GatewayAppServiceCallOptions Options
    {
      get => _options ?? _asyncLocalOptions.Value;
      set
      {
        _asyncLocalOptions.Value = value;
        _options = value;
      }
    }
  }
}
