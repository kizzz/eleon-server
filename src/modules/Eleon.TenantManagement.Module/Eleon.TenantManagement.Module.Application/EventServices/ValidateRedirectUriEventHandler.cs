using Common.EventBus.Module;
using Commons.Module.Messages.RedirectUri;
using Logging.Module;
using Logging.Module.ErrorHandling.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.EventServices;


public class ValidateRedirectUriEventHandler : IDistributedEventHandler<ValidateRedirectUriRequestMsg>, ITransientDependency
{
  private readonly IVportalLogger<ValidateRedirectUriEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly ICurrentTenant _currentTenant;

  public ValidateRedirectUriEventHandler(
      IVportalLogger<ValidateRedirectUriEventHandler> logger,
      IResponseContext responseContext,
      //IClientApplicationAppService clientApplicationAppService,
      ICurrentTenant currentTenant
      )
  {
    _logger = logger;
    _responseContext = responseContext;
    _currentTenant = currentTenant;
  }
  public async Task HandleEventAsync(ValidateRedirectUriRequestMsg eventData)
  {
    var response = new ValidateRedirectUriResponseMsg
    {
      IsValid = false,
    };
    try
    {
      response.IsValid = await ValidateUriAsync(eventData.RedirectUri, "/signin-oidc", eventData.IsSignOut);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }


  private async Task<bool> ValidateUriAsync(string requestedUri, string validPath, bool isSignOut)
  {
    Uri redirectUri = ParseUri(requestedUri);

    if (redirectUri.LocalPath?.StartsWith(validPath) == true)
    {
      return true;
    }
    //if (currentApp == null || !HasValidPath(redirectUri, currentApp, validPath))
    //{
    //    return false;
    //}

    return true;
  }

  private Uri ParseUri(string redirectUri)
  {
    try
    {
      return new Uri(redirectUri);
    }
    catch (Exception ex)
    {

      throw new InvalidOperationException($"Redirect uri '{redirectUri}' is not valid.", ex)
          .WithMessageAsFriendly();
    }
  }
}
