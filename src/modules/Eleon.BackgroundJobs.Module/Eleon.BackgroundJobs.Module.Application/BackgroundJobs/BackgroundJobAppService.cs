using Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Commons.Module.Constants.BackgroundJobs;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using VPortal.BackgroundJobs.Module;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Permissions;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobAppService : ModuleAppService, IBackgroundJobAppService
  {
    private readonly IVportalLogger<BackgroundJobAppService> logger;
    private readonly IBackgroundJobDomainService domainService;
    private readonly IDistributedEventBus eventBus;

    public BackgroundJobAppService(
        IVportalLogger<BackgroundJobAppService> logger,
        IBackgroundJobDomainService domainService,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.domainService = domainService;
      this.eventBus = eventBus;
    }

    [Authorize]
    public async Task<BackgroundJobDto> CreateAsync(CreateBackgroundJobDto input)
    {
      BackgroundJobDto response = null;

      try
      {
        var keyName = CurrentUser.GetApiKeyName();

        var sourceType = string.IsNullOrEmpty(keyName) ? BackgroundJobConstants.SourceType.User : BackgroundJobConstants.SourceType.Api;
        var sourceId = string.IsNullOrEmpty(keyName) ? CurrentUser.UserName : keyName;

        var entity = await domainService.CreateAsync(
            CurrentTenant.Id,
            GuidGenerator.Create(),
            input.ParentJobsIds?.FirstOrDefault(),
            input.Type,
            input.Initiator,
            input.IsRetryAllowed,
            input.Description,
            input.StartExecutionParams,
            input.ScheduleExecutionDateUtc,
            false,
            startExecutionExtraParams: input.StartExecutionExtraParams,
            sourceId,
            sourceType,
            input.TimeoutInMinutes,
            input.MaxRetryAttempts,
            input.RetryInMinutes,
            input.OnFailureRecepients);
        response = ObjectMapper.Map<BackgroundJobEntity, BackgroundJobDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return response;
    }

    [Authorize]
    public async Task<BackgroundJobDto> CompleteAsync(BackgroundJobExecutionCompleteDto input)
    {
      BackgroundJobDto response = null;

      try
      {
        await eventBus.PublishAsync(new BackgroundJobExecutionCompletedMsg
        {
          BackgroundJobId = input.BackgroundJobId,
          Type = input.Type,
          ExecutionId = input.ExecutionId,
          ParamsForRetryExecution = input.ParamsForRetryExecution,
          ExtraParamsForRetryExecution = input.ExtraParamsForRetryExecution,
          Status = input.Status,
          Messages = input.Messages?.Select(m => new BackgroundJobTextInfoEto
          {
            Type = m.MessageType,
            TextMessage = m.TextMessage
          }).ToList(),
          Result = input.Result,
          TenantId = CurrentTenant.Id,
          TenantName = CurrentTenant.Name,
          CompletedBy = CurrentUser.UserName ?? CurrentUser.GetName(),
          IsManually = true
        });
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return response;
    }

    [Authorize(ModulePermissions.General)]
    public async Task<FullBackgroundJobDto> GetBackgroundJobByIdAsync(Guid id)
    {
      FullBackgroundJobDto response = null;

      try
      {
        var gotEntity = await domainService.GetAsync(id);
        response = ObjectMapper.Map<BackgroundJobEntity, FullBackgroundJobDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    [Authorize(ModulePermissions.General)]
    public async Task<PagedResultDto<BackgroundJobHeaderDto>> GetBackgroundJobListAsync(BackgroundJobListRequestDto input)
    {
      PagedResultDto<BackgroundJobHeaderDto> result = null;
      try
      {
        var pair = await domainService
            .GetBackgroundJobsList(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.SearchQuery,
                input.CreationDateFilterStart,
                input.CreationDateFilterEnd,
                input.LastExecutionDateFilterStart,
                input.LastExecutionDateFilterEnd,
                input.TypeFilter,
                input.StatusFilter);
        var dtos = ObjectMapper
            .Map<List<BackgroundJobEntity>, List<BackgroundJobHeaderDto>>(pair.Value);
        result = new PagedResultDto<BackgroundJobHeaderDto>(pair.Key, dtos);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> RetryBackgroundJobAsync(Guid jobId)
    {
      bool response = false;

      try
      {
        response = await domainService.RetryJob(jobId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return response;
    }

    public async Task<bool> CancelBackgroundJobAsync(Guid id)
    {
      try
      {
        await eventBus.PublishAsync(new CancelBackgroundJobMsg
        {
          JobId = id,
          TenantId = CurrentTenant.Id,
          TenantName = CurrentTenant.Name,
          CancelledBy = CurrentUser.GetName(),
          IsManually = true,
          CancelledMessage = $"Cancelled by user with user name: {CurrentUser.UserName} and user id: {CurrentUser.Id}."
        });
        return true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        return false;
      }
      finally
      {
      }
    }

    public async Task<bool> MarkExecutionStartedAsync(Guid jobId, Guid executionId)
    {
      try
      {
        await eventBus.PublishAsync(new MarkJobExecutionStartedMsg
        {
          JobId = jobId,
          ExecutionId = executionId,
          TenantId = CurrentTenant.Id,
          TenantName = CurrentTenant.Name
        });

        return true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }
  }
}
