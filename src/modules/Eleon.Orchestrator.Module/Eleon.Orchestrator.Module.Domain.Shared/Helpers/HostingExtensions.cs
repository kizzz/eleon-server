using EleonsoftModuleCollector.Commons.Module.BasicNotificators.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServicesOrchestrator.HealthChecks;
using ServicesOrchestrator.Services;
using ServicesOrchestrator.Services.Abstractions;
using ServicesOrchestrator.Services.ServiceHandlers;

namespace ServicesOrchestrator.Helpers;

public static class HostingExtensions
{
  public static IApplicationBuilder UseOrchestratorEndpoints(this IApplicationBuilder app, string basePath)
  {
    app.UseEndpoints(endpoints =>
    {
      endpoints.MapGet($"{basePath}/", async (IOrchestratingService svc, CancellationToken ct)
              => Results.Json(await svc.GetStatusAsync(ct)));

      endpoints.MapGet($"{basePath}/api", async (IOrchestratingService svc, CancellationToken ct)
              => Results.Json(await svc.GetStatusAsync(ct)));

      endpoints.MapGet($"{basePath}/start", async (IOrchestratingService svc, CancellationToken ct) =>
          {
          await svc.StartAsync(ct);
          return Results.Json(await svc.GetStatusAsync(ct));
        });

      endpoints.MapGet($"{basePath}/stop", async (IOrchestratingService svc, CancellationToken ct) =>
          {
          await svc.StopAsync(ct);
          return Results.Json(await svc.GetStatusAsync(ct));
        });
    });

    return app;
  }

  public static IServiceCollection AddOrchestrator(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddSingleton<ServiceLifecycleManager>();
    services.AddTransient<IAlertService, AlertService>();
    services.AddSingleton<IOrchestratingService, OrchestratingService>();
    services.AddSingleton<IOrchestratingConfigurationService, AppsettingsOrchestratingConfigurationService>();
    //services.AddSingleton<IServiceHandler, DatabaseServiceHandler>();
    //services.AddSingleton<IServiceHandler, ConnectionServiceHandler>();
    //services.AddSingleton<IServiceHandler, AppServiceHandler>();
    //services.AddSingleton<IServiceHandler, WebAppServiceHandler>();
    services.AddTransient<ServicesOrchestratingLoopService>();
    services.AddHostedService<ServicesOrchestratingLoopService>();
    services.AddBasicNotificatorServices();
    services.Configure<OrchestratorOptions>(configuration.GetSection(OrchestratorOptions.SectionName));
    services.Configure<OrchestratorManifest>(configuration.GetSection("Orchestrator:Manifest"));

    return services;
  }
}
