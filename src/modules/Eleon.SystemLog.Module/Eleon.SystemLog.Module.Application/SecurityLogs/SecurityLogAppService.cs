using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using VPortal.DocMessageLog.Module;
using VPortal.DocMessageLog.Module.Permissions;
using VPortal.Infrastructure.Module.Domain.DomainServices;

namespace VPortal.Infrastructure.Module.SecurityLogs
{
  [Authorize]
  public class SecurityLogAppService : SystemLogModuleAppService, ISecurityLogAppService
  {
    private readonly IVportalLogger<SecurityLogAppService> logger;
    private readonly IPermissionChecker permissionChecker;
    private readonly SecurityLogDomainService domainService;

    public SecurityLogAppService(
        IVportalLogger<SecurityLogAppService> logger,
        IPermissionChecker permissionChecker,
        SecurityLogDomainService domainService)
    {
      this.logger = logger;
      this.permissionChecker = permissionChecker;
      this.domainService = domainService;
    }

    public async Task<FullSecurityLogDto> GetSecurityLogByIdAsync(Guid id)
    {
      FullSecurityLogDto result = null;
      try
      {
        var entity = await domainService.GetSecurityLogByIdAsync(id);
        result = ObjectMapper.Map<IdentitySecurityLog, FullSecurityLogDto>(entity);

      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
      return result;
    }

    public async Task<PagedResultDto<SecurityLogDto>> GetSecurityLogList(SecurityLogListRequestDto input)
    {
      PagedResultDto<SecurityLogDto> result = new PagedResultDto<SecurityLogDto>();
      try
      {
        if (input.UserId == null || input.UserId != CurrentUser.Id)
        {
          if (!await permissionChecker.IsGrantedAsync(SystemLogModulePermissions.ViewSecurityLogs))
          {
            throw new AbpAuthorizationException("User does not have a permission to view security logs.");
          }
        }

        var pair = await domainService.GetSecurityLogList(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.StartTime,
            input.EndTime,
            input.Action,
            input.UserId,
            input.UserName,
            input.ApplicationName,
            input.CorrelationId,
            input.Identity,
            input.ClientId,
            input.ClientIpAddress);
        var dtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<SecurityLogDto>>(pair.Value);
        result = new PagedResultDto<SecurityLogDto>(pair.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
