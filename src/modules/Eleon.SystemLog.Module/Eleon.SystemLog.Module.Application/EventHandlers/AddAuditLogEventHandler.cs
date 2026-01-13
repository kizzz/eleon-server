using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Audit;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using VPortal.Infrastructure.Module.Domain.DomainServices;

namespace EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.EventServices.AuditLogs;
public class AddAuditLogEventHandler : IDistributedEventHandler<AddAuditMsg>, ITransientDependency
{
  private readonly IVportalLogger<AuditLogDomainService> _logger;
  private readonly AuditLogDomainService _auditLogDomainService;
  private readonly IGuidGenerator _guidGenerator;

  public AddAuditLogEventHandler(
      IVportalLogger<AuditLogDomainService> logger,
      AuditLogDomainService auditLogDomainService,
      IGuidGenerator guidGenerator)
  {
    _logger = logger;
    _auditLogDomainService = auditLogDomainService;
    _guidGenerator = guidGenerator;
  }

  public async Task HandleEventAsync(AddAuditMsg eventData)
  {

    try
    {
      var auditLog = ToAuditLogEntity(eventData.AuditLogInfo?.ToAuditInfo(), _guidGenerator);
      await _auditLogDomainService.AddAuditLogAsync(auditLog);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  /// <summary>
  /// Converts ABP pipeline AuditLogInfo to the persisted AuditLog entity.
  /// </summary>
  public static AuditLog ToAuditLogEntity(AuditLogInfo info, IGuidGenerator guidGenerator)
  {
    if (info == null) throw new ArgumentNullException(nameof(info));
    if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

    var id = guidGenerator.Create();
    var entityChanges = MapEntityChanges(id, info, guidGenerator);
    var actions = MapActions(id, info, guidGenerator);


    return new AuditLog(
        id: id,
        applicationName: info.ApplicationName,
        tenantId: info.TenantId,
        tenantName: info.TenantName, // may be null depending on your pipeline
        userId: info.UserId,
        userName: info.UserName,
        executionTime: info.ExecutionTime,
        executionDuration: info.ExecutionDuration,
        clientIpAddress: info.ClientIpAddress,
        clientName: info.ClientName,
        clientId: info.ClientId,
        correlationId: info.CorrelationId,
        browserInfo: info.BrowserInfo,
        httpMethod: info.HttpMethod,
        url: info.Url,
        httpStatusCode: info.HttpStatusCode,
        impersonatorUserId: info.ImpersonatorUserId,
        impersonatorUserName: info.ImpersonatorUserName,   // if your ABP version provides it
        impersonatorTenantId: info.ImpersonatorTenantId,
        impersonatorTenantName: info.ImpersonatorTenantName, // if available in your version
        extraPropertyDictionary: info.ExtraProperties ?? new(),
        entityChanges: entityChanges,
        actions: actions,
        exceptions: FlattenExceptions(info),
        comments: string.Join(';', info.Comments ?? [])
    );
  }

  private static List<AuditLogAction> MapActions(Guid auditId, AuditLogInfo info, IGuidGenerator guidGenerator)
  {
    return info.Actions?.Select(a =>
        new AuditLogAction(
            guidGenerator.Create(),
            auditId,
            a,
            info.TenantId
        )
    )?.ToList() ?? [];
  }

  private static List<EntityChange> MapEntityChanges(Guid auditId, AuditLogInfo info, IGuidGenerator guidGenerator)
  {
    return info.EntityChanges?.Select(ec =>
        new EntityChange(
            guidGenerator,
            auditId,
            ec,
            info.TenantId
        )
    )?.ToList() ?? [];
  }

  private static string FlattenExceptions(AuditLogInfo info)
  {
    if (info.Exceptions == null || info.Exceptions.Count == 0)
      return string.Empty;

    if (info.Exceptions != null && info.Exceptions.Any())
    {
      var sb = new StringBuilder();
      foreach (var ex in info.Exceptions)
      {
        sb.AppendLine(ex.Message);
        if (!ex.StackTrace.IsNullOrWhiteSpace())
        {
          sb.AppendLine(ex.StackTrace);
        }
        sb.AppendLine(new string('-', 60));
      }
      return sb.ToString();
    }

    return string.Empty;
  }
}
