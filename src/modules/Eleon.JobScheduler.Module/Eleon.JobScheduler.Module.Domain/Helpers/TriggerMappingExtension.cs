using Common.Module.Constants;
using ModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.JobScheduler.Module.Entities;

namespace EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;
public static class TriggerMappingExtension
{
  public static void MapDisplayPropertiesToPersistent(this TriggerEntity triggerEntity)
  {
    // DaysOfWeek
    if (triggerEntity.DaysOfWeekList != null)
    {
      triggerEntity.DaysOfWeek = (int)DaysOfWeekMasks.None;
      foreach (var dayOfWeek in triggerEntity.DaysOfWeekList)
      {
        if (dayOfWeek >= 1 && dayOfWeek <= 7)
        {
          triggerEntity.DaysOfWeek |= 1 << (dayOfWeek - 1);
        }
      }
    }

    // DaysOfWeekOccurences
    if (triggerEntity.DaysOfWeekOccurencesList != null)
    {
      triggerEntity.DaysOfWeekOccurences = (int)DaysOfWeekOccurencesMasks.None;
      foreach (var dayOfWeekOccurence in triggerEntity.DaysOfWeekOccurencesList)
      {
        if (dayOfWeekOccurence >= 1 && dayOfWeekOccurence <= 4)
        {
          triggerEntity.DaysOfWeekOccurences |= 1 << (dayOfWeekOccurence - 1);
        }
      }

      if (triggerEntity.DaysOfWeekOccurencesLast)
      {
        triggerEntity.DaysOfWeekOccurences |= (int)DaysOfWeekOccurencesMasks.Last;
      }
    }

    // Months
    if (triggerEntity.MonthsList != null)
    {
      triggerEntity.Months = (int)MonthsMasks.None;
      foreach (var month in triggerEntity.MonthsList)
      {
        if (month >= 1 && month <= 12)
        {
          triggerEntity.Months |= 1 << (month - 1);
        }
      }
    }

    // DaysOfMonth
    if (triggerEntity.DaysOfMonthList != null)
    {
      triggerEntity.DaysOfMonth = (long)DaysOfMonthMasks.None;
      foreach (var dayOfMonth in triggerEntity.DaysOfMonthList)
      {
        if (dayOfMonth >= 1 && dayOfMonth <= 31)
        {
          triggerEntity.DaysOfMonth |= 1L << (dayOfMonth - 1);
        }
      }

      if (triggerEntity.DaysOfMonthLast)
      {
        triggerEntity.DaysOfMonth |= (long)DaysOfMonthMasks.Last;
      }
    }
  }

  public static void MapPersistentPropertiesToDisplayOnly(this TriggerEntity triggerEntity)
  {
    // DaysOfWeek
    triggerEntity.DaysOfWeekList = new List<int>();
    for (int i = 0; i < 7; i++)
    {
      int mask = 1 << i;
      if ((triggerEntity.DaysOfWeek & mask) > 0)
      {
        triggerEntity.DaysOfWeekList.Add(i + 1);
      }
    }

    // DaysOfWeekOccurences
    triggerEntity.DaysOfWeekOccurencesList = new List<int>();
    for (int i = 0; i < 4; i++)
    {
      int mask = 1 << i;
      if ((triggerEntity.DaysOfWeekOccurences & mask) > 0)
      {
        triggerEntity.DaysOfWeekOccurencesList.Add(i + 1);
      }
    }

    triggerEntity.DaysOfWeekOccurencesLast = (triggerEntity.DaysOfWeekOccurences & (int)DaysOfWeekOccurencesMasks.Last) > 0;

    // Months
    triggerEntity.MonthsList = new List<int>();
    for (int i = 0; i < 12; i++)
    {
      int mask = 1 << i;
      if ((triggerEntity.Months & mask) > 0)
      {
        triggerEntity.MonthsList.Add(i + 1);
      }
    }

    // DaysOfMonth
    triggerEntity.DaysOfMonthList = new List<int>();
    for (int i = 0; i < 31; i++)
    {
      long mask = 1 << i;
      if ((triggerEntity.DaysOfMonth & mask) > 0)
      {
        triggerEntity.DaysOfMonthList.Add(i + 1);
      }
    }

    triggerEntity.DaysOfMonthLast = (triggerEntity.DaysOfMonth & (long)DaysOfMonthMasks.Last) > 0;
  }
}
