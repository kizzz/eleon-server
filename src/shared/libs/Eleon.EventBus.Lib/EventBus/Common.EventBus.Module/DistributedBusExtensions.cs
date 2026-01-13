using Volo.Abp.EventBus.Distributed;

namespace Common.EventBus.Module
{
  public static class DistributedBusExtensions
  {
    public static async Task<TResponse> RequestAsync<TResponse>(this IDistributedEventBus bus, object request, int timeoutSeconds = 180)
        where TResponse : class
    {
      if (bus is IResponseCapableEventBus responseBus)
      {
        return await responseBus.RequestAsync<TResponse>(request, timeoutSeconds);
      }
      else
      {
        throw new NotImplementedException("Reply mechanism is not implemented for this bus.");
      }
    }
  }
}
