using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SharedModule.modules.Helpers.Module;

public enum ConcurrencyResolutionStrategy
{
  /// <summary>
  /// If DB already matches desired state => success, else throw conflict.
  /// Safest for many APIs because it never overwrites unknown changes.
  /// </summary>
  DesiredStateWins,

  /// <summary>
  /// Retry by reloading latest entity and applying the same mutation again.
  /// This is effectively "client/command wins" and can overwrite concurrent edits.
  /// Use only for idempotent/commutative operations or internal commands.
  /// </summary>
  RetryApply,

  /// <summary>
  /// Reload latest entity then run a merge delegate.
  /// The merge decides how to resolve field-by-field.
  /// </summary>
  CustomMerge,
}

public sealed record ConcurrencyResolutionOptions(
  ConcurrencyResolutionStrategy Strategy = ConcurrencyResolutionStrategy.DesiredStateWins,
  int MaxRetries = 3,
  TimeSpan? BaseDelay = null,
  bool UseExponentialBackoff = true
);

public static class ConcurrencyExtensions
{
  private static ConcurrencyHandlingOptions defaultOptions = new();

  public static ConcurrencyHandlingOptions DefaultOptions
  {
    get => defaultOptions;
    set => defaultOptions = value ?? new ConcurrencyHandlingOptions();
  }

  /// <summary>
  /// Robust update helper for entities that use ConcurrencyStamp (ABP optimistic concurrency).
  ///
  /// Best practice:
  /// - Interactive UI edits => Strategy = DesiredStateWins, and pass expectedConcurrencyStamp from client DTO.
  /// - Idempotent server commands => Strategy = RetryApply (or CustomMerge).
  /// </summary>
  public static async Task<TEntity> UpdateWithConcurrencyResolutionAsync<TEntity>(
    this IUnitOfWorkManager uowManager,
    IBasicRepository<TEntity, Guid> repository,
    Guid entityId,
    Action<TEntity> applyChanges,
    Func<TEntity, bool>? isDesiredState = null,
    string? expectedConcurrencyStamp = null,
    Func<TEntity, TEntity, bool>? merge = null, // (currentDb, incomingAppliedClone?) => merged?
    ConcurrencyResolutionOptions? options = null,
    ILogger? logger = null,
    string? operationName = null,
    CancellationToken cancellationToken = default
  )
    where TEntity : class, IEntity<Guid>, IHasConcurrencyStamp
  {
    options ??= new ConcurrencyResolutionOptions();
    operationName ??= $"Update<{typeof(TEntity).Name}>";

    if (options.Strategy == ConcurrencyResolutionStrategy.CustomMerge && merge is null)
    {
      throw new ArgumentException("CustomMerge strategy requires a merge delegate.", nameof(merge));
    }

    var baseDelay = options.BaseDelay ?? TimeSpan.FromMilliseconds(50);
    var rng = new Random();

    for (var attempt = 1; attempt <= options.MaxRetries; attempt++)
    {
      cancellationToken.ThrowIfCancellationRequested();

      try
      {
        using var uow = uowManager.Begin(requiresNew: true);

        // Load latest (tracked)
        var entity = await repository.GetAsync(entityId, cancellationToken: cancellationToken);

        // If you got concurrency stamp from client, set it before change.
        // This enforces "stale client" detection instead of last-write-wins.
        if (!string.IsNullOrWhiteSpace(expectedConcurrencyStamp))
        {
          entity.ConcurrencyStamp = expectedConcurrencyStamp!;
        }

        // Apply mutation
        applyChanges(entity);

        // Save
        await repository.UpdateAsync(entity, autoSave: false, cancellationToken: cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        await uow.CompleteAsync(cancellationToken);

        return entity;
      }
      catch (AbpDbConcurrencyException ex) when (attempt < options.MaxRetries)
      {
        logger?.LogWarning(
          ex,
          "Concurrency conflict in {Operation} for entity {EntityId} (attempt {Attempt}/{Max}).",
          operationName,
          entityId,
          attempt,
          options.MaxRetries
        );

        // Verify "already done" path first (safe + cheap)
        if (isDesiredState is not null)
        {
          var alreadyDone = await TryVerifyDesiredStateAsync(
            repository,
            entityId,
            isDesiredState,
            logger,
            operationName,
            cancellationToken
          );

          if (alreadyDone is not null)
          {
            return alreadyDone;
          }
        }

        // Policy choices
        if (options.Strategy == ConcurrencyResolutionStrategy.DesiredStateWins)
        {
          // Don't overwrite unknown concurrent changes.
          throw;
        }

        if (options.Strategy == ConcurrencyResolutionStrategy.CustomMerge)
        {
          // Fresh UoW for merge attempt; we'll do it immediately (still counts as retry loop)
          // We need latest DB entity; "incomingAppliedClone" is simulated by applying on a fresh load
          // then merging into another fresh load to avoid tracker weirdness.
          var merged = await TryMergeAndSaveAsync(
            uowManager,
            repository,
            entityId,
            applyChanges,
            merge!,
            expectedConcurrencyStamp,
            logger,
            operationName,
            cancellationToken
          );

          if (merged is not null)
          {
            return merged;
          }

          // Merge decided it cannot resolve -> throw conflict
          throw;
        }

        // RetryApply: just retry loop (reload latest next iteration)
        await DelayWithBackoffAsync(attempt, options, baseDelay, rng, cancellationToken);
        continue;
      }
      catch (AbpDbConcurrencyException ex)
      {
        // Last attempt or DesiredStateWins forced failure
        logger?.LogError(
          ex,
          "Concurrency conflict in {Operation} for entity {EntityId} after {Max} attempts.",
          operationName,
          entityId,
          options.MaxRetries
        );
        throw;
      }
    }

    // Unreachable due to throw/return, but compiler likes closure.
    throw new AbpDbConcurrencyException("Unexpected concurrency resolution flow.");
  }

  private static async Task<TEntity?> TryVerifyDesiredStateAsync<TEntity>(
    IBasicRepository<TEntity, Guid> repo,
    Guid id,
    Func<TEntity, bool> isDesiredState,
    ILogger? logger,
    string operationName,
    CancellationToken ct
  )
    where TEntity : class, IEntity<Guid>
  {
    var current = await repo.FindAsync(id, cancellationToken: ct);
    if (current is null)
      return null;

    if (isDesiredState(current))
    {
      logger?.LogWarning(
        "{Operation}: entity {EntityId} already in desired state after concurrency conflict. Treating as success.",
        operationName,
        id
      );
      return current;
    }

    return null;
  }

  private static async Task<TEntity?> TryMergeAndSaveAsync<TEntity>(
    IUnitOfWorkManager uowManager,
    IBasicRepository<TEntity, Guid> repo,
    Guid id,
    Action<TEntity> applyChanges,
    Func<TEntity, TEntity, bool> merge,
    string? expectedConcurrencyStamp,
    ILogger? logger,
    string operationName,
    CancellationToken ct
  )
    where TEntity : class, IEntity<Guid>, IHasConcurrencyStamp
  {
    // Build "incoming" by applying changes to a fresh copy (not tracked across UoWs).
    TEntity incoming;
    using (var uowIncoming = uowManager.Begin(requiresNew: true))
    {
      incoming = await repo.GetAsync(id, cancellationToken: ct);

      if (!string.IsNullOrWhiteSpace(expectedConcurrencyStamp))
      {
        incoming.ConcurrencyStamp = expectedConcurrencyStamp!;
      }

      applyChanges(incoming);
      await uowIncoming.CompleteAsync(ct); // no save, just detach when disposed
    }

    // Merge into latest and save
    using var uow = uowManager.Begin(requiresNew: true);
    var currentDb = await repo.GetAsync(id, cancellationToken: ct);

    var canResolve = merge(currentDb, incoming);
    if (!canResolve)
    {
      logger?.LogWarning("{Operation}: merge refused for entity {EntityId}.", operationName, id);
      return null;
    }

    await repo.UpdateAsync(currentDb, autoSave: false, cancellationToken: ct);
    await uow.SaveChangesAsync(ct);
    await uow.CompleteAsync(ct);

    logger?.LogInformation(
      "{Operation}: merge resolved concurrency for entity {EntityId}.",
      operationName,
      id
    );
    return currentDb;
  }

  private static async Task DelayWithBackoffAsync(
    int attempt,
    ConcurrencyResolutionOptions options,
    TimeSpan baseDelay,
    Random rng,
    CancellationToken ct
  )
  {
    var factor = options.UseExponentialBackoff ? Math.Pow(2, attempt - 1) : 1.0;
    var jitterMs = rng.Next(0, 25); // small jitter
    var delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor + jitterMs);
    await Task.Delay(delay, ct);
  }

  private static async Task DelayWithBackoffAsync(
    int attempt,
    ConcurrencyHandlingOptions options,
    Random rng,
    CancellationToken ct
  )
  {
    var factor = options.UseExponentialBackoff ? Math.Pow(2, attempt - 1) : 1.0;
    var jitterMs = rng.Next(0, 25);
    var delayMs = options.BaseDelay.TotalMilliseconds * factor + jitterMs;
    if (options.MaxDelay > TimeSpan.Zero)
    {
      delayMs = Math.Min(delayMs, options.MaxDelay.TotalMilliseconds);
    }
    var delay = TimeSpan.FromMilliseconds(delayMs);
    await Task.Delay(delay, ct);
  }

  /// <summary>
  /// Idempotent command helper:
  /// - Execute operation in a new UoW.
  /// - On AbpDbConcurrencyException: verify desired state; if true => success.
  /// - Otherwise retry with fresh UoW up to MaxRetries.
  ///
  /// Use for background jobs / message handlers that may execute more than once.
  /// </summary>
  public static async Task<TEntity> ExecuteIdempotentUpdateAsync<TEntity>(
    this IUnitOfWorkManager uowManager,
    IBasicRepository<TEntity, Guid> repository,
    Guid entityId,
    Action<TEntity> applyChanges,
    Func<TEntity, bool> isDesiredState,
    ILogger logger,
    string operationName,
    int maxRetries = 5,
    TimeSpan? baseDelay = null,
    CancellationToken ct = default
  )
    where TEntity : class, IEntity<Guid>
  {
    baseDelay ??= TimeSpan.FromMilliseconds(50);
    var rng = new Random();

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
      ct.ThrowIfCancellationRequested();

      try
      {
        using var uow = uowManager.Begin(requiresNew: true);

        var entity = await repository.GetAsync(entityId, cancellationToken: ct);
        applyChanges(entity);

        await repository.UpdateAsync(entity, autoSave: false, cancellationToken: ct);
        await uow.SaveChangesAsync(ct);
        await uow.CompleteAsync(ct);

        return entity;
      }
      catch (AbpDbConcurrencyException ex)
      {
        // 1) Verify desired state (safe resolution)
        var current = await repository.FindAsync(entityId, cancellationToken: ct);
        if (current is null)
          throw; // entity deleted; decide if you want to treat as success in your domain

        if (isDesiredState(current))
        {
          logger.LogWarning(
            ex,
            "{Operation}: concurrency conflict, but entity {EntityId} already in desired state. Success.",
            operationName,
            entityId
          );
          return current;
        }

        if (attempt == maxRetries)
        {
          logger.LogError(
            ex,
            "{Operation}: concurrency conflict for entity {EntityId} after {Attempts} attempts. Failing.",
            operationName,
            entityId,
            attempt
          );
          throw; // let ABP/job infra retry later or dead-letter
        }

        logger.LogWarning(
          ex,
          "{Operation}: concurrency conflict for entity {EntityId} (attempt {Attempt}/{Max}). Retrying...",
          operationName,
          entityId,
          attempt,
          maxRetries
        );

        // small exponential backoff + jitter
        var backoff = TimeSpan.FromMilliseconds(
          baseDelay.Value.TotalMilliseconds * Math.Pow(2, attempt - 1)
        );
        var jitter = TimeSpan.FromMilliseconds(rng.Next(0, 25));
        await Task.Delay(backoff + jitter, ct);
      }
    }

    throw new AbpDbConcurrencyException("Unreachable concurrency resolution flow.");
  }

  /// <summary>
  /// Verifies entity state after concurrency conflict and returns entity if in desired state.
  /// Useful for domain services that need to check if operation already completed.
  /// This is a backward-compatibility wrapper that uses the new infrastructure.
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  /// <param name="repository">The repository to query</param>
  /// <param name="entityId">The entity ID to verify</param>
  /// <param name="isDesiredState">Function to check if entity is in desired state</param>
  /// <param name="logger">Logger for logging concurrency conflicts</param>
  /// <param name="operationName">Name of the operation for logging purposes</param>
  /// <returns>The entity if in desired state</returns>
  /// <exception cref="EntityNotFoundException">Thrown if entity no longer exists</exception>
  /// <exception cref="InvalidOperationException">Thrown if entity is not in desired state</exception>
  public static async Task<TEntity> VerifyEntityStateAfterConflictAsync<TEntity>(
    this IBasicRepository<TEntity, Guid> repository,
    Guid entityId,
    Func<TEntity, bool> isDesiredState,
    ILogger logger,
    string operationName
  )
    where TEntity : class, IEntity<Guid>
  {
    var currentEntity = await repository.FindAsync(entityId);

    if (currentEntity == null)
    {
      logger.LogWarning(
        "Entity {EntityId} no longer exists while verifying concurrency conflict for {OperationName}.",
        entityId,
        operationName
      );
      throw new EntityNotFoundException(typeof(TEntity), entityId);
    }

    if (isDesiredState(currentEntity))
    {
      logger.LogWarning(
        "Entity {EntityId} already in desired state after concurrency conflict for {OperationName}. Treating as success.",
        entityId,
        operationName
      );
      return currentEntity;
    }

    logger.LogError(
      "Concurrency conflict for entity {EntityId} in {OperationName}: current state does not match desired outcome.",
      entityId,
      operationName
    );
    throw new InvalidOperationException(
      $"Entity {entityId} not in desired state after concurrency conflict."
    );
  }

  /// <summary>
  /// Executes an operation within a unit of work with automatic concurrency exception handling.
  /// If a concurrency exception occurs, verifies the current DB state and treats "already in desired state" as success.
  /// Uses fresh UoW per attempt to avoid EF change tracker issues.
  /// </summary>
  /// <typeparam name="TResult">The result type of the operation</typeparam>
  /// <param name="unitOfWorkManager">The unit of work manager</param>
  /// <param name="operation">The operation to execute within a unit of work</param>
  /// <param name="isOperationStillNeeded">Function to verify DB state when concurrency conflict occurs. Should return the result if already in desired state, or throw if real conflict.</param>
  /// <param name="logger">Logger for logging concurrency conflicts</param>
  /// <param name="operationNameForLog">Name of the operation for logging purposes</param>
  /// <param name="entityIdForLog">Optional entity ID for logging</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>The result of the operation</returns>
  public static async Task<TResult> ExecuteWithConcurrencyHandlingAsync<TResult>(
    this IUnitOfWorkManager unitOfWorkManager,
    Func<IUnitOfWork, Task<TResult>> operation,
    Func<Task<bool>>? isOperationStillNeeded,
    ILogger logger,
    string operationNameForLog,
    object? entityIdForLog = null,
    CancellationToken cancellationToken = default,
    int maxRetries = 0,
    TimeSpan? baseDelay = null,
    bool useExponentialBackoff = true,
    ConcurrencyHandlingOptions? options = null
  )
  {
    try
    {
      using var uow = unitOfWorkManager.Begin(requiresNew: true);
      var result = await operation(uow);
      await uow.SaveChangesAsync(cancellationToken);
      await uow.CompleteAsync(cancellationToken);
      return result;
    }
    catch (AbpDbConcurrencyException ex)
    {
      var effectiveOptions = ResolveOptions(options, maxRetries, baseDelay, useExponentialBackoff);
      var stopwatch = Stopwatch.StartNew();
      var rng = new Random();
      var attempt = 0;
      var lastLogAt = TimeSpan.Zero;

      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        attempt++;

        if (attempt == 1 || stopwatch.Elapsed - lastLogAt >= effectiveOptions.LogEvery)
        {
          logger.LogWarning(
            ex,
            "Concurrency conflict in {OperationName}{EntityId} (attempt {Attempt}). Verifying current DB state...",
            operationNameForLog,
            entityIdForLog != null ? $" for entity {entityIdForLog}" : string.Empty,
            attempt
          );
          lastLogAt = stopwatch.Elapsed;
        }

        // Verify what actually ended up in DB using fresh UoW

        bool operationStillNeeded = true;

        try
        {
          if (isOperationStillNeeded != null)
          {
            operationStillNeeded = await isOperationStillNeeded();
          }
        }
        catch (Exception verifyEx)
        {
          throw new Exception("Error verifying is operation still needed", verifyEx);
        }

        try
        {
          if (operationStillNeeded)
          {
            using var uow = unitOfWorkManager.Begin(requiresNew: true);
            var result = await operation(uow);
            await uow.SaveChangesAsync(cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            return result;
          }
          else if (isOperationStillNeeded != null)
          {
            logger.LogWarning(
              "Operation {OperationName}{EntityId} not required after concurrency conflict. Treating as success.",
              operationNameForLog,
              entityIdForLog != null ? $" for entity {entityIdForLog}" : string.Empty
            );
          }
        }
        catch (Exception verifyEx)
        {
          if (stopwatch.Elapsed >= effectiveOptions.MaxWait)
          {
            logger.LogError(
              ex,
              "Concurrency conflict in {OperationName}{EntityId}: desired state not reached after waiting {MaxWait}. Last verification error: {VerificationError}",
              operationNameForLog,
              entityIdForLog != null ? $" for entity {entityIdForLog}" : string.Empty,
              effectiveOptions.MaxWait,
              verifyEx.Message
            );
            throw; // Preserve stack trace
          }

          if (stopwatch.Elapsed - lastLogAt >= effectiveOptions.LogEvery)
          {
            logger.LogWarning(
              verifyEx,
              "Operation {OperationName}{EntityId} not in desired state after concurrency conflict. Waiting...",
              operationNameForLog,
              entityIdForLog != null ? $" for entity {entityIdForLog}" : string.Empty
            );
            lastLogAt = stopwatch.Elapsed;
          }

          await DelayWithBackoffAsync(attempt, effectiveOptions, rng, cancellationToken);
        }
      }
    }

    // Unreachable due to return/throw, but compiler likes closure.
    throw new AbpDbConcurrencyException("Unexpected concurrency resolution flow.");
  }

  public readonly record struct ConcurrencyWaitResult<TResult>(bool IsDesired, TResult? Result, string? Details);

  /// <summary>
  /// Waits for a desired state after a concurrency conflict by repeatedly verifying current state.
  /// Returns the verified result when desired; throws after timeout.
  /// </summary>
  public static async Task<TResult?> WaitForDesiredStateAsync<TResult>(
    Func<Task<ConcurrencyWaitResult<TResult>>> verify,
    ILogger logger,
    string operationName,
    object? entityId = null,
    ConcurrencyHandlingOptions? options = null,
    CancellationToken ct = default
  )
  {
    var effectiveOptions = ResolveOptions(options, 0, null, useExponentialBackoff: false);
    var stopwatch = Stopwatch.StartNew();
    var rng = new Random();
    var attempt = 0;
    var lastLogAt = TimeSpan.Zero;

    while (true)
    {
      ct.ThrowIfCancellationRequested();
      attempt++;

      if (attempt == 1 || stopwatch.Elapsed - lastLogAt >= effectiveOptions.LogEvery)
      {
        logger.LogWarning(
          "Concurrency conflict in {OperationName}{EntityId} (attempt {Attempt}). Waiting for desired state...",
          operationName,
          entityId != null ? $" for entity {entityId}" : string.Empty,
          attempt
        );
        lastLogAt = stopwatch.Elapsed;
      }

      var result = await verify();
      if (result.IsDesired)
      {
        logger.LogWarning(
          "Operation {OperationName}{EntityId} reached desired state after concurrency conflict.",
          operationName,
          entityId != null ? $" for entity {entityId}" : string.Empty
        );
        return result.Result;
      }

      if (stopwatch.Elapsed >= effectiveOptions.MaxWait)
      {
        logger.LogError(
          "Concurrency conflict in {OperationName}{EntityId}: desired state not reached after waiting {MaxWait}. Last state: {Details}",
          operationName,
          entityId != null ? $" for entity {entityId}" : string.Empty,
          effectiveOptions.MaxWait,
          result.Details ?? "unknown"
        );
        throw new AbpDbConcurrencyException(
          $"Concurrency wait timeout after {effectiveOptions.MaxWait} for {operationName}{(entityId != null ? $" ({entityId})" : string.Empty)}."
        );
      }

      await DelayWithBackoffAsync(attempt, effectiveOptions, rng, ct);
    }
  }

  private static ConcurrencyHandlingOptions ResolveOptions(
    ConcurrencyHandlingOptions? options,
    int maxRetries,
    TimeSpan? baseDelay,
    bool useExponentialBackoff
  )
  {
    var source = options ?? DefaultOptions;
    var resolved = new ConcurrencyHandlingOptions
    {
      MaxWait = source.MaxWait,
      BaseDelay = baseDelay ?? source.BaseDelay,
      UseExponentialBackoff = useExponentialBackoff ? true : source.UseExponentialBackoff,
      MaxDelay = source.MaxDelay,
      LogEvery = source.LogEvery
    };

    if (maxRetries > 0)
    {
      var delayMs = resolved.BaseDelay.TotalMilliseconds;
      if (delayMs <= 0)
      {
        delayMs = 50;
      }
      resolved.MaxWait = TimeSpan.FromMilliseconds(delayMs * maxRetries);
    }

    return resolved;
  }
}
