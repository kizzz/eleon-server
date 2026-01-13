using System;

namespace SharedModule.modules.Helpers.Module;

/// <summary>
/// Options for concurrency conflict handling and wait/retry behavior.
/// </summary>
public sealed class ConcurrencyHandlingOptions
{
  // Configuration section name (e.g., in appsettings.json).
  // Example:
  // "ConcurrencyHandling": {
  //   "MaxWait": "00:10:00",
  //   "BaseDelay": "00:00:00.100",
  //   "UseExponentialBackoff": false,
  //   "MaxDelay": "00:00:02",
  //   "LogEvery": "00:00:30"
  // }
  public const string DefaultSectionName = "ConcurrencyHandling";

  /// <summary>
  /// Maximum time to wait for desired state after a concurrency conflict.
  /// Default: 10 minutes.
  /// </summary>
  public TimeSpan MaxWait { get; set; } = TimeSpan.FromMinutes(10);

  /// <summary>
  /// Base delay between verification attempts.
  /// Default: 100ms.
  /// </summary>
  public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(100);

  /// <summary>
  /// Enable exponential backoff for delays.
  /// Default: false (linear).
  /// </summary>
  public bool UseExponentialBackoff { get; set; } = false;

  /// <summary>
  /// Maximum delay between attempts (applies to exponential backoff).
  /// Default: 2 seconds.
  /// </summary>
  public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(2);

  /// <summary>
  /// How often to log "still waiting" messages while retrying.
  /// Default: 30 seconds.
  /// </summary>
  public TimeSpan LogEvery { get; set; } = TimeSpan.FromSeconds(30);
}
