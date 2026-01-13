using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

/// <summary>
/// Service for restarting the application.
/// Security: Should be gated by authorization and off by default.
/// </summary>
public class RestartService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<RestartService> _logger;

    public RestartService(
        IHostApplicationLifetime lifetime,
        ILogger<RestartService> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
    }

    public void Restart()
    {
        _logger.LogWarning("Application restart requested");
        _lifetime.StopApplication();
    }
}
