using Common.EventBus.Module;
using Commons.Module.Messages.Permissions;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.Repositories;

namespace TenantManagementModule.TenantManagement.Module.Domain.EventServices;

public class GetGrantedRolePermissionsEventHandler : IDistributedEventHandler<GetGrantedRolePermissionsMsg>, ITransientDependency
{
  private readonly IVportalLogger<GetGrantedRolePermissionsEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly IRoleLookupRepository _roleLookupRepository;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public GetGrantedRolePermissionsEventHandler(
      IVportalLogger<GetGrantedRolePermissionsEventHandler> logger,
      IResponseContext responseContext,
      IRoleLookupRepository roleLookupRepository,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _logger = logger;
    _responseContext = responseContext;
    _roleLookupRepository = roleLookupRepository;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(GetGrantedRolePermissionsMsg eventData)
  {
    var response = new GetGrantedRolePermissionsResponseMsg { Permissions = new List<string>() };
    try
    {
      using var uow = _unitOfWorkManager.Begin();
      var result = await _roleLookupRepository.GetGrantedRolePermissions(eventData.UserId);
      response.Permissions = result;
      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
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
