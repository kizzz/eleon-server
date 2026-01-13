using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
public class CheckConfigurationMiddleware : IMiddleware
{
  private readonly ILogger<CheckConfigurationMiddleware> _logger;
  private readonly CheckConfigurationService _checkConfigurationService;
  private readonly CheckConfigurationOptions _options;

  public CheckConfigurationMiddleware(
      ILogger<CheckConfigurationMiddleware> logger,
      IOptions<CheckConfigurationOptions> options,
      CheckConfigurationService checkConfigurationService)
  {
    _logger = logger;
    _checkConfigurationService = checkConfigurationService;
    _options = options.Value;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    _logger.LogDebug("Checking configuration started");
    try
    {
      if (_options?.Enabled == true)
      {
        var result = await _checkConfigurationService.CheckAsync();
        if (!result.Key && _options.ThrowExceptionOnInvalid)
        {
          var exception = new Exception("Configuration check failed.")
              .WithStatusCode(EleonsoftStatusCodes.Default.BadConfiguration)
              .WithMessageAsFriendly();

          foreach (var item in result.Value)
          {
            if (item.Value.IsErrored)
              exception.Data[item.Key] = item.Value.ErrorMessage;
          }

          throw exception;
        }
      }
    }
    finally
    {
      _logger.LogDebug("Checking configuration finished");
    }

    await next(context);
  }
}
