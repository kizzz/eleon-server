using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace abp_sdk.Auditing;
public class ApiAuditLogContributor : AuditLogContributor
{
  public override void PostContribute(AuditLogContributionContext context)
  {
    var _logger = context.ServiceProvider.GetRequiredService<ILogger<ApiAuditLogContributor>>();
    _logger.LogDebug("ApiAuditLogContributor PostContribute started");
    try
    {
      var dto = new EleoncoreAuditLogDto
      {
        ApplicationName = context.AuditInfo.ApplicationName,
        ClientIpAddress = context.AuditInfo.ClientIpAddress,
        ClientName = context.AuditInfo.ClientName,
        ExecutionDuration = context.AuditInfo.ExecutionDuration,
        ExecutionTime = context.AuditInfo.ExecutionTime,
        ImpersonatorTenantId = context.AuditInfo.ImpersonatorTenantId,
        ImpersonatorUserId = context.AuditInfo.ImpersonatorUserId,
        TenantId = context.AuditInfo.TenantId,
        UserId = context.AuditInfo.UserId,
        UserName = context.AuditInfo.UserName,
        BrowserInfo = context.AuditInfo.BrowserInfo,
        Actions = context.AuditInfo.Actions?.Select(a => new EleoncoreAuditLogActionDto
        {
          ServiceName = a.ServiceName,
          MethodName = a.MethodName,
          Parameters = a.Parameters,
          ExecutionTime = a.ExecutionTime,
          ExecutionDuration = a.ExecutionDuration,
        }).ToList(),
        Comments = string.Join(';', context.AuditInfo.Comments),
        ClientId = context.AuditInfo.ClientId,
        CorrelationId = context.AuditInfo.CorrelationId,
        EntityChanges = context.AuditInfo.EntityChanges.Select(x => new EleoncoreEntityChangeDto
        {
          ChangeTime = x.ChangeTime,
          ChangeType = (EleoncoreEntityChangeType)x.ChangeType,
          EntityId = x.EntityId,
          EntityTypeFullName = x.EntityTypeFullName,
          TenantId = x.EntityTenantId,
          PropertyChanges = x.PropertyChanges?.Select(pc => new EleoncoreEntityPropertyChangeDto
          {
            NewValue = pc.NewValue,
            OriginalValue = pc.OriginalValue,
            PropertyName = pc.PropertyName,
            PropertyTypeFullName = pc.PropertyTypeFullName
          }).ToList()
        }).ToList()
      };

      var auditApi = new AuditLogApi();
      auditApi.UseApiAuth();

      Task.Run(async () => await auditApi.InfrastructureAuditLogAddAuditAsync(dto));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "ApiAuditLogContributor PostContribute error");
    }
    finally
    {
      _logger.LogDebug("ApiAuditLogContributor PostContribute finished");
    }
  }
}
