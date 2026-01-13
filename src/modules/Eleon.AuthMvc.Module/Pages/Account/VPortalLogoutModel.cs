using Common.Module.Constants;
using ExternalLogin.Module;
using Identity.Module.Application.Contracts.IdentityServerServices;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Logging.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace VPortal.Pages.Account
{
  [ExposeServices(typeof(LogoutModel))]
  public class VPortalLogoutModel : IdentityServerSupportedLogoutModel
  {
    private readonly OpenIdConnectStateResolver oidcStateResolver;
    private readonly IEnumerable<ILogoutHandler> logoutHandlers;
    private readonly IVportalLogger<VPortalLogoutModel> logger;
    private readonly IUnitOfWorkManager unitOfWorkManager;

    public VPortalLogoutModel(
        IIdentityServerInteractionService interaction,
        OpenIdConnectStateResolver oidcStateResolver,
        IEnumerable<ILogoutHandler> logoutHandlers,
        IVportalLogger<VPortalLogoutModel> logger,
        IUnitOfWorkManager unitOfWorkManager)
        : base(interaction)
    {
      this.oidcStateResolver = oidcStateResolver;
      this.logoutHandlers = logoutHandlers;
      this.logger = logger;
      this.unitOfWorkManager = unitOfWorkManager;
    }

    public async override Task<IActionResult> OnGetAsync()
    {
      string logoutId = null;
      LogoutRequest logoutRequest = null;
      try
      {

        string idp = GetIdp();
        logoutId = GetLogoutId();

        foreach (var logoutHandler in logoutHandlers)
        {
          try
          {
            await logoutHandler.ExecuteAsync();
          }
          catch (Exception ex)
          {
            logger.Log.LogError(ex, "An exception has occured while processing logout event");
          }
        }

        await SignInManager.SignOutAsync();

        if (!string.IsNullOrEmpty(logoutId))
        {
          logoutRequest = await Interaction.GetLogoutContextAsync(logoutId);
          return await RemoteSignOut(logoutRequest, idp) ?? await FinishSignOut(logoutRequest);
        }

        return await HandleMissingLogoutId();
      }
      catch (Exception ex)
      {
        logger.Log.LogError(ex, "An error has occured while processing logout action");
        if (string.IsNullOrEmpty(logoutId))
        {
          return await HandleMissingLogoutId();
        }
        logoutRequest ??= await Interaction.GetLogoutContextAsync(logoutId);
        return await FinishSignOut(logoutRequest);
      }
      finally
      {
        try
        {
          await unitOfWorkManager.Current?.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          await unitOfWorkManager.Current?.RollbackAsync();
          logger.Log.LogError(ex, "An error has occured while saving to database");
        }
      }
    }

    private async Task<IActionResult> HandleMissingLogoutId()
    {
      await SaveSecurityLogAsync();

      if (ReturnUrl != null)
      {
        return LocalRedirect(ReturnUrl ?? "/");
      }

      Logger.LogDebug(
          $"IdentityServerSupportedLogoutModel couldn't find postLogoutUri... Redirecting to:/Account/Login..");
      return RedirectToPage("/Account/Login");
    }

    private async Task<IActionResult> FinishSignOut(LogoutRequest logoutContext)
    {
      oidcStateResolver.ClearOidcStateCookie(HttpContext);

      await SaveSecurityLogAsync(logoutContext?.ClientId);

      await SignInManager.SignOutAsync();

      HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

      Logger.LogDebug($"Redirecting to Post Logout URL...");

      return Redirect(logoutContext.PostLogoutRedirectUri ?? "/");
    }

    private async Task<IActionResult> RemoteSignOut(LogoutRequest logoutContext, string idp)
    {
      if (
          idp.IsNullOrWhiteSpace()
          || !Enum.TryParse<ExternalLoginProviderType>(idp, out var externalProviderType)
          || externalProviderType == ExternalLoginProviderType.None
          || externalProviderType == ExternalLoginProviderType.Local)
      {
        return await FinishSignOut(logoutContext);
      }

      bool oidcCompleted = oidcStateResolver.IsStatePresent(HttpContext);
      if (oidcCompleted)
      {
        return await FinishSignOut(logoutContext);
      }

      string signOutScheme = GetSignOutScheme(externalProviderType);
      await SaveRemoteSignOutLog(logoutContext, signOutScheme);

      var logoutId = Request.Query["logoutId"].ToString();
      string url = $"https://{Request.Host}/auth/Account/Logout?logoutId={logoutId}";
      return SignOut(new AuthenticationProperties { RedirectUri = url }, signOutScheme);
    }

    private async Task SaveRemoteSignOutLog(LogoutRequest logoutContext, string authScheme)
    {
      await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = authScheme,
        Action = ExternalLoginSecurityLogActions.StartRemoteLogout,
        ClientId = logoutContext.ClientId,
      });
    }

    private string GetLogoutId() => Request.Query["logoutId"].ToString();

    private string GetIdp()
        => HttpContext.User.Claims.FirstOrDefault(x => x.Type == "idp")?.Value;

    private string GetSignOutScheme(ExternalLoginProviderType provider)
    {
      if (provider == ExternalLoginProviderType.Local)
      {
        return IdentityConstants.ApplicationScheme;
      }

      return provider.ToString();
    }
  }
}
