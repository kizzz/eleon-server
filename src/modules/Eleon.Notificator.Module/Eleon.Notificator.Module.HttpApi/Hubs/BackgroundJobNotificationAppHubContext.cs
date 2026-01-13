using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using VPortal.Notificator.Module.BackgroundJobs;
using VPortal.Notificator.Module.HubGroups;

namespace VPortal.Notificator.Module.Hubs
{
  [ExposeServices(typeof(IBackgroundJobNotificationClient))]
  public class BackgroundJobNotificationAppHubContext : IBackgroundJobNotificationClient
  {
    private readonly IVportalLogger<BackgroundJobNotificationAppHubContext> _logger;
    private readonly ICurrentTenant _currentTenant;
    private readonly IHubContext<BackgroundJobNotificationHub, IBackgroundJobNotificationClient> _hubContext;

    public BackgroundJobNotificationAppHubContext(
        IVportalLogger<BackgroundJobNotificationAppHubContext> logger,
        ICurrentTenant currentTenant,
        IHubContext<BackgroundJobNotificationHub, IBackgroundJobNotificationClient> hubContext)
    {
      _logger = logger;
      _currentTenant = currentTenant;
      _hubContext = hubContext;
    }

    public async Task AddJob(BackgroundJobEto job)
    {
      try
      {
        var group = _hubContext.Clients.Group(new TenantGroupId(_currentTenant).ToString());
        if (group != null)
        {
          await group.AddJob(job);
        }
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task UpdateJob(BackgroundJobEto job)
    {
      try
      {
        var group = _hubContext.Clients.Group(new TenantGroupId(_currentTenant).ToString());
        if (group != null)
        {
          await group.UpdateJob(job);
        }
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
