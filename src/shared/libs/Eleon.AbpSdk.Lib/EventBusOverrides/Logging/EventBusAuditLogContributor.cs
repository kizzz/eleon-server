using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.Auditing;

public class EventBusAuditLogContributor : AuditLogContributor
{
  public override void PostContribute(AuditLogContributionContext context)
  {
    var _logger = context.ServiceProvider.GetRequiredService<ILogger<EventBusAuditLogContributor>>();
    var scopeFactory = context.ServiceProvider.GetRequiredService<IServiceScopeFactory>();

    _logger.LogDebug("ApiAuditLogContributor PostContribute started");
    try
    {
        // fire-and-forget on a new scope so we don't reuse the disposed request scope
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var scopedBus = scope.ServiceProvider.GetRequiredService<IDistributedEventBus>();

                var auditLogEto = AuditLogEto.FromAuditInfo(context.AuditInfo);
                await scopedBus.PublishAsync(new AddAuditMsg { AuditLogInfo = auditLogEto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApiAuditLogContributor PostContribute: failed to send audit log");
            }
        });
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
