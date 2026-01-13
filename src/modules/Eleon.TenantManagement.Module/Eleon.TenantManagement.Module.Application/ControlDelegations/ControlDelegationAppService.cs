using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class ControlDelegationAppService : TenantManagementAppService, IControlDelegationAppService
  {
    private readonly IVportalLogger<ControlDelegationAppService> logger;
    private readonly ControlDelegationDomainService controlDelegationDomainService;

    public ControlDelegationAppService(
        IVportalLogger<ControlDelegationAppService> logger,
        ControlDelegationDomainService controlDelegationDomainService)
    {
      this.logger = logger;
      this.controlDelegationDomainService = controlDelegationDomainService;
    }

    public async Task<bool> AddControlDelegation(CreateControlDelegationRequestDto request)
    {
      bool result = false;
      try
      {
        await controlDelegationDomainService.AddControlDelegation(request.DelegatedToUserId, request.DelegationStartDate, request.DelegationEndDate, request.Reason);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ControlDelegationDto>> GetActiveControlDelegationsByUser()
    {
      List<ControlDelegationDto> result = null;
      try
      {
        var entities = await controlDelegationDomainService.GetActiveControlDelegationsByUser();
        result = ObjectMapper.Map<List<ControlDelegationEntity>, List<ControlDelegationDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ControlDelegationDto>> GetActiveControlDelegationsToUser()
    {
      List<ControlDelegationDto> result = null;
      try
      {
        var entities = await controlDelegationDomainService.GetActiveControlDelegationsToUser();
        result = ObjectMapper.Map<List<ControlDelegationEntity>, List<ControlDelegationDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ControlDelegationDto> GetControlDelegation(Guid delegationId)
    {
      ControlDelegationDto result = null;
      try
      {
        var entity = await controlDelegationDomainService.GetControlDelegation(delegationId);
        result = ObjectMapper.Map<ControlDelegationEntity, ControlDelegationDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<PagedResultDto<ControlDelegationDto>> GetControlDelegationsByUser(int skip, int take)
    {
      PagedResultDto<ControlDelegationDto> result = null;
      try
      {
        var entities = await controlDelegationDomainService.GetControlDelegationsByUser(skip, take);
        var dtos = ObjectMapper.Map<List<ControlDelegationEntity>, List<ControlDelegationDto>>(entities.Value);
        result = new(entities.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetControlDelegationActiveState(Guid delegationId, bool isActive)
    {
      bool result = false;
      try
      {
        await controlDelegationDomainService.SetControlDelegationActiveState(delegationId, isActive);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> UpdateControlDelegation(UpdateControlDelegationRequestDto request)
    {
      bool result = false;
      try
      {
        await controlDelegationDomainService.UpdateControlDelegation(request.DelegationId, request.FromDate, request.ToDate, request.Reason);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
