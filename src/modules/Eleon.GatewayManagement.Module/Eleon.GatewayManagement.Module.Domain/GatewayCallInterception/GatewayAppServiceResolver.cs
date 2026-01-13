using Castle.DynamicProxy;
using GatewayManagement.Module.Proxies;
using Volo.Abp.DependencyInjection;

namespace VPortal.GatewayManagement.Module.Domain.Shared.GatewayCallInterception
{
  public class GatewayAppServiceResolver : ITransientDependency
  {
    private static readonly ProxyGenerator CastleGatewayGenerator = new ProxyGenerator();
    private readonly CurrentGateway currentGateway;
    private readonly IAbpLazyServiceProvider serviceProvider;

    public GatewayAppServiceResolver(
        CurrentGateway currentGateway,
        IAbpLazyServiceProvider serviceProvider)
    {
      this.currentGateway = currentGateway;
      this.serviceProvider = serviceProvider;
    }

    public TAppService ResolveScopedGatewayAppService<TAppService>(GatewayAppServiceCallOptions options)
        where TAppService : class
    {
      var appService = serviceProvider.LazyGetRequiredService<TAppService>();
      return GenerateScopedGatewayAppService(appService, options);
    }

    private TAppService GenerateScopedGatewayAppService<TAppService>(TAppService appService, GatewayAppServiceCallOptions options)
        where TAppService : class
    {
      var interceptor = new GatewayAppServiceCallInterceptor(currentGateway, options);
      return CastleGatewayGenerator.CreateInterfaceProxyWithTarget(appService, interceptor);
    }
  }
}
