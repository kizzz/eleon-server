using Microsoft.Extensions.DependencyInjection;

namespace Common.EventBus.Module.Interception
{
  public static class IServiceCollectionExtensions
  {
    public static IServiceCollection AddEventSendInterceptor<T>(this IServiceCollection services) where T : IEventSendInterceptor
    {
      return services.AddTransient(typeof(IEventSendInterceptor), typeof(T));
    }

    public static IServiceCollection AddEventConsumeInterceptor<T>(this IServiceCollection services) where T : IEventConsumeInterceptor
    {
      return services.AddTransient(typeof(IEventConsumeInterceptor), typeof(T));
    }
  }
}
