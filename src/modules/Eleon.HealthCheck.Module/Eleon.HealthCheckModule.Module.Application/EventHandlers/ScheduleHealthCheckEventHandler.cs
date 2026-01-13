using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.DomainServices;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Repositories;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.EventHandlers;
public class ScheduleHealthCheckEventHandler : IDistributedEventHandler<ScheduleMsg>, ITransientDependency
{
  private readonly IVportalLogger<ScheduleHealthCheckEventHandler> _logger;
  private readonly HealthCheckDomainService _healthCheckDomainService;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly HealthCheckModuleOptions _options;
  private static SemaphoreSlim _semaphore = new(1, 1);

  public ScheduleHealthCheckEventHandler(
      IVportalLogger<ScheduleHealthCheckEventHandler> logger,
      HealthCheckDomainService healthCheckDomainService,
      IUnitOfWorkManager unitOfWorkManager,
      IOptions<HealthCheckModuleOptions> options)
  {
    _logger = logger;
    _healthCheckDomainService = healthCheckDomainService;
    _unitOfWorkManager = unitOfWorkManager;
    _options = options.Value;
  }
  public async Task HandleEventAsync(ScheduleMsg eventData)
  {
    if (!_options.Enabled)
    {
      return;
    }

    await _semaphore.WaitAsync();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true);

      var latestHealthCheckTime = (await _healthCheckDomainService.GetListAsync("CreationTime DESC", 0, 1, type: HealthCheckDefaults.HealthCheckTypes.Scheduled)).Value.FirstOrDefault()?.CreationTime;

      if (latestHealthCheckTime.HasValue && latestHealthCheckTime.Value + TimeSpan.FromMinutes(_options.IntervalMinutes) > DateTime.UtcNow)
      {
        return;
      }

      await _healthCheckDomainService.CreateAsync(HealthCheckDefaults.HealthCheckTypes.Scheduled, HealthCheckDefaults.Inititors.System, true);

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while handling ScheduleMsg event.");
      throw;
    }
    finally
    {
      _semaphore.Release();
    }
  }
}
