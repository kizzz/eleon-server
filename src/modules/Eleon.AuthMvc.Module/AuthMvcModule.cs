using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.IdentityServer.Jwt;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.VirtualFileSystem;
using VPortal;
using VPortal.Content;
using VPortal.ExternalLinkModule;
using VPortal.Identity.Module;
using VPortal.Options;
using VPortal.Otp.Module;

namespace Eleon.Auth.App;

[DependsOn(typeof(IdentityModuleCollector))]
public class AuthMvcModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();

    context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
    {
      options.AddAssemblyResource(typeof(VPortal.Identity.Module.Localization.IdentityResource), typeof(AuthMvcModule).Assembly);
    });

    PreConfigure<IdentityUiOptions>(opt =>
    {
      opt.PreConfigure(configuration);
    });

    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(AuthMvcModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();
    var hostingEnvironment = context.Services.GetHostingEnvironment();

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<AuthMvcModule>();
    });

    ConfigureBundle();

    context.Services
        .AddAuthentication(opt =>
        {
          opt.DefaultAuthenticateScheme = "jwt_or_cookie";
          opt.DefaultChallengeScheme = "jwt_or_cookie";
        })
        .AddPolicyScheme("jwt_or_cookie", "Authorization Bearer or OIDC", options =>
        {
          options.ForwardDefaultSelector = context =>
              {
              var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
              if (authHeader?.ToLower().StartsWith("bearer ") == true)
              {
                return JwtBearerDefaults.AuthenticationScheme;
              }

              return IdentityConstants.ApplicationScheme;
            };
        })
        .AddCookie(cfg => cfg.SlidingExpiration = true);

    context.Services.AddExternalLogin(configuration);
  }

  public override void PostConfigureServices(ServiceConfigurationContext context)
  {
    PostConfigure<StaticFileOptions>(options =>
    {
      options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

      // Use the WebRootFileProvider to serve files from the real folder
      options.FileProvider = options.FileProvider ?? context.Services.GetHostingEnvironment().WebRootFileProvider;

      // Add embedded files from Auth assembly (includes all module files compiled via <Compile Include>)
      var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly);
      options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
    });

    PostConfigure<AbpMvcLibsOptions>(options =>
    {
      options.CheckLibs = false;
    });
  }

  public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
  {
    IApplicationBuilder? app = null;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }
    app.UseStaticFiles("/files");

    app.UseStaticFiles(new StaticFileOptions
    {
      FileProvider = new EmbeddedFileProvider(
            typeof(AuthMvcModule).Assembly,
            "Eleon.AuthMvc.Module.Resources"
        ),
      RequestPath = "/resources"
    });

    app.UseJwtTokenMiddleware(IdentityConstants.ApplicationScheme);
  }

  public override void OnApplicationInitialization(ApplicationInitializationContext context)
  {
    var env = context.ServiceProvider.GetService<IWebHostEnvironment>();
    if (env == null)
    {
      return;
    }

    IApplicationBuilder? app = null;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }

    var cfg = context.GetConfiguration();

    app.UseIdentityServer();
  }

  private void ConfigureBundle()
  {

    Configure<AbpBundlingOptions>(options =>
    {
      options
              .StyleBundles
              .Get(StandardBundles.Styles.Global)
              .AddContributors(new ContentBundleContributor());

      options.StyleBundles.Configure(
              BasicThemeBundles.Styles.Global,
              bundle =>
              {
              bundle.AddFiles("/global-styles.css");
            }
          );
      //options
      //    .StyleBundles
      //    .Add(BasicThemeBundles.Styles.Global, bundle =>
      //    {
      //        bundle
      //            .AddBaseBundles(BasicThemeBundles.Styles.Global)
      //            .AddContributors(typeof(BasicThemeGlobalStyleContributor));
      //    });

      //options
      //    .ScriptBundles
      //    .Add(BasicThemeBundles.Scripts.Global, bundle =>
      //    {
      //        bundle
      //            .AddBaseBundles(BasicThemeBundles.Scripts.Global)
      //            .AddContributors(typeof(BasicThemeGlobalScriptContributor));
      //    });
    });
  }
}
