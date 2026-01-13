using System;
using System.Collections.Generic;
using System.Threading;

namespace SharedModule.modules.Otel.Module;

/// <summary>
/// In-process circuit breaker for telemetry exporters.
/// Maintains state per key (signal type + endpoint).
/// State machine: Closed → Open (on failure threshold) → HalfOpen (after open duration) → Closed/Open (on probe result).
/// </summary>
public sealed class TelemetryCircuitBreaker
{
  private readonly Dictionary<string, BreakerState> _states = new();
  private readonly object _lock = new();

  private class BreakerState
  {
    public CircuitState State { get; set; } = CircuitState.Closed;
    public int FailureCount { get; set; }
    public DateTime LastFailureTime { get; set; }
    public DateTime? OpenedAt { get; set; }
    public int HalfOpenAttempts { get; set; }
  }

  private enum CircuitState
  {
    Closed,
    Open,
    HalfOpen
  }

  /// <summary>
  /// Records a failure for the given key.
  /// </summary>
  public void RecordFailure(string key, int failureThreshold, TimeSpan samplingWindow)
  {
    lock (_lock)
    {
      if (!_states.TryGetValue(key, out var state))
      {
        state = new BreakerState();
        _states[key] = state;
      }

      // Reset failure count if sampling window has expired
      if (state.State == CircuitState.Closed && state.FailureCount > 0)
      {
        var timeSinceLastFailure = DateTime.UtcNow - state.LastFailureTime;
        if (timeSinceLastFailure > samplingWindow)
        {
          state.FailureCount = 0;
        }
      }

      state.FailureCount++;
      state.LastFailureTime = DateTime.UtcNow;

      // Open circuit if threshold exceeded
      if (state.State == CircuitState.Closed && state.FailureCount >= failureThreshold)
      {
        state.State = CircuitState.Open;
        state.OpenedAt = DateTime.UtcNow;
        state.HalfOpenAttempts = 0;
      }
    }
  }

  /// <summary>
  /// Records a success for the given key.
  /// </summary>
  public void RecordSuccess(string key)
  {
    lock (_lock)
    {
      if (!_states.TryGetValue(key, out var state))
        return;

      if (state.State == CircuitState.HalfOpen)
      {
        // Success in half-open → close circuit
        state.State = CircuitState.Closed;
        state.FailureCount = 0;
        state.HalfOpenAttempts = 0;
        state.OpenedAt = null;
      }
      else if (state.State == CircuitState.Closed)
      {
        // Reset failure count on success
        state.FailureCount = 0;
      }
    }
  }

  /// <summary>
  /// Checks if the circuit is open for the given key.
  /// </summary>
  public bool IsOpen(string key, TimeSpan openDuration, int halfOpenMaxAttempts)
  {
    lock (_lock)
    {
      if (!_states.TryGetValue(key, out var state))
        return false;

      if (state.State == CircuitState.Open)
      {
        // Check if we should transition to half-open
        if (state.OpenedAt.HasValue)
        {
          var timeSinceOpened = DateTime.UtcNow - state.OpenedAt.Value;
          if (timeSinceOpened >= openDuration)
          {
            state.State = CircuitState.HalfOpen;
            state.HalfOpenAttempts = 0;
            return false; // Half-open allows one attempt
          }
        }
        return true;
      }

      if (state.State == CircuitState.HalfOpen)
      {
        // Check if half-open attempts exceeded
        if (state.HalfOpenAttempts >= halfOpenMaxAttempts)
        {
          // Re-open circuit
          state.State = CircuitState.Open;
          state.OpenedAt = DateTime.UtcNow;
          state.HalfOpenAttempts = 0;
          return true;
        }
        // Increment attempt counter (caller should call RecordSuccess/RecordFailure)
        state.HalfOpenAttempts++;
        return false; // Allow the attempt
      }

      return false; // Closed state
    }
  }

  /// <summary>
  /// Gets the current state (for metrics/observability).
  /// </summary>
  public bool GetIsOpen(string key)
  {
    lock (_lock)
    {
      if (!_states.TryGetValue(key, out var state))
        return false;
      return state.State == CircuitState.Open;
    }
  }

  /// <summary>
  /// Clears state for a key (useful for testing or cleanup).
  /// </summary>
  public void Clear(string key)
  {
    lock (_lock)
    {
      _states.Remove(key);
    }
  }
}
