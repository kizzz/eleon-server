using Eleon.Logging.Lib.SystemLog.Logger;
using EventManagementModule.Module.Domain.Shared.Entities;
using EventManagementModule.Module.Domain.Shared.Repositories;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.EntityFrameworkCore.Repositories;
using Serilog;
using SharedModule.modules.Logging.Module.SystemLog;
using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.ChangeTracking;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Threading;
using VPortal.EventManagementModule.Module.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCor.Repositories;


[DisableEntityChangeTracking]
public class QueueRepository : EfCoreRepository<EventManagementModuleDbContext, QueueEntity, Guid>, IQueueRepository
{
  private readonly IVportalLogger<QueueRepository> _logger;
  private readonly ConcurrencyManager _concurrencyManager;

  public QueueRepository(
          IDbContextProvider<EventManagementModuleDbContext> dbContextProvider,
          IVportalLogger<QueueRepository> logger,
          ConcurrencyManager concurrencyManager)
          : base(dbContextProvider)
  {
    _logger = logger;
    _concurrencyManager = concurrencyManager;
  }

  #region Overrides for concurrency management

  public async override Task<QueueEntity> UpdateAsync(QueueEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

    await sem.WaitAsync();
    try
    {
      entity = await base.UpdateAsync(entity, true, cancellationToken);
    }
    finally
    {
      sem.Release();
    }

    return entity;
  }

  public async override Task UpdateManyAsync(IEnumerable<QueueEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    foreach (var entity in entities)
    {
      var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

      await sem.WaitAsync();
      try
      {
        await base.UpdateAsync(entity, true, cancellationToken);
      }
      finally
      {
        sem.Release();
      }
    }
  }

  public async override Task DeleteAsync(QueueEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

    await sem.WaitAsync();
    try
    {
      await base.DeleteAsync(entity, true, cancellationToken);
    }
    finally
    {
      sem.Release();
    }
  }

  public async override Task DeleteManyAsync(IEnumerable<QueueEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    foreach (var entity in entities)
    {
      var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

      await sem.WaitAsync();
      try
      {
        await base.DeleteAsync(entity, true, cancellationToken);
      }
      finally
      {
        sem.Release();
      }
    }
  }

  public async override Task DeleteAsync(Guid id, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    var entity = await GetAsync(id, cancellationToken: cancellationToken);

    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

    await sem.WaitAsync();
    try
    {
      await base.DeleteAsync(id, true, cancellationToken);
    }
    finally
    {
      sem.Release();
    }
  }

  public async override Task DeleteManyAsync(IEnumerable<Guid> ids, bool autoSave = true, CancellationToken cancellationToken = default)
  {
    foreach (var id in ids)
    {
      var entity = await GetAsync(id, cancellationToken: cancellationToken);

      var sem = await _concurrencyManager.GetLockAsync(GetLockKey(entity));

      await sem.WaitAsync();
      try
      {
        await base.DeleteAsync(id, true, cancellationToken);
      }
      finally
      {
        sem.Release();
      }
    }
  }

  public async override Task<IQueryable<QueueEntity>> WithDetailsAsync()
  {
    var query = await base.WithDetailsAsync();
    return query; // include message not recommended for performance reasons
  }

  public async override Task<List<QueueEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default(CancellationToken))
  {
    var query = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();

    if (!sorting.IsNullOrWhiteSpace())
    {
      var parts = sorting.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      var field = parts[0];
      var direction = parts.Length > 1 && parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

      if (field.Equals("fillPercentage", StringComparison.OrdinalIgnoreCase))
      {
        query = direction == "asc"
            ? query.OrderBy(q => q.MessagesLimit > 0 ? (double)q.Count / q.MessagesLimit : 0)
            : query.OrderByDescending(q => q.MessagesLimit > 0 ? (double)q.Count / q.MessagesLimit : 0);
      }
      else
      {
        query = query.OrderBy($"{field} {direction}");
      }
    }
    else
    {
      query = query.OrderByDescending(q => q.CreationTime);
    }

    query = query.PageBy(skipCount, maxResultCount);

    return await query.ToListAsync(GetCancellationToken(cancellationToken));
  }

  #endregion

  public async Task<List<EventEntity>> GetMessagesListAsync(Guid queueId, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, bool includeDetails = true)
  {
    var query = (await GetDbContextAsync()).EventManagementEvents.AsQueryable();

    query = query
        .Where(e => e.QueueId == queueId)
        .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(EventEntity.CreationTime) + " desc" : sorting)
        .PageBy(skipCount, maxResultCount);

    return await query.ToListAsync(GetCancellationToken());
  }

  public async Task EnqueueAsync(Guid queueId, EventEntity message)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));

    await sem.WaitAsync(GetCancellationToken());
    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      await PrivateEnqueueAsync(dbContext, queue, message);
      await dbContext.SaveChangesAsync(GetCancellationToken());
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task EnqueueManyAsync(Guid queueId, IList<EventEntity> messages)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));
    await sem.WaitAsync(GetCancellationToken());

    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      foreach (var message in messages)
      {
        await PrivateEnqueueAsync(dbContext, queue, message);
      }

      await dbContext.SaveChangesAsync(GetCancellationToken());
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task<EventEntity> DequeueAsync(Guid queueId)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));
    await sem.WaitAsync(GetCancellationToken());

    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      var message = await PrivateDequeueAsync(dbContext, queue);
      await dbContext.SaveChangesAsync(GetCancellationToken());
      return message;
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task<List<EventEntity>> DequeueManyAsync(Guid queueId, int count)
  {
    if (count <= 0)
      return new List<EventEntity>();

    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));
    await sem.WaitAsync(GetCancellationToken());

    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      var dequeued = new List<EventEntity>();
      for (int i = 0; i < count; i++)
      {
        var message = await PrivateDequeueAsync(dbContext, queue);
        if (message != null)
        {
          dequeued.Add(message);
        }
        else
        {
          break;
        }
      }

      await dbContext.SaveChangesAsync(GetCancellationToken());

      return dequeued;
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task ClearAsync(Guid queueId)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));
    await sem.WaitAsync(GetCancellationToken());

    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      await PrivateClear(dbContext, queue);
      await dbContext.SaveChangesAsync(GetCancellationToken());
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task SetMessagesLimitAsync(Guid queueId, int newLimit)
  {
    var sem = await _concurrencyManager.GetLockAsync(GetLockKey(queueId));
    await sem.WaitAsync(GetCancellationToken());

    try
    {
      var dbContext = await GetDbContextAsync();
      var queue = await GetQueueAsync(dbContext, queueId);
      if (newLimit < queue.MessagesLimit)
      {
        var toRemove = queue.Count - newLimit;
        if (toRemove > 0)
        {
          for (int i = 0; i < toRemove; i++)
          {
            await PrivateDequeueAsync(dbContext, queue);
          }
        }
      }

      queue.MessagesLimit = newLimit;

      await dbContext.SaveChangesAsync(GetCancellationToken());
    }
    finally
    {
      sem.Release();
    }
  }

  public async Task<QueueEntity> FindByNameAsync(string name, bool includeDetails = true)
  {
    var query = includeDetails ? await WithDetailsAsync() : await GetQueryableAsync();
    return await query
        .Where(q => q.Name == name)
        .FirstOrDefaultAsync(GetCancellationToken());
  }

  #region Private Methods

  private async Task ValidateQueueAsync(EventManagementModuleDbContext dbContext, QueueEntity queue, bool force = false)
  {
    if (queue.Count == 0 && !queue.Head.HasValue && !queue.Tail.HasValue) // queue is empty and valid
    {
      return;
    }

    if (queue.Count > 0 && queue.Head.HasValue && queue.Tail.HasValue) // queue has messages and seems valid
    {
      if (!force)
      {
        return; // Assume valid if not forced
      }

      // Check if head and tail exist in messages
      var headExists = await dbContext.EventManagementEvents.AnyAsync(e => e.Id == queue.Head.Value && e.QueueId == queue.Id);
      var tailExists = await dbContext.EventManagementEvents.AnyAsync(e => e.Id == queue.Tail.Value && e.QueueId == queue.Id);
      if (headExists && tailExists)
      {
        return; // Queue is valid
      }
    }

    _logger.Log.LogError("Queue {QueueId} ({QueueName}) is invalid. Reason: count: {messagesCount}, next: {head}, prev: {tail}. Starting repair...", queue.Id, queue.Name, queue.Count, queue.Head, queue.Tail);

    // repairing queue
    if (queue.Count < 0 || (queue.Count == 0 && (queue.Head.HasValue || queue.Tail.HasValue)))
    {
      queue.Count = await dbContext.EventManagementEvents
          .Where(e => e.QueueId == queue.Id)
          .CountAsync();

      if (queue.Count == 0)
      {
        queue.Head = null;
        queue.Tail = null;
      }
      else
      {
        var headExists = queue.Head.HasValue && await dbContext.EventManagementEvents.AnyAsync(e => e.Id == queue.Head.Value && e.QueueId == queue.Id);
        var tailExists = queue.Tail.HasValue && await dbContext.EventManagementEvents.AnyAsync(e => e.Id == queue.Tail.Value && e.QueueId == queue.Id);

        if (!headExists) queue.Head = null;
        if (!tailExists) queue.Tail = null;
      }
    }

    if (queue.Count > 0 && (!queue.Head.HasValue || !queue.Tail.HasValue))
    {
      // if queue is too large, find head and tail from database
      if (queue.Count > 10_000)
      {
        var firstEvent = await dbContext.EventManagementEvents
        .Where(e => e.QueueId == queue.Id)
        .OrderBy(e => e.CreationTime)
        .FirstOrDefaultAsync();
        var lastEvent = await dbContext.EventManagementEvents
            .Where(e => e.QueueId == queue.Id)
            .OrderByDescending(e => e.CreationTime)
            .FirstOrDefaultAsync();
        queue.Head = firstEvent?.Id;
        queue.Tail = lastEvent?.Id;
      }
      else
      {
        // more copmlex validation for smaller queues
        var events = await dbContext.EventManagementEvents
            .Where(e => e.QueueId == queue.Id)
            .ToListAsync();

        if (events.Count == 0)
        {
          queue.Head = null;
          queue.Tail = null;
          queue.Count = 0;
          return;
        }

        // Build a dictionary for quick lookup
        var eventDict = events.ToDictionary(e => e.Id, e => e);

        // Find all events that are not pointed to by any other event's Next
        var headCandidates = events.Where(e => !events.Any(ev => ev.Next == e.Id)).ToList();
        var head = headCandidates.OrderBy(e => e.CreationTime).FirstOrDefault();
        head ??= events.OrderBy(e => e.CreationTime).FirstOrDefault();
        queue.Head = head?.Id;

        // Traverse the chain from head, repairing links and collecting visited
        var visited = new HashSet<Guid>();
        var current = head;
        EventEntity prev = null;
        while (current != null)
        {
          visited.Add(current.Id);

          // Repair Prev pointer
          if (prev != null && current.Prev != prev.Id)
          {
            current.Prev = prev.Id;
            dbContext.EventManagementEvents.Attach(current);
            dbContext.Entry(current).Property(x => x.Prev).IsModified = true;
          }

          // Find next event
          EventEntity next = null;
          if (current.Next.HasValue && eventDict.TryGetValue(current.Next.Value, out next))
          {
            // Valid next
          }
          else
          {
            // Find next by creation time if Next is broken
            next = events
                .Where(e => !visited.Contains(e.Id) && e.CreationTime > current.CreationTime)
                .OrderBy(e => e.CreationTime)
                .FirstOrDefault();

            if (next != null)
            {
              current.Next = next.Id;
              dbContext.EventManagementEvents.Attach(current);
              dbContext.Entry(current).Property(x => x.Next).IsModified = true;
            }
            else
            {
              current.Next = null;
              dbContext.EventManagementEvents.Attach(current);
              dbContext.Entry(current).Property(x => x.Next).IsModified = true;
            }
          }

          prev = current;
          current = next;
        }

        // The last visited event is the tail
        queue.Tail = prev?.Id;

        // Add any unvisited events to the end of the chain
        var unlinkedEvents = events.Where(e => !visited.Contains(e.Id)).OrderBy(e => e.CreationTime).ToList();
        foreach (var unlinked in unlinkedEvents)
        {
          if (queue.Tail.HasValue)
          {
            var tailEvent = eventDict[queue.Tail.Value];
            tailEvent.Next = unlinked.Id;
            dbContext.EventManagementEvents.Attach(tailEvent);
            dbContext.Entry(tailEvent).Property(x => x.Next).IsModified = true;

            unlinked.Prev = tailEvent.Id;
            dbContext.EventManagementEvents.Attach(unlinked);
            dbContext.Entry(unlinked).Property(x => x.Prev).IsModified = true;
          }
          else
          {
            queue.Head = unlinked.Id;
            unlinked.Prev = null;
            dbContext.EventManagementEvents.Attach(unlinked);
            dbContext.Entry(unlinked).Property(x => x.Prev).IsModified = true;
          }
          queue.Tail = unlinked.Id;
          visited.Add(unlinked.Id);
        }

        // Update queue count
        queue.Count = visited.Count;
      }
    }
  }

  private async Task<QueueEntity> GetQueueAsync(EventManagementModuleDbContext dbContext, Guid queueId)
  {
    return await dbContext.EventManagementQueues.FirstOrDefaultAsync(q => q.Id == queueId) ?? throw new EntityNotFoundException(typeof(QueueEntity), queueId);
  }

  private async Task PrivateEnqueueAsync(EventManagementModuleDbContext dbContext, QueueEntity queue, EventEntity @event)
  {
    await ValidateQueueAsync(dbContext, queue);

    // Handle queue limit
    if (queue.MessagesLimit > 0 && queue.Count >= queue.MessagesLimit)
    {
      var dequeueCount = queue.Count - queue.MessagesLimit + 1;
      for (int i = 0; i < dequeueCount; i++)
      {
        await PrivateDequeueAsync(dbContext, queue); // Call to dequeue when limit is exceeded
      }
    }

    // Set up the new event
    @event.QueueId = queue.Id;
    @event.Next = null;
    @event.Prev = queue.Tail; // Link to current tail

    if (queue.Tail.HasValue)
    {
      // Update the current tail to point to new event
      await dbContext.EventManagementEvents
          .Where(e => e.Id == queue.Tail.Value)
          .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.Next, @event.Id));

      // New event becomes the tail
      queue.Tail = @event.Id;
    }
    else
    {
      // First message in queue
      queue.Head = @event.Id;
      queue.Tail = @event.Id;
    }

    // Add the new event to database
    await dbContext.EventManagementEvents.AddAsync(@event);

    // Update queue properties
    queue.Count++;
  }

  private async Task<EventEntity> PrivateDequeueAsync(EventManagementModuleDbContext dbContext, QueueEntity queue)
  {
    await ValidateQueueAsync(dbContext, queue);

    // Check if queue is empty
    if (queue.Count == 0 && !queue.Head.HasValue && !queue.Tail.HasValue)
    {
      return null;
    }

    // Get the head event directly from database without loading all messages
    var headEvent = await dbContext.EventManagementEvents
        .FirstOrDefaultAsync(e => e.Id == queue.Head.Value && e.QueueId == queue.Id);

    if (headEvent == null)
    {
      // todo notify invalid queue
      await ValidateQueueAsync(dbContext, queue, true);
      return null;
    }

    // Update queue properties
    queue.Count--;


    if (queue.Count == 0)
    {
      // Queue becomes empty
      queue.Head = null;
      queue.Tail = null;

      // Clean up the dequeued event's links
      headEvent.Next = null;
      headEvent.Prev = null;

      // Remove the event from database
      dbContext.EventManagementEvents.Remove(headEvent);
    }
    else if (headEvent.Next.HasValue)
    {
      // Update the next event to remove its Prev reference
      await dbContext.EventManagementEvents
          .Where(e => e.Id == headEvent.Next.Value)
          .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.Prev, (Guid?)null));

      // Update queue head to point to next event
      queue.Head = headEvent.Next.Value;

      // Clean up the dequeued event's links
      headEvent.Next = null;
      headEvent.Prev = null;

      // Remove the event from database
      dbContext.EventManagementEvents.Remove(headEvent);
    }
    else
    {
      var nextEvent = await dbContext.EventManagementEvents
          .Where(e => e.QueueId == queue.Id && e.CreationTime > headEvent.CreationTime)
          .OrderBy(e => e.CreationTime)
          .FirstOrDefaultAsync();

      headEvent.Next = null;
      headEvent.Prev = null;
      dbContext.EventManagementEvents.Remove(headEvent);

      if (nextEvent != null)
      {
        await dbContext.EventManagementEvents
            .Where(e => e.Id == nextEvent.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Prev, (Guid?)null));

        queue.Head = nextEvent.Id;
      }
      else
      {
        queue.Head = null;
        EleonsoftLog.Error($"Queue {queue.Name}: head has no Next while Count>0 — fallback to full repair");
        await ValidateQueueAsync(dbContext, queue, force: true);
      }
    }

    return headEvent;
  }

  private static async Task PrivateClear(EventManagementModuleDbContext dbContext, QueueEntity queue)
  {
    queue.Count = 0;
    queue.Head = null;
    queue.Tail = null;
    await dbContext.EventManagementEvents
        .Where(e => e.QueueId == queue.Id)
        .ExecuteDeleteAsync();
  }

  private static string GetLockKey(QueueEntity entity) => $"{entity.TenantId}:{entity.Id}";
  private string GetLockKey(Guid queueId) => $"{CurrentTenant.Id}:{queueId}";

  #endregion
}
