using Common.EventBus.Module;
using Commons.Module.Messages.RedirectUri;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Logging.Module;
using Logging.Module.ErrorHandling.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace Identity.Module.Application.IdentityServerServices;

/// <summary>
/// Custom implementation of redirect URI validator. Validates the URIs against
/// the client's configured URIs.
/// </summary>
public class CustomRedirectUriValidator : IRedirectUriValidator
{
  private readonly ICurrentTenant _currentTenant;
  // private readonly IClientApplicationAppService _clientApplicationAppService;
  private readonly IVportalLogger<CustomRedirectUriValidator> _logger;
  private readonly IDistributedEventBus _eventBus;

  public CustomRedirectUriValidator(
      ICurrentTenant currentTenant,
      // IClientApplicationAppService clientApplicationAppService,
      IVportalLogger<CustomRedirectUriValidator> logger,
      IDistributedEventBus eventBus)
  {
    _currentTenant = currentTenant;
    // _clientApplicationAppService = clientApplicationAppService;
    _logger = logger;
    _eventBus = eventBus;
  }

  /// <summary>
  /// Determines whether a redirect URI is valid for a client.
  /// </summary>
  /// <param name="requestedUri">The requested URI.</param>
  /// <param name="client">The client.</param>
  /// <returns>
  ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
  /// </returns>
  public virtual async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
  {

    try
    {
      return await ValidateUriAsync(requestedUri, "/signin-oidc", false);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }

  /// <summary>
  /// Determines whether a post logout URI is valid for a client.
  /// </summary>
  /// <param name="requestedUri">The requested URI.</param>
  /// <param name="client">The client.</param>
  /// <returns>
  ///   <c>true</c> is the URI is valid; <c>false</c> otherwise.
  /// </returns>
  public virtual async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
  {

    try
    {
      return await ValidateUriAsync(requestedUri, "/signin-oidc", true);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }

  private async Task<bool> ValidateUriAsync(string requestedUri, string validPath, bool isSignOut)
  {
    Uri redirectUri = ParseUri(requestedUri);

    var response = await _eventBus.RequestAsync<ValidateRedirectUriResponseMsg>(new ValidateRedirectUriRequestMsg
    {
      RedirectUri = requestedUri,
      IsSignOut = isSignOut,
    });

    //var currentApp = await GetCurrentAppAsync(redirectUri, _currentTenant.Id, validPath);

    //if (currentApp == null || !HasValidPath(redirectUri, currentApp, validPath))
    //{
    //    return false;
    //}

    return response.IsValid;
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

  //private async Task<ClientApplicationDto?> GetCurrentAppAsync(Uri redirectUri, Guid? tenantId, string validPath)
  //{
  //    var apps = await _clientApplicationAppService.GetByTenantIdAsync(tenantId);

  //    var localPath = redirectUri.LocalPath.EndsWith(validPath) ? redirectUri.LocalPath.Substring(0, redirectUri.LocalPath.Length - validPath.Length) : redirectUri.LocalPath;

  //    return apps.FirstOrDefault(app => !string.IsNullOrEmpty(app.Path) && localPath == app.Path.EnsureStartsWith('/'));
  //}

  //private bool HasValidPath(Uri redirectUri, ClientApplicationDto currentApp, string path)
  //{
  //    if (redirectUri.LocalPath == currentApp.Path + path)
  //    {
  //        return true;
  //    }
  //    return false;
  //}

  private Dictionary<string, string> ParseQuery(Uri redirectUri)
  {
    string query = redirectUri.Query.TrimStart('?');
    var queryDictionary = HttpUtility.ParseQueryString(query);
    var result = new Dictionary<string, string>();

    foreach (string key in queryDictionary.AllKeys)
    {
      if (key != null)
      {
        result[key] = queryDictionary[key];
      }
    }
    return result;
  }
}
