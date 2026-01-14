using System;

namespace SharedModule.modules.Helpers.Module;

/// <summary>
/// Helper methods for checking idempotency conditions in domain operations.
/// </summary>
public static class IdempotencyHelpers
{
  /// <summary>
  /// Checks if the current status is in the desired final state.
  /// </summary>
  /// <typeparam name="TStatus">The status enum type</typeparam>
  /// <param name="currentStatus">The current status of the entity</param>
  /// <param name="desiredStatus">The desired final status</param>
  /// <param name="finalStatuses">Optional list of statuses that are considered "final"</param>
  /// <returns>True if current status matches desired status</returns>
  public static bool IsStatusEquals<TStatus>(
      TStatus currentStatus,
      TStatus desiredStatus,
      params TStatus[] finalStatuses)
      where TStatus : Enum
  {
    return currentStatus.Equals(desiredStatus);
  }

  /// <summary>
  /// Checks if the current status is already in a final state matching the desired outcome (success or failure).
  /// </summary>
  /// <typeparam name="TStatus">The status enum type</typeparam>
  /// <param name="currentStatus">The current status of the entity</param>
  /// <param name="successfully">True if the desired outcome is success, false for failure</param>
  /// <param name="successStatus">The status that represents success</param>
  /// <param name="failureStatus">The status that represents failure</param>
  /// <returns>True if current status matches the desired outcome</returns>
  public static bool IsFinalWithSameOutcome<TStatus>(
      TStatus currentStatus,
      bool successfully,
      TStatus successStatus,
      TStatus failureStatus)
      where TStatus : Enum
  {
    if (successfully)
    {
      return currentStatus.Equals(successStatus);
    }
    else
    {
      return currentStatus.Equals(failureStatus);
    }
  }

  /// <summary>
  /// Checks if a status is one of the final statuses.
  /// </summary>
  /// <typeparam name="TStatus">The status enum type</typeparam>
  /// <param name="status">The status to check</param>
  /// <param name="finalStatuses">List of statuses that are considered "final"</param>
  /// <returns>True if the status is in the final statuses list</returns>
  public static bool IsFinalStatus<TStatus>(
      TStatus status,
      params TStatus[] finalStatuses)
      where TStatus : Enum
  {
    foreach (var finalStatus in finalStatuses)
    {
      if (status.Equals(finalStatus))
      {
        return true;
      }
    }
    return false;
  }
}

