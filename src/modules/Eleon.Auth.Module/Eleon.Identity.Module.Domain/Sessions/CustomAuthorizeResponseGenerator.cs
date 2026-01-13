using Common.EventBus.Module;
using Commons.Module.Messages.SessionState;
using IdentityServer4.Extensions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.Sessions
{
  public class CustomAuthorizeResponseGenerator : AuthorizeInteractionResponseGenerator
  {
    private readonly IDistributedEventBus sessionManager;
    private readonly PasswordChangeDomainService passwordChangeDomainService;

    public CustomAuthorizeResponseGenerator(
        IDistributedEventBus sessionManager,
        PasswordChangeDomainService passwordChangeDomainService,
        ISystemClock clock,
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consent,
        IProfileService profile)
        : base(clock, logger, consent, profile)
    {
      this.sessionManager = sessionManager;
      this.passwordChangeDomainService = passwordChangeDomainService;
    }

    protected override async Task<InteractionResponse> ProcessLoginAsync(ValidatedAuthorizeRequest request)
    {
      var authorizationResult = await base.ProcessLoginAsync(request);
      bool authorized = !authorizationResult.IsLogin && !authorizationResult.IsError;
      if (authorized && Guid.TryParse(request.Subject.GetSubjectId(), out Guid userId))
      {
        if (await passwordChangeDomainService.ShouldChangePassword(userId))
        {
          return new InteractionResponse
          {
            RedirectUrl = "/auth/Account/ChangePassword",
          };
        }

        var userState = await sessionManager.RequestAsync<UserSessionStateResponseMsg>(new GetUserSessionStateRequestMsg
        {
          UserId = userId,
        });
        if (userState.PermissionErrorEncountered)
        {
          // Note: logger is passed to base class, not available here directly
          await sessionManager.RequestAsync<UserSessionStateSetMsg>(new SetUserSessionStateMsg
          {
            UserId = userId,
            RequirePeriodicPasswordChange = userState.RequirePeriodicPasswordChange,
            PermissionErrorEncountered = false,
          });
          //return new InteractionResponse
          //{
          //    RedirectUrl = "/Account/PermissionError",
          //};
        }
      }

      return authorizationResult;
    }
  }
}
