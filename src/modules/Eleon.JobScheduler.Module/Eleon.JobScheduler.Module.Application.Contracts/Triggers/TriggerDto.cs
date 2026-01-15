using Common.Module.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.JobScheduler.Module.Triggers
{
  public class TriggerDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime? NextRunUtc { get; set; }
    public DateTime? ExpireUtc { get; set; }
    public TimePeriodType PeriodType { get; set; }
    public DateTime? LastRun { get; set; }
    public int Period { get; set; }
    public bool RepeatTask { get; set; }
    public int RepeatIntervalUnits { get; set; }
    public TimeUnit RepeatIntervalUnitType { get; set; }
    public int? RepeatDurationUnits { get; set; }
    public TimeUnit RepeatDurationUnitType { get; set; }
    public IList<int> DaysOfWeekList { get; set; }
    public IList<int> DaysOfWeekOccurencesList { get; set; }
    public bool DaysOfWeekOccurencesLast { get; set; }
    public IList<int> DaysOfMonthList { get; set; }
    public bool DaysOfMonthLast { get; set; }
    public IList<int> MonthsList { get; set; }
    public Guid TaskId { get; set; }
  }
}
