using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.DocMessageLog.Module.Entities;

namespace VPortal.DocMessageLog.Module.Options;
public class SystemLogUserStateOptions
{
  public Guid? LastAckNotificationLog { get; set; }

  public DateTime? LastAckDate { get; set; }
  public void Acknowledge(SystemLogEntity log)
  {
    LastAckNotificationLog = log.Id;
    LastAckDate = log.LastModificationTime;
  }
}

