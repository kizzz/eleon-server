using Castle.DynamicProxy;
using GatewayManagement.Module.Proxies;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.Domain.Shared.GatewayCallInterception
{
  public class GatewayAppServiceCallInterceptor : IInterceptor
  {
    private static readonly MethodInfo handleAsyncMethodInfo = typeof(GatewayAppServiceCallInterceptor).GetMethod("HandleAsyncWithResult", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly CurrentGateway currentGateway;
    private readonly GatewayAppServiceCallOptions options;

    public GatewayAppServiceCallInterceptor(CurrentGateway currentGateway, GatewayAppServiceCallOptions options)
    {
      this.currentGateway = currentGateway;
      this.options = options;
    }

    public void Intercept(IInvocation invocation)
    {
      currentGateway.Options = options;

      invocation.Proceed();

      var delegateType = GetDelegateType(invocation);
      if (delegateType == MethodType.AsyncAction)
      {
        invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, options.GatewayId);
      }
      else if (delegateType == MethodType.AsyncFunction)
      {
        ExecuteHandleAsyncWithResultUsingReflection(invocation, options.GatewayId);
      }
    }

    private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation, Guid gatewayId)
    {
      var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
      var mi = handleAsyncMethodInfo.MakeGenericMethod(resultType);
      invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue, gatewayId });
    }

    private async Task HandleAsync(Task task, Guid gatewayId)
    {
      try
      {
        await task;
      }
      catch (Exception ex)
      {
        throw new GatewayRequestException(gatewayId, ex);
      }
    }

    private async Task<T> HandleAsyncWithResult<T>(Task<T> task, Guid gatewayId)
    {
      try
      {
        return await task;
      }
      catch (Exception ex)
      {
        throw new GatewayRequestException(gatewayId, ex);
      }
    }

    private MethodType GetDelegateType(IInvocation invocation)
    {
      var returnType = invocation.Method.ReturnType;
      if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
      {
        return MethodType.AsyncFunction;
      }

      return MethodType.AsyncAction;
    }

    private enum MethodType
    {
      AsyncAction,
      AsyncFunction,
    }
  }
}
