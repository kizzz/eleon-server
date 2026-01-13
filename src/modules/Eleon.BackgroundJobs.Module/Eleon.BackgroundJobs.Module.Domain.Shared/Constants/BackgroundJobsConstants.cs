using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;
public class BackgroundJobsConstants
{
  public const string ModuleName = "BackgroundJobs";

  public const int MinJobFailedDelayInMinutes = 5;
  public const int MaxJobFailedDelayInMinutes = 24 * 60; // 24 hours

  public const string RecepientsSeparator = ";";
}
