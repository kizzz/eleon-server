using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [Audited]
  public class TriggerEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual string Name { get; set; }
    public virtual bool IsEnabled { get; set; }
    public virtual DateTime? LastRun { get; set; }
    public virtual DateTime? NextRunUtc { get; set; }
    public virtual DateTime StartUtc { get; set; }
    public virtual DateTime? ExpireUtc { get; set; }
    public virtual TimePeriodType PeriodType { get; set; }
    public virtual int Period { get; set; }
    public virtual int DaysOfWeek { get; set; }
    public virtual int DaysOfWeekOccurences { get; set; }
    public virtual long DaysOfMonth { get; set; }
    public virtual int Months { get; set; }
    public bool RepeatTask { get; set; } = false;

    public int RepeatIntervalUnits { get; set; }
    public TimeUnit RepeatIntervalUnitType { get; set; }
    public int? RepeatDurationUnits { get; set; }
    public TimeUnit RepeatDurationUnitType { get; set; }
    public TaskEntity Task { get; set; }
    public virtual Guid TaskId { get; set; }

    protected TriggerEntity() { }

    public TriggerEntity(Guid id)
    {
      this.Id = id;
    }

    #region DisplayProperties

    /// <summary>
    /// Gets or sets a list of integers representing DaysOfWeek.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public IList<int> DaysOfWeekList { get; set; }

    /// <summary>
    /// Gets or sets a list of integers representing DaysOfWeekOccurences.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public IList<int> DaysOfWeekOccurencesList { get; set; }

    /// <summary>
    /// Gets or sets a boolean DaysOfWeekOccurencesLast representing
    /// whether or not should be the last DaysOfWeek occurence be included in the schedule.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public bool DaysOfWeekOccurencesLast { get; set; }

    /// <summary>
    /// Gets or sets a list of integers representing DaysOfMonth.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public IList<int> DaysOfMonthList { get; set; }

    /// <summary>
    /// Gets or sets a boolean DaysOfMonthLast representing
    /// whether or not should be the last day of month be included in the schedule.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public bool DaysOfMonthLast { get; set; }

    /// <summary>
    /// Gets or sets a list of integers representing Months.
    /// It is a display-only property.
    /// </summary>
    [NotMapped]
    public IList<int> MonthsList { get; set; }

    [NotMapped]
    public virtual TimeSpan RepeatInterval => CustomTimeSpan(RepeatIntervalUnits, RepeatIntervalUnitType);
    [NotMapped]
    public virtual TimeSpan? RepeatDuration => RepeatDurationUnits.HasValue
        ? CustomTimeSpan(RepeatDurationUnits.Value, RepeatDurationUnitType)
        : null;
    #endregion
    private TimeSpan CustomTimeSpan(int units, TimeUnit unitType)
    {
      return unitType switch
      {
        TimeUnit.Minutes => TimeSpan.FromMinutes(units),
        TimeUnit.Hours => TimeSpan.FromHours(units),
        TimeUnit.Days => TimeSpan.FromDays(units),
        _ => throw new Exception("Non-existing TimeUnit type: " + unitType),
      };
    }
  }
}

