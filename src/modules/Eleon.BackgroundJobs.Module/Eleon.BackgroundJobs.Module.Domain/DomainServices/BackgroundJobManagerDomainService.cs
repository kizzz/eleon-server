using BackgroundJobs.Module.JobFiltering;
using Common.Module.Constants;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using VPortal.BackgroundJobs.Module.Repositories;
namespace VPortal.BackgroundJobs.Module.DomainServices;

public class BackgroundJobManagerDomainService : DomainService
{
  private readonly IVportalLogger<BackgroundJobManagerDomainService> logger;
  private readonly IBackgroundJobDomainService backgroundJobDomainService;
  private readonly IConfiguration configuration;
  private readonly MultiTenancyDomainService multiTenancyDomainService;
  private readonly IBackgroundJobsRepository _backgroundJobsRepository;

  public BackgroundJobManagerDomainService(
      IVportalLogger<BackgroundJobManagerDomainService> logger,
      IBackgroundJobDomainService backgroundJobDomainService,
      IConfiguration configuration,
      MultiTenancyDomainService multiTenancyDomainService,
      IBackgroundJobsRepository backgroundJobsRepository)
  {
    this.logger = logger;
    this.backgroundJobDomainService = backgroundJobDomainService;
    this.configuration = configuration;
    this.multiTenancyDomainService = multiTenancyDomainService;
    _backgroundJobsRepository = backgroundJobsRepository;
  }

  public async Task RunJobsByScheduledTimeAsync()
  {
    try
    {
      var jobFilter = new JobFilter(configuration);

      await multiTenancyDomainService.ForEachTenant(async (tenantId) =>
      {
        var retryJobs = await backgroundJobDomainService.GetRetryJobsAsync(); // retry jobs where LastExecution + RetryInterval <= now
        var filteredRetryJobs = jobFilter.Filter(retryJobs).ToList();
        foreach (var job in filteredRetryJobs)
        {
          try
          {
            await backgroundJobDomainService.StartExecutionAsync(job.Id, isManualRetry: false, autoRetry: true);
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(ex);
            // job.Status = BackgroundJobStatus.Errored;
            // todo update job and write error message to job messages
          }
        }

        var jobs = await backgroundJobDomainService.GetCurrentJobsAsync(); // new jobs where ScheduledTime <= now
        var filteredJobs = jobFilter.Filter(jobs).ToList();

        foreach (var job in filteredJobs)
        {
          try
          {
            await backgroundJobDomainService.StartExecutionAsync(job.Id);
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(ex);
            // job.Status = BackgroundJobStatus.Errored;

            // todo update job and write error message to job messages
            // TODO: Ask what to do with job errors before execution
          }
        }
      });
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }

  }

  public async Task CancelLongTimeJobsAsync()
  {
    try
    {
      await multiTenancyDomainService.ForEachTenant(async (tenantId) =>
      {
        var jobs = await _backgroundJobsRepository.GetLongTimeExecutingJobIdsAsync();

        foreach (var job in jobs)
        {
          try
          {
            await backgroundJobDomainService.CancelJobAsync(job.Id, BackgroundJobsConstants.ModuleName, false, $"Cancelled by timeout: {job.TimeoutInMinutes} minutes");
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(ex);
          }
        }
      });
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }
  }
}
