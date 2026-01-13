using Common.EventBus.Module.Interception;
using Common.Module.Serialization;
using Eleon.Common.Lib.UserToken;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Common.EventBus.Module
{
  public class EventContextManager : ITransientDependency
  {
    private readonly ITransientCachedServiceProvider transientCachedServiceProvider;
    private readonly ICurrentTenant currentTenant;
    private readonly ICurrentPrincipalAccessor currentPrincipalAccessor;
    private readonly CurrentEvent currentEvent;
    private readonly DuplicateEventTracker duplicateEventTracker;
    private readonly IServiceProvider _serviceProvider;

    public EventContextManager(
        ITransientCachedServiceProvider transientCachedServiceProvider,
        ICurrentTenant currentTenant,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        CurrentEvent currentEvent,
        DuplicateEventTracker duplicateEventTracker,
        IServiceProvider serviceProvider)
    {
      this.transientCachedServiceProvider = transientCachedServiceProvider;
      this.currentTenant = currentTenant;
      this.currentPrincipalAccessor = currentPrincipalAccessor;
      this.currentEvent = currentEvent;
      this.duplicateEventTracker = duplicateEventTracker;
      _serviceProvider = serviceProvider;
    }

    public void UnwrapEventContext(string contextJson)
    {
      var context = JsonConvert.DeserializeObject<EventContext>(contextJson);
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      // dont fail if no token provider
      var userTokenProvider = _serviceProvider.GetService<IUserTokenProvider>();
      userTokenProvider?.Token = context.Token;

      currentTenant.Change(context.TenantId, context.TenantName);
      currentPrincipalAccessor.Change(context.Principal.ToClaimsPrincipal());
      
      currentEvent.EventId = context.EventId;
      currentEvent.EventSendTime = context.EventSendTime;
    }

    public string CreateEventContext()
    {
      var context = new EventContext()
      {
        TenantId = currentTenant.Id,
        TenantName = currentTenant.Name,
        Token = _serviceProvider.GetService<IUserTokenProvider>()?.Token,
        Principal = new ClaimsPrincipalData(currentPrincipalAccessor.Principal),
        EventId = currentEvent.EventId,
        EventSendTime = currentEvent.EventSendTime,
      };

      return JsonConvert.SerializeObject(context);
    }

    public async Task RegisterMessageConsume(Type eventType, object eventData)
    {
      var interceptors = transientCachedServiceProvider.GetServices<IEventConsumeInterceptor>();
      foreach (var interceptor in interceptors)
      {
        await interceptor.Intercept(eventType, eventData);
      }
    }

    public bool IsCurrentEventDuplicate()
        => !duplicateEventTracker.RegisterMessage(currentEvent.EventId, currentEvent.EventSendTime);

    private class EventContext
    {
      public Guid? TenantId { get; set; }
      public required string TenantName { get; set; }
      public required ClaimsPrincipalData Principal { get; set; }
      public string? Token { get; set; }
      public Guid EventId { get; set; }
      public DateTime EventSendTime { get; set; }
    }
  }
}
