using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Handlers;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logging.Module.ErrorHandling.Extensions;

public static class ErrorHandlingExtensions
{
  /// <summary>
  /// Adds exception handling services including ProblemDetails, exception mapper, enricher, and handler.
  /// </summary>
  public static IServiceCollection AddExceptionHandling(this IServiceCollection services, IConfiguration configuration, string sectionName = "ErrorHandling")
  {
    // Configure options
    services.Configure<ErrorHandlingOptions>(configuration.GetSection(sectionName));

    // Register ProblemDetails service
    services.AddProblemDetails(options =>
    {
      // Base customization - will be further enriched by IErrorEnricher
      options.CustomizeProblemDetails = ctx =>
      {
        var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["traceId"] = traceId;
      };
    });

    // Register exception mapper (default implementation)
    services.AddSingleton<IExceptionMapper, DefaultExceptionMapper>();

    // Register error enricher (default implementation)
    services.AddSingleton<IErrorEnricher, DefaultErrorEnricher>();

    // Register global exception handler
    services.AddExceptionHandler<GlobalExceptionHandler>();

    return services;
  }

  /// <summary>
  /// Uses the exception handling middleware. This is the modern approach using IExceptionHandler.
  /// </summary>
  public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
  {
    // Use the built-in exception handler middleware (which calls IExceptionHandler)
    app.UseExceptionHandler();
    return app;
  }
}
