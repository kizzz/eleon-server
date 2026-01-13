using Logging.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayForwardedRequestManager : ISingletonDependency
  {
    private readonly GatewayForwardedRequestCollection<string, string> forwardedRequests = new();
    private readonly IVportalLogger<GatewayForwardedRequestManager> logger;
    private readonly IGuidGenerator guidGenerator;

    public GatewayForwardedRequestManager(
        IVportalLogger<GatewayForwardedRequestManager> logger,
        IGuidGenerator guidGenerator)
    {
      this.logger = logger;
      this.guidGenerator = guidGenerator;
    }

    public Guid AddForwardedRequest(string request)
    {
      Guid requestId = default;
      try
      {
        requestId = guidGenerator.Create();
        var forwardedRequest = new GatewayForwardedRequest<string, string>(request);
        forwardedRequests.AddRequest(requestId, forwardedRequest);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return requestId;
    }

    public async Task<string> WaitForResponse(Guid requestId, TimeSpan timeout)
    {
      string response = null;
      try
      {
        var forwardedRequest = forwardedRequests.GetRequest(requestId);

        var responseTask = forwardedRequest.ResponseTask;
        var timeoutTask = Task.Delay(timeout);

        var completed = await Task.WhenAny(responseTask, timeoutTask);
        if (completed == timeoutTask)
        {
          throw new TimeoutException($"Gateway has not provided a response in {timeout}.");
        }

        response = await responseTask;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
        forwardedRequests.RemoveRequest(requestId);
      }

      return response;
    }

    public void SetForwardedRequestResponse(Guid requestId, string response)
    {
      try
      {
        var forwardedRequest = forwardedRequests.GetRequest(requestId);
        forwardedRequest.SetResponse(response);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public string GetForwardedRequest(Guid requestId)
    {
      string request = null;
      try
      {
        var forwardedRequest = forwardedRequests.GetRequest(requestId);
        request = forwardedRequest.Request;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return request;
    }

    class GatewayForwardedRequest<TRequest, TResponse>
    {
      public GatewayForwardedRequest(TRequest request)
      {
        Request = request;
      }

      private readonly TaskCompletionSource<TResponse> tcs = new();

      public TRequest Request { get; }
      public Task<TResponse> ResponseTask => tcs.Task;

      public void SetResponse(TResponse response)
      {
        tcs.SetResult(response);
      }
    }

    class GatewayForwardedRequestCollection<TRequest, TResponse>
    {
      private readonly ConcurrentDictionary<Guid, GatewayForwardedRequest<TRequest, TResponse>> requests = new();

      public void AddRequest(Guid requestId, GatewayForwardedRequest<TRequest, TResponse> request)
      {
        if (requests.ContainsKey(requestId))
        {
          throw new Exception("This request is already registered.");
        }

        requests[requestId] = request;
      }

      public GatewayForwardedRequest<TRequest, TResponse> GetRequest(Guid requestId)
      {
        if (requests.TryGetValue(requestId, out var request))
        {
          return request;
        }

        throw new Exception("A request with this ID is not registered.");
      }

      public void RemoveRequest(Guid requestId)
      {
        if (!requests.ContainsKey(requestId))
        {
          throw new Exception("A request with this ID is not registered.");
        }

        requests.Remove(requestId, out _);
      }
    }
  }
}
