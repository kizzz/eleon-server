using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Repositories;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using HealthChecks.UI.Data;
using Logging.Module;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.DomainServices;
public class HealthCheckDomainService : DomainService
{
  private readonly IVportalLogger<HealthCheckDomainService> _logger;
  private readonly IHealthCheckRepository _healthCheckRepository;
  private readonly IHealthCheckReportRepository _healthCheckReportRepository;
  private readonly IDistributedEventBus _eventBus;
  private readonly MultiTenancyDomainService _multiTenancyDomainService;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly HealthCheckModuleOptions _options;
  private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _healthCheckLocks = new();

  public HealthCheckDomainService(
      IVportalLogger<HealthCheckDomainService> logger,
      IHealthCheckRepository healthCheckRepository,
      IHealthCheckReportRepository healthCheckReportRepository,
      IDistributedEventBus eventBus,
      MultiTenancyDomainService multiTenancyDomainService,
      IOptions<HealthCheckModuleOptions> options,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _logger = logger;
    _healthCheckRepository = healthCheckRepository;
    _healthCheckReportRepository = healthCheckReportRepository;
    _eventBus = eventBus;
    _multiTenancyDomainService = multiTenancyDomainService;
    _unitOfWorkManager = unitOfWorkManager;
    _options = options.Value;
  }

  private static SemaphoreSlim GetOrCreateSemaphore(Guid healthCheckId)
  {
    return _healthCheckLocks.GetOrAdd(healthCheckId, _ => new SemaphoreSlim(1, 1));
  }

  private static void CleanupSemaphore(Guid healthCheckId)
  {
    if (_healthCheckLocks.TryRemove(healthCheckId, out var semaphore))
    {
      semaphore?.Dispose();
    }
  }

  public async Task<HealthCheck> CreateAsync(string type, string initiatorName, bool notifyHealthCheckStarted)
  {

    try
    {
      var healthCheck = new HealthCheck(GuidGenerator.Create())
      {
        Type = type,
        InitiatorName = initiatorName,
        Status = HealthCheckStatus.OK,
        InProgress = true,
      };

      healthCheck = await _healthCheckRepository.InsertAsync(healthCheck);

      if (notifyHealthCheckStarted)
      {
        await _multiTenancyDomainService.ForEachTenant(async (tenantId) =>
        {
          using (CurrentTenant.Change(tenantId))
          {
            await _eventBus.PublishAsync(
                    new HealthCheckStartedMsg
                {
                  HealthCheckId = healthCheck.Id,
                  Type = type,
                  InitiatorName = initiatorName,
                  TenantId = tenantId,
                });
          }
        });
      }

      return healthCheck;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while creating HealthCheck.");
      throw;
    }
    finally
    {
    }
  }

  public async Task<HealthCheck> SendAsync(HealthCheck healthCheck)
  {

    var healthCheckId = healthCheck.Id != Guid.Empty ? healthCheck.Id : Guid.NewGuid();
    var semaphore = GetOrCreateSemaphore(healthCheckId);

    await semaphore.WaitAsync();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

      var existing = healthCheck.Id != Guid.Empty ? await _healthCheckRepository.FindAsync(healthCheck.Id) : null;

      if (existing == null)
      {
        existing = await _healthCheckRepository.InsertAsync(new HealthCheck(GuidGenerator.Create())
        {
          Type = healthCheck.Type,
          InitiatorName = healthCheck.InitiatorName,
          Status = HealthCheckStatus.OK,
          InProgress = false
        });
      }

      foreach (var rep in healthCheck.Reports)
      {
        await PrivateAddReportAsync(existing, rep.ServiceName, rep.ServiceVersion, rep.UpTime, rep.CheckName, rep.Status, rep.Message, rep.IsPublic, rep.ExtraInformation.ToList());
      }

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
      return existing;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, "An error occurred while creating HealthCheck.");
      throw;
    }
    finally
    {
      semaphore.Release();
    }
  }

  public async Task<HealthCheckReport> AddReportAsync(
      Guid healthCheckId,
      string serviceName,
      string serviceVersion,
      TimeSpan upTime,
      string checkName,
      HealthCheckStatus status,
      string message,
      bool isPublic,
      List<ReportExtraInformation> extraParams)
  {

    var semaphore = GetOrCreateSemaphore(healthCheckId);

    await semaphore.WaitAsync();
    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

      var healthCheck = await _healthCheckRepository.GetAsync(healthCheckId);

      var report = await PrivateAddReportAsync(healthCheck, serviceName, serviceVersion, upTime, checkName, status, message, isPublic, extraParams);

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
      return report;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, $"An error occurred while adding report to HealthCheck with ID: {healthCheckId}");
      throw;
    }
    finally
    {
      semaphore.Release();
    }
  }

  public async Task<HealthCheck> GetAsync(Guid id)
  {
    try
    {
      var healthCheck = await _healthCheckRepository.GetFullByIdAsync(id);
      return healthCheck;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, $"An error occurred while retrieving HealthCheck with ID: {id}");
      throw;
    }
    finally
    {
    }
  }

  public async Task<KeyValuePair<long, List<HealthCheck>>> GetListAsync(string sorting = null, int skipCount = 0, int maxResultCount = int.MaxValue, string? type = null, string initiator = null, DateTime? minTime = null, DateTime? maxTime = null)
  {
    try
    {
      var result = await _healthCheckRepository.GetListAsync(sorting, skipCount, maxResultCount, type, initiator, minTime, maxTime, includeDetails: false);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, $"An error occurred while retrieving HealthChecks list");
      throw;
    }
    finally
    {
    }
  }

  private async Task<HealthCheckReport> PrivateAddReportAsync(HealthCheck healthCheck,
      string serviceName,
      string serviceVersion,
      TimeSpan upTime,
      string checkName,
      HealthCheckStatus status,
      string message,
      bool isPublic,
      List<ReportExtraInformation> extraParams)
  {
    if (status == HealthCheckStatus.Failed)
    {
      healthCheck.Status = HealthCheckStatus.Failed;
    }

    var reportsCount = healthCheck.GetProperty($"{serviceName}ReportsCount", 0);
    healthCheck.SetProperty($"{serviceName}ReportsCount", reportsCount + 1);

    if (healthCheck.Type == HealthCheckDefaults.HealthCheckTypes.Scheduled)
    {
      healthCheck.SetProperty($"Service{serviceName}", "reported");

      var allServicesSendedReports = true;
      foreach (var requiredService in _options.RequiredServices)
      {
        if (healthCheck.GetProperty($"Service{requiredService}", null) == null)
        {
          allServicesSendedReports = false;
          break;
        }
      }

      if (allServicesSendedReports)
      {
        healthCheck.InProgress = false;
        await _healthCheckRepository.UpdateAsync(healthCheck);
      }
    }
    else if (healthCheck.InProgress)
    {
      healthCheck.InProgress = false;
      await _healthCheckRepository.UpdateAsync(healthCheck);
    }

    await _healthCheckRepository.UpdateAsync(healthCheck);

    var report = new HealthCheckReport(GuidGenerator.Create())
    {
      HealthCheckId = healthCheck.Id,
      ServiceName = serviceName,
      ServiceVersion = serviceVersion,
      UpTime = upTime,
      CheckName = checkName,
      Status = status,
      Message = message,
      IsPublic = isPublic,
      ExtraInformation = extraParams?.Select(x => new ReportExtraInformation(GuidGenerator.Create())
      {
        Key = x.Key,
        Value = x.Value,
        Severity = x.Severity,
        Type = x.Type ?? HealthCheckDefaults.ExtraInfoTypes.Simple,
      }).ToList() ?? new List<ReportExtraInformation>()
    };
    await _healthCheckReportRepository.InsertAsync(report);
    return report;
  }

  public async Task<KeyValuePair<long, List<HealthCheckReport>>> GetReportsAsync(Guid healthCheckId, string filter = null, string sorting = null, int skipCount = 0, int maxResultCount = int.MaxValue)
  {
    try
    {
      var reports = await _healthCheckRepository.GetReportsAsync(healthCheckId, filter, sorting, skipCount, maxResultCount);
      return reports;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex, $"An error occurred while retrieving reports for HealthCheck with ID: {healthCheckId}");
      throw;
    }
    finally
    {
    }
  }
}
