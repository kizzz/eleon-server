using Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.LifecycleFeatureModule.Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;
using VPortal.Lifecycle.Feature.Module.ValueObjects;

namespace VPortal.Lifecycle.Feature.Module.LifecycleManager
{

  [Authorize]
  public class LifecycleManagerAppService : ModuleAppService, ILifecycleManagerAppService
  {
    private readonly IVportalLogger<LifecycleManagerAppService> logger;
    private readonly LifecycleManagerDomainService lifecycleManagerDomainService;
    private readonly StatesGroupAuditDomainService statesGroupAuditDomainService;
    private readonly ICurrentPrincipalAccessor currentPrincipalAccessor;
    private readonly IdentityUserManager currentUserService;

    public LifecycleManagerAppService(
        IVportalLogger<LifecycleManagerAppService> logger,
        LifecycleManagerDomainService lifecycleManagerDomainService,
        StatesGroupAuditDomainService statesGroupAuditDomainService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IdentityUserManager currentUserService)
    {
      this.logger = logger;
      this.lifecycleManagerDomainService = lifecycleManagerDomainService;
      this.statesGroupAuditDomainService = statesGroupAuditDomainService;
      this.currentPrincipalAccessor = currentPrincipalAccessor;
      this.currentUserService = currentUserService;
    }

    [Authorize(LifecyclePermissions.General)]
    public async Task<bool> Reject(string documentObjectType, string documentId, string reason)
    {
      bool result;
      try
      {
        result = await lifecycleManagerDomainService.ChangeApprovmentStatus(documentObjectType, documentId, reason, false);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Approve(string documentObjectType, string documentId)
    {
      bool result;
      try
      {
        result = await lifecycleManagerDomainService.ChangeApprovmentStatus(documentObjectType, documentId, string.Empty, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CanApprove(string documentObjectType, string documentId)
    {

      bool result;
      try
      {
        result = await lifecycleManagerDomainService.GetUserLifecycleApprovalPermission(
            documentObjectType, documentId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;

    }

    // TODO: return [Authorize(LifecyclePermissions.LifecycleManager)] 
    public async Task<StatesGroupAuditTreeDto> GetTrace(string documentObjectType, string documentId)
    {

      StatesGroupAuditTreeDto result;
      try
      {
        var audit = await lifecycleManagerDomainService.GetTrace(documentObjectType, documentId);
        result = ObjectMapper.Map<StatesGroupAuditEntity, StatesGroupAuditTreeDto>(audit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;

    }
    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<LifecycleStatusDto> GetLifecycleStatus(string documentObjectType, string documentId)
    {
      LifecycleStatusDto result = null;
      try
      {
        LifecycleStatusValueObject valueObject = await lifecycleManagerDomainService.GetLifecycleStatus(documentObjectType, documentId);
        result = ObjectMapper.Map<LifecycleStatusValueObject, LifecycleStatusDto>(valueObject);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> GetViewPermissionAsync(Guid initiatorId, string documentObjectType, string documentId, bool review = true)
    {
      var result = false;
      try
      {
        var principal = await currentUserService.CreatePrincipal(initiatorId);
        using (currentPrincipalAccessor.Change(principal))
        {
          result = await lifecycleManagerDomainService.GetViewPermission(documentObjectType, documentId, review);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }
    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<List<string>> GetDocumentIdsByFilterAsync(GetDocumentIdsByFilterRequestDto input)
    {
      var result = new List<string>();
      try
      {
        result = await statesGroupAuditDomainService.GetDocumentIdsByFilter(
            input.DocumentObjectType,
            input.UserId,
            input.Roles,
            input.LifecycleStatuses
            );
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }
    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<StatesGroupAuditTreeDto> StartNewLifecycleAsync(StartNewLifecycleRequestDto input)
    {
      StatesGroupAuditTreeDto result = null;
      try
      {
        var audit = await lifecycleManagerDomainService.StartNewLifecycle(
                input.TemplateId,
                input.DocumentObjectType,
                input.DocumentId,
                input.ExtraProperties,
                input.IsSkipFilled,
                input.StartImmediately);
        result = ObjectMapper.Map<StatesGroupAuditEntity, StatesGroupAuditTreeDto>(audit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }
    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<StatesGroupAuditTreeDto> StartExistingLifecycleAsync(string documentObjectType, string documentId)
    {
      StatesGroupAuditTreeDto result = null;
      try
      {
        var audit = await lifecycleManagerDomainService.StartExistingLifecycle(
            documentObjectType,
            documentId);
        result = ObjectMapper.Map<StatesGroupAuditEntity, StatesGroupAuditTreeDto>(audit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CanReview(string documentObjectType, string documentId)
    {
      bool result = false;
      try
      {
        result = await lifecycleManagerDomainService.CanReview(documentObjectType, documentId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }
  }
}
