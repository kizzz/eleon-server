using Common.EventBus.Module;
using EleonsoftAbp.Auth;
using EleonsoftAbp.Messages.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftAbp.EleonsoftPermissions;
public class ApiKeyAuthorizationHandler : IAuthorizationHandler
{
  private readonly IDistributedEventBus _eventBus;
  public ApiKeyAuthorizationHandler(IDistributedEventBus eventBus)
  {
    _eventBus = eventBus;
  }

  public async Task HandleAsync(AuthorizationHandlerContext context)
  {
    var apiKeyId = context.User?.GetApiKeyId();
    if (string.IsNullOrEmpty(apiKeyId))
    {
      return; // not an API key request, continue with other handlers or requirements
    }

    // todo validate key expiration, etc.

    if (context.Requirements.Where(x => x.GetType() == typeof(EleonsoftSdkPermissionRequirement)).Any())
    {
      return;
    }

    var response = await _eventBus.RequestAsync<ApiKeyResponseMsg>(new ApiKeyRequestMsg { KeyId = apiKeyId });

    if (!response.Found || !response.AllowAuthorize)
    {
      context.Fail();
    }
  }
}
