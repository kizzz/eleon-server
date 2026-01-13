using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EleonsoftSdk.modules.HealthCheck.Module.Api;

/// <summary>
/// Minimal API endpoints for health checks.
/// </summary>
public static class HealthCheckEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static ILogger GetLogger(HttpContext context)
        => context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("HealthCheckEndpoints");

    /// <summary>
    /// Maps health check endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/health");

        // Anonymous endpoints
        group.MapGet("/live", HandleLive)
            .WithName("HealthLive")
            .AllowAnonymous();

        group.MapGet("/ready", HandleReady)
            .WithName("HealthReady")
            .AllowAnonymous();

        // Authenticated endpoints
        group.MapGet("/diag", HandleDiagnostics)
            .WithName("HealthDiagnostics")
            .RequireAuthorization();

        group.MapPost("/run", HandleRun)
            .WithName("HealthRun")
            .RequireAuthorization();

        group.MapGet("/ui", HandleUI)
            .WithName("HealthUI")
            .RequireAuthorization();

        return endpoints;
    }

    private static async Task HandleLive(HttpContext context)
    {
        // Minimal liveness check - just return OK
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsync("OK");
    }

    private static async Task HandleReady(
        HttpContext context,
        HealthCheckService healthCheckService,
        IHealthReportBuilder reportBuilder,
        IOptions<HealthCheckOptions> options)
    {
        try
        {
            // Run only readiness checks (tag: "ready")
            var healthReport = await healthCheckService.CheckHealthAsync(
                check => true, // Will filter by tags in registration
                context.RequestAborted);

            var overallHealthy = healthReport.Status == HealthStatus.Healthy || 
                                healthReport.Status == HealthStatus.Degraded;

            context.Response.StatusCode = overallHealthy 
                ? StatusCodes.Status200OK 
                : StatusCodes.Status503ServiceUnavailable;

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                new { status = healthReport.Status.ToString() },
                JsonOptions,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            var logger = GetLogger(context);
            logger.LogError(ex, "Error in readiness check");
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Unhealthy");
        }
    }

    private static async Task HandleDiagnostics(
        HttpContext context,
        IHealthRunCoordinator coordinator,
        IHealthReportBuilder reportBuilder,
        IOptions<HealthCheckOptions> options)
    {
        var logger = GetLogger(context);

        try
        {
            // Run all checks including diagnostics
            var snapshot = await coordinator.RunAsync(
                type: "diagnostics",
                initiatorName: context.User?.Identity?.Name ?? "anonymous",
                options: new HealthRunOptions
                {
                    CheckTimeoutSeconds = options.Value.CheckTimeout,
                    EnableDiagnostics = true,
                    Tags = new[] { "live", "ready", "diag" }
                },
                ct: context.RequestAborted);

            if (snapshot == null)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync("Health check already in progress");
                return;
            }

            // Determine if user is privileged (admin)
            var isPrivileged = context.User?.IsInRole("Admin") ?? false;

            // Scrub sensitive data if not privileged
            var eto = isPrivileged 
                ? snapshot.HealthCheck 
                : reportBuilder.ScrubSensitiveData(snapshot.HealthCheck, isPrivileged);

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                eto,
                JsonOptions,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in diagnostics check");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Error");
        }
    }

    private static async Task HandleRun(
        HttpContext context,
        IHealthRunCoordinator coordinator,
        IOptions<HealthCheckOptions> options)
    {
        var logger = GetLogger(context);

        try
        {
            var snapshot = await coordinator.RunAsync(
                type: "manual",
                initiatorName: context.User?.Identity?.Name ?? "anonymous",
                options: new HealthRunOptions
                {
                    CheckTimeoutSeconds = options.Value.CheckTimeout
                },
                ct: context.RequestAborted);

            if (snapshot == null)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync("Health check already in progress");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status202Accepted;
            await context.Response.WriteAsync($"Health check started: {snapshot.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error triggering health check");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Error");
        }
    }

    private static async Task HandleUI(
        HttpContext context,
        IHealthRunCoordinator coordinator,
        IHealthReportBuilder reportBuilder,
        IOptions<HealthCheckOptions> options)
    {
        // Delegate to existing UI helper (static method)
        await HealthCheckStaticPageHelper.HandleUIPageAsync(context);
    }
}
