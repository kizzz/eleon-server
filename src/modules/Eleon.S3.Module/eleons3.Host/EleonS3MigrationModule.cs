using Authorization.Module.TenantHostname;
using Common.EventBus.Module;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace VPortal;

[Volo.Abp.Modularity.DependsOn(
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),

    typeof(CommonEventBusModule),
    typeof(AbpAutofacModule)
    )]
public class EleonS3MigrationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EleonS3HttpApiHostModule>(validate: true);
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = false;
            options.SaveStaticPermissionsToDatabase = false;
        });

        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc; // set UTC datetime to the application (ex. CreationTime will be in UTC)
        });

        ConfigureTenentResolvement(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureAuditing();

        // Migrated to V2 HealthChecks architecture
        // Old: context.Services.AddCommonHealthChecks(configuration);
        // Register core infrastructure first
        context.Services.AddEleonHealthChecksCore(configuration);
        // Register all health checks
        context.Services.AddHealthChecks()
            .AddEleonHealthChecksAll(configuration);
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("VPortal");
        if (!hostingEnvironment.IsDevelopment())
        {
            //var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            //dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "VPortal-Protection-Keys");
        }
    }

    private void ConfigureTenentResolvement(IConfiguration cfg)
    {
        Configure<AbpTenantResolveOptions>(options =>
        {
            options.AddHostnameTenantResolver();
        });
    }

    private void ConfigureAuditing()
    {
        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "EleonS3";
        });
    }
}
