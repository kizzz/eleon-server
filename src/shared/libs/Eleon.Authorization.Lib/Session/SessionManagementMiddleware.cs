using Common.EventBus.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace Authorization.Module.Session
{
  public class SessionManagementMiddleware : IMiddleware, ITransientDependency
  {
    private readonly ILogger<SessionManagementMiddleware> logger;
    private readonly IDistributedEventBus setSessionStateClient;

    public SessionManagementMiddleware(
        ILogger<SessionManagementMiddleware> logger,
        IDistributedEventBus setSessionStateClient)
    {
      this.logger = logger;
      this.setSessionStateClient = setSessionStateClient;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var principal = context.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
      var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();

      //bool passwordChangeRequired = false;
      //if (currentUser.Id.HasValue)
      //{
      //    passwordChangeRequired = principal.Principal.HasClaim(c => c.Type == PeriodicPasswordChangeConsts.PasswordChangeRequiredClaimType);
      //}

      //if (passwordChangeRequired)
      //{
      //    var request = new SetUserSessionStateMsg(DocumentObjectType.User, currentUser.Id!.Value);
      //    request.RequirePeriodicPasswordChange = true;
      //    await setSessionStateClient.RequestAsync<UserSessionStateSetMsg>(request);
      //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
      //    return;
      //}

      await next(context);

      if (context.Response.StatusCode == StatusCodes.Status403Forbidden && currentUser.Id.HasValue)
      {
        logger.LogWarning("Response after login was Forbidden. Requested path: {path}", context.Request.Path);
        var request = new SetUserSessionStateMsg
        {
          UserId = currentUser.Id.Value,
          PermissionErrorEncountered = true
        };
        await setSessionStateClient.RequestAsync<UserSessionStateSetMsg>(request);
      }
    }
  }
}
