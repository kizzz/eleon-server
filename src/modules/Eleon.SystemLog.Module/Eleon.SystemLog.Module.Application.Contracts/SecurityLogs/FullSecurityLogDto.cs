using System;

namespace VPortal.Infrastructure.Module.SecurityLogs
{
  public class FullSecurityLogDto : SecurityLogDto
  {
    public new Dictionary<string, string> ExtraProperties { get; set; }
  }
}
