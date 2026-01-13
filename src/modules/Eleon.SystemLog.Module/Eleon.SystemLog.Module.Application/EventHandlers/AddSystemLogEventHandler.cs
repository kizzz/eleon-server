using Common.EventBus.Module;
using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Messages.SystemLog;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.SystemLog;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.DocMessageLog.Module.Domain;
using VPortal.DocMessageLog.Module.Entities;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.EventServices;
public class AddSystemLogEventHandler : IDistributedEventHandler<AddSystemLogMsg>, ITransientDependency
{
  private readonly SystemLogDomainService _domainService;
  private readonly IVportalLogger<AddSystemLogEventHandler> _logger;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly IResponseContext _responseContext;

  public AddSystemLogEventHandler(
      SystemLogDomainService domainService,
      IVportalLogger<AddSystemLogEventHandler> logger,
      IUnitOfWorkManager unitOfWorkManager,
      IResponseContext responseContext)
  {
    _domainService = domainService;
    _logger = logger;
    _unitOfWorkManager = unitOfWorkManager;
    _responseContext = responseContext;
  }

  public async Task HandleEventAsync(AddSystemLogMsg eventData)
  {

    var response = new AddSystemLogResponseMsg()
    {
      Success = true,
    };

    try
    {
      using var uow = _unitOfWorkManager.Begin(requiresNew: true);

      var entities = new List<SystemLogEntity>();
      foreach (var log in eventData.Logs)
      {
        var entity = new SystemLogEntity
        {
          ApplicationName = log.ApplicationName,
          InitiatorId = log.ExtraProperties.GetOrDefault("InitiatorId")?.ToString(),
          InitiatorType = log.ExtraProperties.GetOrDefault("InitiatorType")?.ToString(),
          LogLevel = log.LogLevel,
          Message = log.Message,
          IsArchived = log.LogLevel == SystemLogLevel.Info
        };

        foreach (var prop in log.ExtraProperties)
        {
          entity.SetProperty(prop.Key, prop.Value?.ToString());
        }

        entities.Add(entity);
      }

      await _domainService.WriteManyAsync(entities);

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      response.Success = false;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
