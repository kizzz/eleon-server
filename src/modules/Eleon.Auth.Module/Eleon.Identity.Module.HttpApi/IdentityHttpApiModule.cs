using IdentityServer4.Configuration;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Timeago;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using VPortal.Identity.Module.Localization;
using Common.Module.Constants;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.IdentityServer;
using VPortal.Content;
using VPortal.Options;
using VPortal.Identity.Module.IdentityServerExtensions;
using Volo.Abp.Reflection;
using Volo.Abp.UI.Navigation.Urls;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Volo.Abp;
using Microsoft.AspNetCore.Routing;
using VPortal.Identity.Module.Sessions;
using Microsoft.AspNetCore.Identity;

namespace VPortal.Identity.Module;

[DependsOn(
    typeof(IdentityApplicationContractsModule),
    //typeof(AbpAccountWebIdentityServerModule),
    //typeof(AbpAccountWebModule),
    //typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpIdentityAspNetCoreModule),
    //typeof(AbpAccountHttpApiModule),
    //typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpIdentityDomainModule)
    //,typeof(AbpAspNetCoreMvcModule)
    )]
public class IdentityHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();

    //context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
    //{
    //    options.AddAssemblyResource(typeof(VPortal.Identity.Module.Localization.IdentityResource), typeof(IdentityHttpApiModule).Assembly);
    //});

    //PreConfigure<IdentityUiOptions>(opt =>
    //{
    //    opt.PreConfigure(configuration);
    //});

    PreConfigureIdentityServer(context);

    //PreConfigure<IMvcBuilder>(mvcBuilder =>
    //{
    //    mvcBuilder.AddApplicationPartIfNotExists(typeof(IdentityHttpApiModule).Assembly);
    //});
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      //options.Resources
      //    .Get<IdentityResource>()
      //    .AddBaseTypes(typeof(AbpUiResource));
    });

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<IdentityHttpApiModule>();
    });
    ConfigureAccounts(context);
    ConfigureBundle();

    var configuration = context.Services.GetConfiguration();

    ConfigureUrl(configuration);
    //context.Services.AddRazorPages();
  }

  private void ConfigureUrl(IConfiguration configuration)
  {
  }
  private void PreConfigureIdentityServer(ServiceConfigurationContext context)
  {
    PreConfigure<IdentityBuilder>(identityBuilder =>
    {
      identityBuilder.AddSignInManager<SignIn.SignInManager>();
    });

    PreConfigure<IIdentityServerBuilder>(builder =>
    {
      builder.AddMachineKeyGrant();
      builder.AddApiKeyGrant();
      builder.AddCustomTokenRequestValidator();
      builder.AddImpersonationGrant();
      //builder.AddCustomClaimsProfileService();
      builder.AddAuthorizeInteractionResponseGenerator<CustomAuthorizeResponseGenerator>();
    });

    Configure<AbpClaimsServiceOptions>(options =>
    {
      options.RequestedClaims.AddRange(ReflectionHelper.GetPublicConstantsRecursively(typeof(OptionalUserClaims)));
      options.RequestedClaims.Add(VPortalExtensionGrantsConsts.MachineKey.MachineKeyClaim);
      options.RequestedClaims.Add(VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim);
      options.RequestedClaims.Add(VPortalExtensionGrantsConsts.Impresonation.ImpersonatorUserClaim);
    });

    //context.Services.PostConfigure<IdentityServerOptions>(option =>
    //{
    //    option.Events.RaiseSuccessEvents = true;
    //});
  }
  private void ConfigureAccounts(ServiceConfigurationContext context)
  {
    context.Services.Configure<AbpAccountOptions>(options =>
    {
      //options.TenantAdminUserName = "admin";
      //options.ImpersonationTenantPermission = SaasHostPermissions.Tenants.Impersonation;
      //options.ImpersonationUserPermission = IdentityPermissions.Users.Impersonation;
    });
  }
  private void ConfigureBundle()
  {

    //Configure<AbpBundlingOptions>(options =>
    //{
    //    options
    //        .StyleBundles
    //        .Get(StandardBundles.Styles.Global)
    //        .AddContributors(new ContentBundleContributor());

    //    options.StyleBundles.Configure(
    //        BasicThemeBundles.Styles.Global,
    //        bundle =>
    //        {
    //            bundle.AddFiles("/global-styles.css");
    //        }
    //    );
    //    //options
    //    //    .StyleBundles
    //    //    .Add(BasicThemeBundles.Styles.Global, bundle =>
    //    //    {
    //    //        bundle
    //    //            .AddBaseBundles(BasicThemeBundles.Styles.Global)
    //    //            .AddContributors(typeof(BasicThemeGlobalStyleContributor));
    //    //    });

    //    //options
    //    //    .ScriptBundles
    //    //    .Add(BasicThemeBundles.Scripts.Global, bundle =>
    //    //    {
    //    //        bundle
    //    //            .AddBaseBundles(BasicThemeBundles.Scripts.Global)
    //    //            .AddContributors(typeof(BasicThemeGlobalScriptContributor));
    //    //    });
    //});
  }

  public override void PostConfigureServices(ServiceConfigurationContext context)
  {
    // Uncomment it if you want to all pages was on route /auth/Account/Login
    //PostConfigure<RazorPagesOptions>(options =>
    //{
    //    options.Conventions.AddFolderRouteModelConvention("/Account", model =>
    //    {
    //        foreach (var s in model.Selectors)
    //        {
    //            s.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
    //                new AttributeRouteModel()
    //                {
    //                    Template = "/auth",
    //                },
    //                s.AttributeRouteModel);
    //        }
    //    });
    //});

    PostConfigure<IdentityServerOptions>(options =>
    {
      options.UserInteraction.LoginUrl = "/Account/Login";
      options.UserInteraction.LogoutUrl = "/Account/Logout";
      options.UserInteraction.LoginReturnUrlParameter = "ReturnUrl";
    });

    //PostConfigure<StaticFileOptions>(options =>
    //{
    //    options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

    //    // Use the WebRootFileProvider to serve files from the real folder

    //    options.FileProvider = options.FileProvider ?? context.Services.GetHostingEnvironment().WebRootFileProvider;

    //    var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "wwwroot");
    //    options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
    //});
  }
}
