using Common.Module.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Volo.Abp.Authorization;

namespace Authorization.Module.MachineKeyValidation
{
  public class MachineKeyValidationMiddleware
  {
    private readonly RequestDelegate next;
    private const string AuthorizationHeaderName = "Authorization";

    public MachineKeyValidationMiddleware(RequestDelegate next)
    {
      this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      bool valid = true;
      try
      {
        ValidateContextMachineKey(context);
      }
      catch (AbpAuthorizationException)
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Authorization error");
        valid = false;
      }

      if (valid)
      {
        await next.Invoke(context);
      }
    }

    private void ValidateContextMachineKey(HttpContext context)
    {
      bool authorizationSet = context.Request.Headers.ContainsKey(AuthorizationHeaderName);
      if (!authorizationSet)
      {
        return;
      }

      var identity = context.User.Identity as ClaimsIdentity;
      var machineKeyClaim = identity?.FindFirst("client_" + VPortalExtensionGrantsConsts.MachineKey.MachineKeyClaim);
      var options = context.RequestServices.GetRequiredService<IOptions<MachineKeyValidationOptions>>();
      bool machineKeyRequired = options.Value.RequireMachineKey;

      bool machineKeyHeaderPresent = context.Request.Headers.TryGetValue(VPortalExtensionGrantsConsts.MachineKey.MachineKeyHeader, out var encryptedCompoundKey);
      bool machineKeyClaimSet = machineKeyClaim != null;

      bool machineKeyRequiredButNotSet = (!machineKeyClaimSet || !machineKeyHeaderPresent) && machineKeyRequired;
      bool machineKeyNotFullySpecified = !machineKeyClaimSet && machineKeyHeaderPresent || machineKeyClaimSet && !machineKeyHeaderPresent;
      if (machineKeyRequiredButNotSet || machineKeyNotFullySpecified)
      {
        throw new AbpAuthorizationException("It is required to send both a machine key header and machine key claim for authorized requests.");
      }

      bool machineKeyOptionalAndAbsent = !machineKeyClaimSet && !machineKeyHeaderPresent && !machineKeyRequired;
      if (machineKeyOptionalAndAbsent)
      {
        return;
      }

      string etalonMachineKey = machineKeyClaim?.Value;
      if (etalonMachineKey == null)
      {
        throw new AbpAuthorizationException("Claims do not contain the machine key to compare. Make sure you are using the correct grant type to retreive token.");
      }

      var tokenValues = context.Request.Headers[AuthorizationHeaderName];
      string token = tokenValues.First();
      bool valid = MachineKeyValidator.ValidateMachineKey(encryptedCompoundKey, etalonMachineKey, token);
      if (!valid)
      {
        throw new AbpAuthorizationException("Provided machine key is invalid.");
      }
    }
  }
}
