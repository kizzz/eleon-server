using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{
  public class SecurityLogDomainService : DomainService, ISingletonDependency
  {
    private readonly IIdentitySecurityLogRepository identitySecurityLogRepository;
    private readonly IVportalLogger<SecurityLogDomainService> logger;

    public SecurityLogDomainService(
        IIdentitySecurityLogRepository identitySecurityLogRepository,
        IVportalLogger<SecurityLogDomainService> logger)
    {
      this.identitySecurityLogRepository = identitySecurityLogRepository;
      this.logger = logger;
    }

    public async Task<IdentitySecurityLog> GetSecurityLogByIdAsync(Guid id)
    {
      IdentitySecurityLog securityLog = null;
      try
      {
        securityLog = await identitySecurityLogRepository.GetAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return securityLog;
    }

    public async Task<KeyValuePair<long, List<IdentitySecurityLog>>> GetSecurityLogList(
           string sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           DateTime? startTime = null,
           DateTime? endTime = null,
           string action = null,
           Guid? userId = null,
           string userName = null,
           string applicationName = null,
           string correlationId = null,
           string identity = null,
           string clientId = null,
           string clientIpAddress = null)
    {
      KeyValuePair<long, List<IdentitySecurityLog>> result = default;
      List<IdentitySecurityLog> securityLogs = new List<IdentitySecurityLog>();
      try
      {
        if (startTime.HasValue)
        {
          startTime = startTime.Value.AddDays(1).Date.Date;
        }

        if (endTime.HasValue)
        {
          endTime = endTime.Value.Date.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        long count = 0;
        count = await identitySecurityLogRepository.GetCountAsync(startTime, endTime, applicationName, identity, action, userId, userName, clientId, correlationId, clientIpAddress);
        if (count > 0)
        {
          securityLogs = await identitySecurityLogRepository.GetListAsync(sorting, maxResultCount, skipCount, startTime, endTime, applicationName, identity, action, userId, userName, clientId, correlationId, clientIpAddress);
        }

        result = new KeyValuePair<long, List<IdentitySecurityLog>>(count, securityLogs);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
