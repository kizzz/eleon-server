using abp_sdk.Middlewares;
using Eleoncore.SDK.TenantSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.MultiTenancy.Module;
using System.Globalization;
using TenantSettings.Module.Cache;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

namespace VPortal.TenantManagement.Module.Middlewares
{
  public class EleoncoreMutliTenancyMiddleware : IMiddleware, ITransientDependency
  {
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly ICurrentTenant _currentTenant;
    private readonly IOptions<AbpAspNetCoreMultiTenancyOptions> multitenancyOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<EleoncoreMutliTenancyMiddleware> logger;
    private readonly EleoncoreSdkTenantSettingService eleoncoreTenantResolver;
    private readonly ITenantResolveResultAccessor _tenantResolveResultAccessor;
    private readonly EleonMultiTenancyOptions _eleonsoftOptions;

    public EleoncoreMutliTenancyMiddleware(
        ITenantConfigurationProvider tenantConfigurationProvider,
        ICurrentTenant currentTenant,
        IOptions<AbpAspNetCoreMultiTenancyOptions> multitenancyOptions,
        IServiceProvider serviceProvider,
        ILogger<EleoncoreMutliTenancyMiddleware> logger,
        EleoncoreSdkTenantSettingService eleoncoreTenantResolver,
        ITenantResolveResultAccessor tenantResolveResultAccessor,
        IOptions<EleonMultiTenancyOptions> eleonsoftOptions)
    {


      _tenantConfigurationProvider = tenantConfigurationProvider;
      _currentTenant = currentTenant;
      this.multitenancyOptions = multitenancyOptions;
      this.serviceProvider = serviceProvider;
      this.logger = logger;
      this.eleoncoreTenantResolver = eleoncoreTenantResolver;
      _tenantResolveResultAccessor = tenantResolveResultAccessor;
      _eleonsoftOptions = eleonsoftOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      ArgumentNullException.ThrowIfNull(context, nameof(context));

      Guid? tenantId = null;
      try
      {
        // Here goes the origin resolvment logic
        (var isSuccess, tenantId) = await ResolveAsync(context);
        if (isSuccess)
        {
          context.RequestServices.GetService<EleoncoreTenantResolveResultAccessor>()?.SetResult(tenantId, null);
        }
        else
        {
          context.RequestServices.GetService<EleoncoreTenantResolveResultAccessor>()?.SetResult(tenantId, new Exception("Tenant was not resolved."));
        }
      }
      catch (Exception e)
      {
        logger.LogWarning(e, "Tenant was not resolved: {reason}", e.Message);

        tenantId = null;
        context.RequestServices.GetService<EleoncoreTenantResolveResultAccessor>()?.SetResult(tenantId, e);
        if (!_eleonsoftOptions.SuppressUnresolvedTenant)
        {
          throw;
        }
      }

      _tenantResolveResultAccessor.Result = new TenantResolveResult() { TenantIdOrName = tenantId?.ToString() };

      if (tenantId != _currentTenant.Id)
      {
        using (_currentTenant.Change(tenantId))
        {
          if (_tenantResolveResultAccessor.Result != null &&
              _tenantResolveResultAccessor.Result.AppliedResolvers.Contains(
                  QueryStringTenantResolveContributor.ContributorName
                  ))
          {
            AbpMultiTenancyCookieHelper.SetTenantCookie(context, _currentTenant.Id, multitenancyOptions.Value.TenantKey);
          }

          var requestCulture = await TryGetRequestCultureAsync(context);
          if (requestCulture != null)
          {
            CultureInfo.CurrentCulture = requestCulture.Culture;
            CultureInfo.CurrentUICulture = requestCulture.UICulture;
            AbpRequestCultureCookieHelper.SetCultureCookie(
                context,
                requestCulture
            );
            context.Items[AbpRequestLocalizationMiddleware.HttpContextItemName] = true;
          }

          await next(context);
        }
      }
      else
      {
        await next(context);
      }
    }

    private async Task<RequestCulture?> TryGetRequestCultureAsync(HttpContext httpContext)
    {
      var requestCultureFeature = httpContext.Features.Get<IRequestCultureFeature>();

      /* If requestCultureFeature == null, that means the RequestLocalizationMiddleware was not used
       * and we don't want to set the culture. */
      if (requestCultureFeature == null)
      {
        return null;
      }

      /* If requestCultureFeature.Provider is not null, that means RequestLocalizationMiddleware
       * already picked a language, so we don't need to set the default. */
      if (requestCultureFeature.Provider != null)
      {
        return null;
      }

      var settingProvider = httpContext.RequestServices.GetRequiredService<ISettingProvider>();
      var defaultLanguage = await settingProvider.GetOrNullAsync(LocalizationSettingNames.DefaultLanguage);
      if (defaultLanguage.IsNullOrWhiteSpace())
      {
        return null;
      }

      string culture;
      string uiCulture;

      if (defaultLanguage!.Contains(';'))
      {
        var splitted = defaultLanguage.Split(';');
        culture = splitted[0];
        uiCulture = splitted[1];
      }
      else
      {
        culture = defaultLanguage;
        uiCulture = defaultLanguage;
      }

      if (CultureHelper.IsValidCultureCode(culture) &&
          CultureHelper.IsValidCultureCode(uiCulture))
      {
        return new RequestCulture(culture, uiCulture);
      }

      return null;
    }

    private async Task<(bool handled, Guid? tenantId)> ResolveAsync(HttpContext httpContext)
    {
      return await eleoncoreTenantResolver.ResolveTenantFromHostname(httpContext);
    }
  }
}
