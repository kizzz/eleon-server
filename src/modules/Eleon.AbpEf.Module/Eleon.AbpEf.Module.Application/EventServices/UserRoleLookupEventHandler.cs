using Common.EventBus.Module;
using Commons.Module.Messages.Permissions;
using Eleon.InternalCommons.Lib.Messages.UserRole;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.Repositories;
using VPortal.TenantManagement.Module.Roles;

namespace TenantManagementModule.TenantManagement.Module.Domain.EventServices;

public class UserRoleLookupEventHandler : IDistributedEventHandler<GetUsersInRoleMsg>, IDistributedEventHandler<GetRolesByUserIdMsg>, ITransientDependency
{
  private readonly IVportalLogger<UserRoleLookupEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly IRoleLookupRepository _roleLookupRepository;

  public UserRoleLookupEventHandler(
      IVportalLogger<UserRoleLookupEventHandler> logger,
      IResponseContext responseContext,
      IRoleLookupRepository roleLookupRepository)
  {
    _logger = logger;
    _responseContext = responseContext;
    _roleLookupRepository = roleLookupRepository;
  }

  public async Task HandleEventAsync(GetUsersInRoleMsg eventData)
  {
    var response = new GetUsersInRoleResponseMsg { TotalCount = 0, Users = new List<RoleUserLookup>() };
    try
    {
      var result = await _roleLookupRepository.GetUsersByRole(eventData.RoleName, eventData.UserNameFilter, eventData.Skip, eventData.Take, eventData.ExclusionMode);
      response.TotalCount = result.Key;
      response.Users = result.Value;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(GetRolesByUserIdMsg eventData)
  {
    var response = new GetRolesByUserIdResponseMsg { Roles = new List<UserRoleLookup>() };
    try
    {
      var result = await _roleLookupRepository.GetRolesByUserId(eventData.UserId, eventData.ProviderFormat);
      response.Roles = result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
