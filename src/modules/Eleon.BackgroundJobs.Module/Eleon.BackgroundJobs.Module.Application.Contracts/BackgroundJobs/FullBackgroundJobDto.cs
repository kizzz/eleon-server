using BackgroundJobs.Module.BackgroundJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Module.BackgroundJobs;
public class FullBackgroundJobDto : BackgroundJobDto
{
  public Dictionary<string, string> ExtraProperties { get; set; }
}
