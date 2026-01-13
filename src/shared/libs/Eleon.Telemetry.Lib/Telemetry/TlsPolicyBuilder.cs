using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReliabilityOptions = SharedModule.modules.Otel.Module.OtelOptions.ReliabilityOptions;

namespace SharedModule.modules.Otel.Module;

/// <summary>
/// Builds HttpClientHandler/SocketsHttpHandler with TLS settings.
/// Supports system trust store, thumbprint pinning, client certificates, and dev-only insecure mode.
/// </summary>
internal static class TlsPolicyBuilder
{
  /// <summary>
  /// Creates an HttpClientHandler configured with TLS settings from reliability options.
  /// Returns null if no custom TLS configuration is needed (use default handler).
  /// </summary>
  public static HttpClientHandler? CreateHandler(
    ReliabilityOptions.TlsOptions? tlsOptions,
    IHostEnvironment hostEnvironment,
    ILogger? logger = null)
  {
    if (tlsOptions == null)
      return null;

    var handler = new HttpClientHandler();

    // Dev-only: AllowInvalidCertificate
    if (tlsOptions.AllowInvalidCertificate)
    {
      if (!hostEnvironment.IsDevelopment())
      {
        logger?.LogError("AllowInvalidCertificate TLS setting is only allowed in Development environment. TLS handler creation failed.");
        return null;
      }

      handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
      {
        logger?.LogWarning("TLS certificate validation bypassed (Development mode only): {Subject}", cert?.Subject);
        return true;
      };
    }

    // Thumbprint pinning
    if (tlsOptions.Mode == ReliabilityOptions.TlsOptions.TlsMode.PinnedThumbprint &&
        tlsOptions.PinnedThumbprints != null && tlsOptions.PinnedThumbprints.Length > 0)
    {
      var thumbprints = new HashSet<string>(tlsOptions.PinnedThumbprints, StringComparer.OrdinalIgnoreCase);
      handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
      {
        if (cert == null)
          return false;

        var certThumbprint = cert.GetCertHashString();
        if (thumbprints.Contains(certThumbprint))
        {
          return true;
        }

        logger?.LogWarning("TLS certificate thumbprint {Thumbprint} does not match pinned thumbprints", certThumbprint);
        return false;
      };
    }

    // Client certificate (mTLS) - path-based loading
    if (tlsOptions.ClientCertificate?.Path != null)
    {
      try
      {
        var cert = new X509Certificate2(tlsOptions.ClientCertificate.Path);
        handler.ClientCertificates.Add(cert);
        logger?.LogDebug("TLS client certificate loaded from path: {Path}", tlsOptions.ClientCertificate.Path);
      }
      catch (Exception ex)
      {
        logger?.LogError(ex, "Failed to load TLS client certificate from path: {Path}", tlsOptions.ClientCertificate.Path);
        return null;
      }
    }

    // Note: SecretKey-based client cert loading would require integration with secret management
    // This is left as a placeholder for future implementation
    if (tlsOptions.ClientCertificate?.SecretKey != null)
    {
      logger?.LogWarning("TLS client certificate loading from secret key is not yet implemented. Secret key: {SecretKey}", "[REDACTED]");
      // TODO: Implement secret key-based certificate loading when secret management is available
    }

    // Custom CA bundle (TlsMode.CustomCaBundle) - not implemented in v1
    if (tlsOptions.Mode == ReliabilityOptions.TlsOptions.TlsMode.CustomCaBundle)
    {
      logger?.LogWarning("Custom CA bundle TLS mode is not yet implemented");
      // TODO: Implement custom CA bundle loading
    }

    return handler;
  }
}
