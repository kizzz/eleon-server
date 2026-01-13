using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.Data;
using Volo.Abp.Guids;

namespace EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Helpers;
public static class AuditLogHelper
{
  /// <summary>
  /// Creates a deep copy of <paramref name="source"/> with a NEW Id (and new Ids for Actions & EntityChanges)
  /// using the provided <paramref name="guidGenerator"/>.
  /// </summary>
  public static AuditLog CopyWithNewIds(AuditLog source, IGuidGenerator guidGenerator)
  {
    if (source == null) throw new ArgumentNullException(nameof(source));
    if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

    var newAuditLogId = guidGenerator.Create();

    // ExtraProperties
    var extraProps = source.ExtraProperties != null
        ? new ExtraPropertyDictionary(source.ExtraProperties)
        : new ExtraPropertyDictionary();

    // ----- Actions -----
    // Rebuild through AuditLogActionInfo so we can use the public ctor
    var newActions = source.Actions?.Select(a =>
    {
      var info = new AuditLogActionInfo
      {
        ServiceName = a.ServiceName,
        MethodName = a.MethodName,
        Parameters = a.Parameters,
        ExecutionTime = a.ExecutionTime,
        ExecutionDuration = a.ExecutionDuration,
      };

      if (a.ExtraProperties != null)
      {
        foreach (var prop in a.ExtraProperties)
        {
          info.ExtraProperties[prop.Key] = prop.Value;
        }
      }

      return new AuditLogAction(
              id: guidGenerator.Create(),
              auditLogId: newAuditLogId,
              actionInfo: info,
              tenantId: a.TenantId
          );
    }).ToList() ?? new List<AuditLogAction>();

    // ----- EntityChanges -----
    // Build EntityChangeInfo (+ nested EntityPropertyChangeInfo) so we can use the public ctor of EntityChange
    var newEntityChanges = source.EntityChanges?.Select(ec =>
    {
      var propInfos = ec.PropertyChanges?.Select(pc => new EntityPropertyChangeInfo
      {
        NewValue = pc.NewValue,
        OriginalValue = pc.OriginalValue,
        PropertyName = pc.PropertyName,
        PropertyTypeFullName = pc.PropertyTypeFullName
      }).ToList() ?? new List<EntityPropertyChangeInfo>();

      var changeInfo = new EntityChangeInfo
      {
        ChangeTime = ec.ChangeTime,
        ChangeType = ec.ChangeType,
        EntityTenantId = ec.EntityTenantId,
        EntityId = ec.EntityId,
        EntityTypeFullName = ec.EntityTypeFullName,
        PropertyChanges = propInfos,
      };

      if (ec.ExtraProperties != null)
      {
        foreach (var prop in ec.ExtraProperties)
        {
          changeInfo.ExtraProperties[prop.Key] = prop.Value;
        }
      }

      // Public ctor requires (IGuidGenerator, auditLogId, EntityChangeInfo, tenantId)
      return new EntityChange(
              guidGenerator: guidGenerator,
              auditLogId: newAuditLogId,
              entityChangeInfo: changeInfo,
              tenantId: ec.TenantId
          );
    }).ToList() ?? new List<EntityChange>();

    // Construct the cloned AuditLog with a brand-new Id
    var clone = new AuditLog(
        id: newAuditLogId,
        applicationName: source.ApplicationName,
        tenantId: source.TenantId,
        tenantName: source.TenantName,
        userId: source.UserId,
        userName: source.UserName,
        executionTime: source.ExecutionTime,
        executionDuration: source.ExecutionDuration,
        clientIpAddress: source.ClientIpAddress,
        clientName: source.ClientName,
        clientId: source.ClientId,
        correlationId: source.CorrelationId,
        browserInfo: source.BrowserInfo,
        httpMethod: source.HttpMethod,
        url: source.Url,
        httpStatusCode: source.HttpStatusCode,
        impersonatorUserId: source.ImpersonatorUserId,
        impersonatorUserName: source.ImpersonatorUserName,
        impersonatorTenantId: source.ImpersonatorTenantId,
        impersonatorTenantName: source.ImpersonatorTenantName,
        extraPropertyDictionary: extraProps,
        entityChanges: newEntityChanges,
        actions: newActions,
        exceptions: source.Exceptions,
        comments: source.Comments
    );

    return clone;
  }
}
