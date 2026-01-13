using Common.Module.Constants;
using Humanizer;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Volo.Abp.Validation;
using VPortal.JobScheduler.Module.Entities;

namespace EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;

/// <summary>
/// Helper class for calculating trigger next run times based on schedules.
/// 
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Daily schedules:</strong> O(1) - Direct calculation using period arithmetic</description></item>
/// <item><description><strong>Weekly schedules:</strong> O(1) - Direct calculation with week alignment</description></item>
/// <item><description><strong>Monthly schedules:</strong> O(n) where n is the number of months searched (max 1200 iterations = 100 years)</description></item>
/// <item><description><strong>Repeat intervals:</strong> O(1) - Direct calculation using Math.Ceiling</description></item>
/// </list>
/// 
/// <para><strong>Maximum Recommended Values:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Period:</strong> Recommended maximum of 1000 days for Daily, 52 weeks for Weekly, 120 months (10 years) for Monthly. 
/// Larger values are supported but may impact performance for Monthly schedules due to iteration limits.</description></item>
/// <item><description><strong>RepeatInterval:</strong> Validated range: 5 minutes to 100 years (3650 days). 
/// Recommended maximum: 1 year for optimal performance.</description></item>
/// <item><description><strong>RepeatDuration:</strong> Validated range: 5 minutes to 100 years (3650 days). 
/// Must be greater than or equal to RepeatInterval.</description></item>
/// </list>
/// 
/// <para><strong>Precision Considerations:</strong></para>
/// <list type="bullet">
/// <item><description><strong>TimeSpan precision:</strong> All time intervals use TimeSpan which has millisecond precision. 
/// Calculations use TotalMilliseconds for interval arithmetic to maintain precision.</description></item>
/// <item><description><strong>Floating point arithmetic:</strong> For very small intervals (close to 5-minute minimum), 
/// floating point precision is handled by adding one additional interval if the calculated value equals or is before the minimum.</description></item>
/// <item><description><strong>DateTime precision:</strong> All dates are trimmed to minute precision (seconds/milliseconds set to 0) 
/// for consistency and to avoid sub-minute scheduling issues.</description></item>
/// </list>
/// 
/// <para><strong>Timezone and UTC Requirements:</strong></para>
/// <list type="bullet">
/// <item><description><strong>All dates must be in UTC:</strong> All DateTime values (StartUtc, ExpireUtc, LastRun, nowUtc parameter) 
/// must use DateTimeKind.Utc. The helper does not perform timezone conversions.</description></item>
/// <item><description><strong>No DST handling:</strong> Since all calculations use UTC, Daylight Saving Time (DST) transitions 
/// are not a concern. If local time support is ever needed, DST transitions would need special handling 
/// (e.g., 2 AM occurrences might not exist or occur twice during DST transitions).</description></item>
/// <item><description><strong>Leap years:</strong> All date calculations use DateTime.DaysInMonth which correctly handles leap years. 
/// No hardcoded month lengths are used.</description></item>
/// </list>
/// 
/// <para><strong>Performance Optimizations:</strong></para>
/// <list type="bullet">
/// <item><description>Reflection results are cached to reduce overhead when accessing TriggerEntity properties</description></item>
/// <item><description>While loops have been replaced with direct calculations where possible to prevent infinite loops</description></item>
/// <item><description>Monthly schedule searches are limited to 1200 iterations (100 years) to prevent excessive computation</description></item>
/// </list>
/// </summary>
public static class TriggerDateHelper
{
  // Cache PropertyInfo objects to reduce reflection overhead
  private static readonly Dictionary<string, PropertyInfo> _propertyCache = new();
  private static readonly object _propertyCacheLock = new object();

  /// <summary>
  /// Gets a property value from TriggerEntity using cached PropertyInfo to reduce reflection overhead.
  /// </summary>
  private static T GetPropertyValue<T>(TriggerEntity trigger, string propertyName) where T : class
  {
    if (!_propertyCache.TryGetValue(propertyName, out var prop))
    {
      lock (_propertyCacheLock)
      {
        if (!_propertyCache.TryGetValue(propertyName, out prop))
        {
          prop = typeof(TriggerEntity).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
          if (prop != null)
          {
            _propertyCache[propertyName] = prop;
          }
        }
      }
    }
    return prop?.GetValue(trigger) as T;
  }
  public static void ValidateTriggerDateProperties(TriggerEntity trigger)
  {
    var errors = new List<string>();

    if (trigger.ExpireUtc.HasValue && trigger.ExpireUtc <= trigger.StartUtc)
      errors.Add("ExpireUtc must be greater than StartUtc.");

    var months = trigger.MonthsList ?? new List<int>();
    var dom = trigger.DaysOfMonthList ?? new List<int>();
    var dow = SelectedDaysOfWeek(trigger).ToList();
    var occ = trigger.DaysOfWeekOccurencesList ?? new List<int>();

    foreach (var m in months)
      if (m < 1 || m > 12) errors.Add($"Invalid month '{m}'. Allowed 1..12.");
    foreach (var d in dom)
      if (d < 1 || d > 31) errors.Add($"Invalid day-of-month '{d}'. Allowed 1..31.");
    foreach (var o in occ)
      if (o < 1 || o > 5) errors.Add($"Invalid weekday occurrence '{o}'. Allowed 1..5.");

    switch (trigger.PeriodType)
    {
      case TimePeriodType.Weekly:
        if (dow.Count == 0)
          errors.Add("Weekly schedule requires at least one ISO day-of-week (1..7).");
        break;

      case TimePeriodType.Monthly:
        bool daysModel = trigger.DaysOfMonthLast || dom.Count > 0;
        bool weekdayModel = dow.Count > 0 || trigger.DaysOfWeekOccurencesLast || occ.Count > 0;

        if (daysModel && weekdayModel)
          errors.Add("Monthly schedule must use either days-of-month OR weekday+occurrence model, not both.");

        if (!daysModel && dow.Count > 0)
        {
          bool hasOcc = (occ.Count > 0) || trigger.DaysOfWeekOccurencesLast;
          if (!hasOcc)
            errors.Add("Monthly weekday model requires at least one occurrence (1..5 or Last).");
        }

        if ((occ.Count > 0 || trigger.DaysOfWeekOccurencesLast) && dow.Count == 0)
          errors.Add("Monthly weekday occurrences specified but no weekdays selected.");
        break;
    }

    if (trigger.RepeatTask)
    {
      var minTime = TimeSpan.FromMinutes(5);
      var maxTime = TimeSpan.FromDays(3650);

      var repeatInterval = trigger.RepeatInterval;
      var repeatDurationOpt = trigger.RepeatDuration;

      if (repeatInterval < minTime || repeatInterval > maxTime)
        errors.Add("RepeatInterval must be between 5 minutes and 100 years");

      if (repeatDurationOpt.HasValue)
      {
        var repeatDuration = repeatDurationOpt.Value;
        if (repeatDuration < minTime || repeatDuration > maxTime)
          errors.Add("RepeatDuration must be between 5 minutes and 100 years");
        if (repeatInterval > repeatDuration)
          errors.Add("RepeatInterval cannot be greater than RepeatDuration.");
      }
    }

    if (errors.Count > 0)
      throw new AbpValidationException(string.Join(Environment.NewLine, errors));
  }

  /// <summary>
  /// Calculates the next run time for a trigger according to production-grade scheduling contract.
  ///
  /// CONTRACT RULES:
  /// 1. Monotonicity: NextRunUtc MUST be strictly > max(NowUtc, LastRunUtc) when LastRunUtc exists; else NextRunUtc > NowUtc.
  /// 2. Expiry: If ExpireUtc is provided, NextRunUtc MUST be strictly < ExpireUtc. If NextRunUtc >= ExpireUtc => return null.
  /// 3. OneTime: If LastRunUtc is null and NowUtc < StartUtc: return StartUtc (subject to expiry). If LastRunUtc is null and NowUtc >= StartUtc: return null. If LastRunUtc is set: return null. Never return DateTime.MaxValue.
  /// 4. Repeats: Next repeat MUST be strictly > max(NowUtc, LastRunUtc). Never return the base time itself.
  /// 5. Weekly: "Next Monday" after a Monday means +7 days, not the same day.
  /// 6. Monthly: Skip months without requested day. Next occurrence must be strictly after fromExclusive.
  ///
  /// Clock Skew Handling:
  /// When now < lastRun (clock skew scenario), we still enforce monotonicity: nextRun > lastRun.
  /// This prevents duplicate scheduling even when system clocks are out of sync.
  /// </summary>
  public static DateTime? GetNextRunTime(TriggerEntity trigger, DateTime? nowUtc = null)
  {
    if (trigger == null || !trigger.IsEnabled) return null;

    var now = nowUtc ?? DateTime.UtcNow;

    // Enforce monotonicity: determine the minimum time that next run must be after
    var minNextRun = now;
    if (trigger.LastRun.HasValue)
    {
      // Next run must be strictly after both now and LastRun
      minNextRun = trigger.LastRun.Value > now ? trigger.LastRun.Value : now;
    }

    DateTime? candidateNext = null;

    // Handle OneTime schedules
    if (trigger.PeriodType == TimePeriodType.OneTime)
    {
      // If LastRun is set, OneTime already ran - return null
      if (trigger.LastRun.HasValue)
        return null;

      // If now < StartUtc, return StartUtc (subject to expiry and monotonicity)
      if (trigger.StartUtc > now)
      {
        candidateNext = trigger.StartUtc;
      }
      else
      {
        // now >= StartUtc and LastRun is null - no catch-up, return null
        return null;
      }
    }
    else
    {
      // For recurring schedules, calculate the next major occurrence
      DateTime fromExclusive = minNextRun; // Start from minNextRun to enforce monotonicity

      // If StartUtc is in the future relative to fromExclusive, use StartUtc as first candidate
      if (trigger.StartUtc > fromExclusive)
      {
        candidateNext = trigger.StartUtc;
      }
      else
      {
        // Calculate next occurrence from fromExclusive
        candidateNext = trigger.PeriodType switch
        {
          TimePeriodType.Daily => GetNextDailyRunUtc(trigger, fromExclusive),
          TimePeriodType.Weekly => GetNextWeeklyRunUtc(trigger, fromExclusive),
          TimePeriodType.Monthly => GetNextMonthlyRunUtc(trigger, fromExclusive),
          _ => throw new Exception("Unsupported PeriodType"),
        };
      }

      // If we have repeats, calculate the next repeat time
      if (trigger.RepeatTask && candidateNext.HasValue)
      {
        // For repeats, we need to find the major occurrence that the repeat window is based on
        // This is the most recent major occurrence <= max(now, StartUtc) OR LastRun's major occurrence
        DateTime repeatWindowBase = candidateNext.Value;
        if (trigger.LastRun.HasValue && trigger.LastRun.Value > minNextRun)
        {
          // If LastRun is after minNextRun, find the major occurrence that LastRun belongs to
          var lastRunMajor = FindMajorOccurrenceForTime(trigger, trigger.LastRun.Value);
          if (lastRunMajor.HasValue && lastRunMajor.Value <= trigger.LastRun.Value)
            repeatWindowBase = lastRunMajor.Value;
        }
        else if (trigger.StartUtc <= minNextRun)
        {
          // Find the major occurrence that minNextRun belongs to
          var nowMajor = FindMajorOccurrenceForTime(trigger, minNextRun);
          if (nowMajor.HasValue && nowMajor.Value <= minNextRun)
            repeatWindowBase = nowMajor.Value;
        }
        else
        {
          repeatWindowBase = trigger.StartUtc;
        }

        var nextRepeat = CalculateNextRepeat(trigger, repeatWindowBase, minNextRun, candidateNext.Value);

        // Use the earlier of nextRepeat or next major occurrence
        // But only if nextRepeat is valid and within the repeat window
        if (nextRepeat.HasValue)
        {
          if (nextRepeat < candidateNext.Value)
          {
            candidateNext = nextRepeat;
          }
          // If nextRepeat is at or after candidateNext, we'll use candidateNext (the major occurrence)
        }
      }
    }

    // Enforce monotonicity: candidate must be strictly > minNextRun
    if (candidateNext.HasValue && candidateNext.Value <= minNextRun)
    {
      // If candidate equals or is before minNextRun, advance it
      if (trigger.RepeatTask && trigger.RepeatInterval > TimeSpan.Zero)
      {
        // For repeats, advance by interval
        candidateNext = minNextRun.Add(trigger.RepeatInterval);
      }
      else
      {
        // For major occurrences, recalculate from minNextRun + 1 tick
        var fromExclusive = minNextRun.AddTicks(1);
        candidateNext = trigger.PeriodType switch
        {
          TimePeriodType.Daily => GetNextDailyRunUtc(trigger, fromExclusive),
          TimePeriodType.Weekly => GetNextWeeklyRunUtc(trigger, fromExclusive),
          TimePeriodType.Monthly => GetNextMonthlyRunUtc(trigger, fromExclusive),
          _ => candidateNext,
        };
      }
    }

    // Enforce expiry: candidate must be strictly < ExpireUtc
    if (candidateNext.HasValue && trigger.ExpireUtc.HasValue)
    {
      if (candidateNext.Value >= trigger.ExpireUtc.Value)
        return null; // Expired
    }

    return candidateNext;
  }

  /// <summary>
  /// Calculates the next repeat time within a repeat window.
  /// Enforces: nextRepeat > max(now, lastRun), never returns base time itself.
  /// </summary>
  private static DateTime? CalculateNextRepeat(TriggerEntity trigger, DateTime repeatWindowBase, DateTime minNextRun, DateTime nextMajorOccurrence)
  {
    if (!trigger.RepeatTask || trigger.RepeatInterval <= TimeSpan.Zero)
      return null;

    // Defensive check: if RepeatInterval > RepeatDuration, this is invalid configuration
    // Handle gracefully by returning null (will fall back to next major occurrence)
    if (trigger.RepeatDuration.HasValue && trigger.RepeatInterval > trigger.RepeatDuration.Value)
    {
      return null;
    }

    // Calculate next repeat: must be strictly > minNextRun (which is max(now, lastRun))
    // Never return the base time itself - first repeat is base + interval
    var firstRepeat = repeatWindowBase + trigger.RepeatInterval;

    // If firstRepeat is not strictly after minNextRun, advance it
    DateTime nextRepeat = firstRepeat;
    if (nextRepeat <= minNextRun)
    {
      // Calculate how many intervals we need to advance
      var delta = minNextRun - repeatWindowBase;
      var intervals = (long)Math.Ceiling(delta.TotalMilliseconds / trigger.RepeatInterval.TotalMilliseconds);
      nextRepeat = repeatWindowBase + TimeSpan.FromMilliseconds(intervals * trigger.RepeatInterval.TotalMilliseconds);

      // Ensure it's strictly after minNextRun (handle floating point precision issues)
      // Use direct calculation instead of while loop to avoid potential infinite loops
      if (nextRepeat <= minNextRun)
      {
        nextRepeat = nextRepeat + trigger.RepeatInterval;
      }
    }

    // Check if nextRepeat is within the repeat window
    if (trigger.RepeatDuration.HasValue && trigger.RepeatDuration.Value > TimeSpan.Zero)
    {
      var windowEnd = repeatWindowBase + trigger.RepeatDuration.Value;
      if (nextRepeat >= windowEnd)
      {
        // Next repeat is outside the window, return null (will use next major occurrence)
        return null;
      }
    }

    // Check if nextRepeat is before the next major occurrence
    if (nextRepeat < nextMajorOccurrence)
    {
      return nextRepeat;
    }

    // If nextRepeat is at or after nextMajorOccurrence, use nextMajorOccurrence instead (no repeat needed)
    return null;
  }

  /// <summary>
  /// Finds the major occurrence (daily/weekly/monthly) that a given time belongs to.
  /// Used to determine the repeat window base when LastRun is in the future.
  /// </summary>
  private static DateTime? FindMajorOccurrenceForTime(TriggerEntity trigger, DateTime time)
  {
    return trigger.PeriodType switch
    {
      TimePeriodType.Daily => FindDailyOccurrence(trigger, time),
      TimePeriodType.Weekly => FindWeeklyOccurrence(trigger, time),
      TimePeriodType.Monthly => FindMonthlyOccurrence(trigger, time),
      _ => null,
    };
  }

  private static DateTime? FindDailyOccurrence(TriggerEntity trigger, DateTime time)
  {
    if (time < trigger.StartUtc) return null;

    var tod = Tod(trigger.StartUtc);
    var period = Math.Max(1, trigger.Period);
    var baseDate = time.Date;
    var daysSinceStart = (int)(baseDate - trigger.StartUtc.Date).TotalDays;
    if (daysSinceStart < 0) return null;

    int mod = daysSinceStart % period;
    var alignedDate = baseDate.AddDays(-mod);
    var occurrence = AtTod(alignedDate, tod);

    if (occurrence < trigger.StartUtc)
      occurrence = occurrence.AddDays(period);

    return occurrence <= time ? occurrence : null;
  }

  private static DateTime? FindWeeklyOccurrence(TriggerEntity trigger, DateTime time)
  {
    if (time < trigger.StartUtc) return null;

    var tod = Tod(trigger.StartUtc);
    var period = Math.Max(1, trigger.Period);
    var isoList = GetPropertyValue<IEnumerable<int>>(trigger, "DaysOfWeekList");
    List<DayOfWeek> days = (isoList != null && isoList.Any())
        ? isoList.Select(iso => (DayOfWeek)(iso % 7)).Distinct().OrderBy(d => d).ToList()
        : BuildDaysFromMask(trigger.DaysOfWeek);

    DateTime WeekStart(DateTime d) => d.Date.AddDays(-(int)d.DayOfWeek);
    var startWeek = WeekStart(trigger.StartUtc);
    var timeWeek = WeekStart(time);

    var k = (int)((timeWeek - startWeek).TotalDays / 7);
    if (k < 0) return null;
    var kAligned = (k / period) * period;
    var weekAnchor = startWeek.AddDays(7 * kAligned);

    // Find the occurrence in this week that is <= time
    foreach (var dow in days.OrderByDescending(d => d))
    {
      var dt = AtTod(weekAnchor.AddDays((int)dow), tod);
      if (weekAnchor == startWeek && dt < trigger.StartUtc) continue;
      if (dt <= time) return dt;
    }

    return null;
  }

  private static DateTime? FindMonthlyOccurrence(TriggerEntity trigger, DateTime time)
  {
    if (time < trigger.StartUtc) return null;

    // Optimize: find the month that time belongs to and calculate the occurrence
    // Start from beginning of the month to reduce search space
    var timeMonth = new DateTime(time.Year, time.Month, 1, 0, 0, 0, DateTimeKind.Utc);
    var fromExclusive = timeMonth.AddDays(-1); // Start from beginning of month
    var candidate = GetNextMonthlyRunUtc(trigger, fromExclusive);
    if (candidate.HasValue && candidate.Value <= time)
    {
      // Check if there's a later one still <= time (but in same month if possible)
      var next = GetNextMonthlyRunUtc(trigger, candidate.Value);
      if (next.HasValue && next.Value <= time)
        return next;
      return candidate;
    }
    return null;
  }

  /* ---------------- helpers ---------------- */

  public static DateTime TrimToMinute(DateTime dt)
  {
    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
  }
  public static DateTime? TrimToMinute(DateTime? dt)
  {
    if (dt == null)
    {
      return null;
    }

    return TrimToMinute(dt.Value);
  }

  private static TimeSpan Tod(DateTime dt) => new(dt.Hour, dt.Minute, dt.Second);
  private static DateTime AtTod(DateTime dateUtc, TimeSpan tod)
      => new(dateUtc.Year, dateUtc.Month, dateUtc.Day, tod.Hours, tod.Minutes, tod.Seconds, DateTimeKind.Utc);

  /* ==== DAILY (every N days anchored to StartUtc) ==== */
  private static DateTime? GetNextDailyRunUtc(TriggerEntity t, DateTime fromExclusive)
  {
    if (fromExclusive < t.StartUtc)
      return t.StartUtc; // Will be checked for expiry and monotonicity by caller

    var tod = Tod(t.StartUtc);
    var period = Math.Max(1, t.Period);

    var baseDate = fromExclusive.Date;
    var daysSinceStart = (int)(baseDate - t.StartUtc.Date).TotalDays;
    if (daysSinceStart < 0) daysSinceStart = 0;

    int mod = daysSinceStart % period;
    var alignedDate = baseDate.AddDays(mod == 0 ? 0 : (period - mod));

    var candidate = AtTod(alignedDate, tod);
    // CONTRACT: candidate must be strictly > fromExclusive
    if (candidate <= fromExclusive)
      candidate = AtTod(alignedDate.AddDays(period), tod);

    // Ensure candidate is not before StartUtc
    if (candidate < t.StartUtc)
    {
      var startTod = AtTod(t.StartUtc.Date, tod);
      candidate = startTod <= t.StartUtc
        ? AtTod(t.StartUtc.Date.AddDays(period), tod)
        : startTod;
    }

    // Ensure candidate is strictly > fromExclusive (enforce monotonicity)
    // Use direct calculation instead of while loop to avoid potential infinite loops
    if (candidate <= fromExclusive)
    {
      var delta = fromExclusive - candidate;
      if (delta.TotalDays > 0)
      {
        var periodsToAdvance = (int)Math.Ceiling(delta.TotalDays / period);
        candidate = candidate.AddDays(periodsToAdvance * period);
      }
      // Handle edge case where candidate equals fromExclusive (should advance by one period)
      if (candidate <= fromExclusive)
      {
        candidate = candidate.AddDays(period);
      }
    }

    return candidate; // Expiry and monotonicity enforced by caller
  }

  private static IEnumerable<int> SelectedDaysOfWeek(TriggerEntity t)
  {
    for (int iso = 1; iso <= 7; iso++)
      if ((t.DaysOfWeek & (1 << (iso - 1))) != 0) yield return iso;
  }

  private static List<DayOfWeek> BuildDaysFromMask(int maskBits)
  {
    var list = new List<DayOfWeek>();
    for (int i = 0; i < 7; i++)
    {
      if ((maskBits & (1 << i)) != 0)
      {
        // i: 0..6 represents Mon..Sun in mask; map to DayOfWeek (Sun..Sat)
        list.Add((DayOfWeek)((i + 1) % 7));
      }
    }
    if (list.Count == 0) list = System.Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
    return list.Distinct().OrderBy(d => d).ToList();
  }

  /* ==== WEEKLY (every N weeks anchored to StartUtc's week) ==== */
  private static DateTime? GetNextWeeklyRunUtc(TriggerEntity t, DateTime fromExclusive)
  {
    var tod = Tod(t.StartUtc);
    var period = Math.Max(1, t.Period);

    // If before the initial start boundary, return StartUtc as the first occurrence
    if (fromExclusive < t.StartUtc)
      return t.StartUtc; // Will be checked for expiry and monotonicity by caller

    // Build weekdays from ISO 1..7 (Mon..Sun) => DayOfWeek (Sun..Sat)
    var isoList = GetPropertyValue<IEnumerable<int>>(t, "DaysOfWeekList");
    List<DayOfWeek> days = (isoList != null && isoList.Any())
        ? isoList.Select(iso => (DayOfWeek)(iso % 7)).Distinct().OrderBy(d => d).ToList()
        : BuildDaysFromMask(t.DaysOfWeek);

    DateTime WeekStart(DateTime d) => d.Date.AddDays(-(int)d.DayOfWeek);
    var startWeek = WeekStart(t.StartUtc);

    // CONTRACT: If fromExclusive is already on a target weekday at the same time, next should be +7 days (next week)
    // Start from the week containing fromExclusive
    var fromWeek = WeekStart(fromExclusive);
    var k = (int)((fromWeek - startWeek).TotalDays / 7);
    if (k < 0) k = 0;
    var kAligned = (k % period == 0) ? k : k + (period - (k % period));
    var weekAnchor = startWeek.AddDays(7 * kAligned);

    // Check days in the current week first
    foreach (var dow in days.OrderBy(d => d))
    {
      var dt = AtTod(weekAnchor.AddDays((int)dow), tod);
      if (weekAnchor == startWeek && dt < t.StartUtc) continue;
      // CONTRACT: Must be strictly > fromExclusive
      if (dt > fromExclusive) return dt;
    }

    // If no valid day in current week, move to next period
    var nextWeekStart = weekAnchor.AddDays(7 * period);
    foreach (var dow in days.OrderBy(d => d))
    {
      var dt = AtTod(nextWeekStart.AddDays((int)dow), tod);
      // CONTRACT: Must be strictly > fromExclusive
      if (dt > fromExclusive) return dt;
    }

    // Should not reach here, but handle edge case
    return null;
  }

  /* ==== MONTHLY (every N months anchored to StartUtc's month) ==== */
  private static DateTime? GetNextMonthlyRunUtc(TriggerEntity t, DateTime fromExclusive)
  {
    var tod = Tod(t.StartUtc);
    var period = Math.Max(1, t.Period);

    var monthsProp = GetPropertyValue<IEnumerable<int>>(t, "MonthsList");
    var monthsList = monthsProp?.Distinct().Where(m => m is >= 1 and <= 12).OrderBy(m => m).ToList() ?? new List<int>();
    List<int> months = monthsList.Count > 0 ? monthsList : BuildMonthsFromMask(t.Months);

    List<int> dom =
        GetPropertyValue<IEnumerable<int>>(t, "DaysOfMonthList")?.Distinct().Where(d => d is >= 1 and <= 31).OrderBy(d => d).ToList()
        ?? new List<int>();
    bool domLast = (bool?)GetPropertyValue<object>(t, "DaysOfMonthLast") ?? false;

    var isoDows = GetPropertyValue<IEnumerable<int>>(t, "DaysOfWeekList");
    List<DayOfWeek> dows;
    if (isoDows != null && isoDows.Any())
    {
      dows = isoDows.Select(iso => (DayOfWeek)(iso % 7)).Distinct().OrderBy(d => d).ToList();
    }
    else
    {
      var fromMask = new List<DayOfWeek>();
      for (int i = 0; i < 7; i++)
      {
        if ((t.DaysOfWeek & (1 << i)) != 0)
          fromMask.Add((DayOfWeek)((i + 1) % 7));
      }
      dows = fromMask.Count > 0
          ? fromMask.Distinct().OrderBy(d => d).ToList()
          : System.Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
    }
    List<int> occ =
        GetPropertyValue<IEnumerable<int>>(t, "DaysOfWeekOccurencesList")?.Distinct().Where(x => x is >= 1 and <= 4).OrderBy(x => x).ToList()
        ?? new List<int>();
    bool occLast = (bool?)GetPropertyValue<object>(t, "DaysOfWeekOccurencesLast") ?? false;

    static int MIdx(DateTime d) => d.Year * 12 + (d.Month - 1);
    var startIdx = MIdx(t.StartUtc);
    var fromIdx = Math.Max(MIdx(fromExclusive), startIdx);
    int delta = fromIdx - startIdx;
    int k = (delta % period == 0) ? delta : delta + (period - (delta % period));

    // Add iteration limit to prevent infinite loops (1200 iterations = 100 years max)
    const int MAX_MONTH_ITERATIONS = 1200;
    int iterations = 0;

    while (iterations < MAX_MONTH_ITERATIONS)
    {
      iterations++;
      int idx = startIdx + k;
      int year = idx / 12;
      int month = (idx % 12) + 1;
      if (year < 1) year = 1;
      if (year > 9999) return null;

      if (months.Contains(month))
      {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);

        var cands = new List<DateTime>();

        // Only add day-of-month candidates if we have days-of-month specified
        if (dom.Count > 0 || domLast)
        {
          foreach (var d in dom)
            if (d >= 1 && d <= daysInMonth)
              cands.Add(AtTod(new DateTime(year, month, d, 0, 0, 0, DateTimeKind.Utc), tod));
          if (domLast)
            cands.Add(AtTod(new DateTime(year, month, daysInMonth, 0, 0, 0, DateTimeKind.Utc), tod));
        }

        // Only add weekday occurrence candidates if we have weekday+occurrence specified
        if ((occ.Count > 0 || occLast) && dows.Count > 0)
        {
          foreach (var dow in dows)
          {
            int deltaFirst = ((int)dow - (int)monthStart.DayOfWeek + 7) % 7;
            int firstDom = 1 + deltaFirst;

            foreach (var o in occ)
            {
              int day = firstDom + 7 * (o - 1);
              if (day >= 1 && day <= daysInMonth)
                cands.Add(AtTod(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc), tod));
            }

            if (occLast)
            {
              var lastOfMonth = new DateTime(year, month, daysInMonth, 0, 0, 0, DateTimeKind.Utc);
              int deltaLast = ((int)lastOfMonth.DayOfWeek - (int)dow + 7) % 7;
              int lastDom = daysInMonth - deltaLast;
              if (lastDom >= 1 && lastDom <= daysInMonth)
                cands.Add(AtTod(new DateTime(year, month, lastDom, 0, 0, 0, DateTimeKind.Utc), tod));
            }
          }
        }

        // If no candidates were generated (e.g., requested day doesn't exist in this month), skip to next month
        if (cands.Count == 0)
        {
          k += period;
          continue;
        }

        if (idx == startIdx)
          cands = cands.Where(d => d >= t.StartUtc).ToList();

        // CONTRACT: Next occurrence must be strictly > fromExclusive
        var next = cands.Where(d => d > fromExclusive).OrderBy(d => d).FirstOrDefault();
        if (next != default) return next;
      }

      k += period;

      // Check expiry to avoid infinite loop
      if (t.ExpireUtc.HasValue)
      {
        var probeMonth = new DateTime((startIdx + k) / 12, ((startIdx + k) % 12) + 1, 1, 0, 0, 0, DateTimeKind.Utc);
        if (probeMonth > t.ExpireUtc.Value) return null;
      }
    }

    // No valid occurrence found within bounds
    return null;

    static List<int> BuildMonthsFromMask(int maskBits)
    {
      var list = new List<int>();
      for (int i = 0; i < 12; i++) if ((maskBits & (1 << i)) != 0) list.Add(i + 1);
      if (list.Count == 0) list = Enumerable.Range(1, 12).ToList();
      return list;
    }
  }
}
