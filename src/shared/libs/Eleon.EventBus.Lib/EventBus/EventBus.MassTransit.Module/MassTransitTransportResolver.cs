using Volo.Abp.DependencyInjection;

namespace EventBus.MassTransit.Module
{
  public class MassTransitTransportResolver : ITransientDependency
  {
    private readonly IEnumerable<IMassTransitTransportResolveContributor> contributors;

    public MassTransitTransportResolver(
        IEnumerable<IMassTransitTransportResolveContributor> contributors)
    {
      this.contributors = contributors;
    }

    public IMassTransitTransport Resolve(MassTransitOptions options)
    {
      var resolveContext = new MassTransitTransportResolveContext();
      resolveContext.Options = options;

      foreach (var contrib in contributors)
      {
        contrib.Resolve(resolveContext);
        if (resolveContext.Resolved)
        {
          break;
        }
      }

      return resolveContext.Transport ?? new MassTransitInMemoryTransport();
    }
  }

  public interface IMassTransitTransportResolveContributor
  {
    void Resolve(MassTransitTransportResolveContext context);
  }

  public class MassTransitTransportResolveContext
  {
    public MassTransitTransportResolveContext()
    {
      Resolved = false;
    }

    public MassTransitOptions Options { get; set; } = null!;
    public bool Resolved { get; set; }
    public IMassTransitTransport Transport { get; set; } = null!;
  }
}
