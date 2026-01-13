using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.BackgroundJobs.Module.Entities;

namespace Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.Domain.Shared.DomainServices
{
  public interface IBackgroundJobHubContext
  {
    Task JobCompleted(BackgroundJobEntity job);
  }
}
