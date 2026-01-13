using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using GatewayManagement.Module.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Proxies;
using VPortal.GatewayManagement.Module.EventBuses;
using EventBusManagement.Module.EntityFrameworkCore;

namespace GatewayManagement.Module.Proxies
{
  [Authorize]
  public class GatewayManagementAppService : GatewayManagementBaseAppService, IGatewayManagementAppService
  {
    private readonly IVportalLogger<GatewayManagementAppService> logger;
    private readonly GatewayManagementDomainService gatewayDomainService;

    public GatewayManagementAppService(
        GatewayManagementDomainService gatewayDomainService,
        IVportalLogger<GatewayManagementAppService> logger)
    {
      this.gatewayDomainService = gatewayDomainService;
      this.logger = logger;
    }

    public async Task<string> AddGateway(GatewayDto gateway)
    {
      string response = null;
      try
      {
        var gatewayEntity = ObjectMapper.Map<GatewayDto, GatewayEntity>(gateway);
        response = await gatewayDomainService.AddGateway(gatewayEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<GatewayRegistrationKeyDto> GetCurrentGatewayRegistrationKey(Guid gatewayId)
    {
      GatewayRegistrationKeyDto response = null;
      try
      {
        var keyEntity = await gatewayDomainService.GetCurrentGatewayRegistrationKey(gatewayId);
        response = ObjectMapper.Map<GatewayRegistrationKeyEntity, GatewayRegistrationKeyDto>(keyEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<GatewayDto> GetGateway(Guid gatewayId)
    {
      GatewayDto response = null;
      try
      {
        var gatewayEntity = await gatewayDomainService.GetGateway(gatewayId);
        response = ObjectMapper.Map<GatewayEntity, GatewayDto>(gatewayEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<GatewayDto>> GetGatewayList(GatewayListRequestDto request)
    {
      List<GatewayDto> response = null;
      try
      {
        var gatewayEntity = await gatewayDomainService.GetGatewayList(request.StatusFilter);
        response = ObjectMapper.Map<List<GatewayEntity>, List<GatewayDto>>(gatewayEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> RemoveGateway(Guid gatewayId)
    {
      bool response = false;
      try
      {
        await gatewayDomainService.RemoveGateway(gatewayId);
        response = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<GatewayRegistrationKeyDto> RequestGatewayRegistration(Guid gatewayId)
    {
      GatewayRegistrationKeyDto response = null;
      try
      {
        var keyEntity = await gatewayDomainService.RequestGatewayRegistration(gatewayId);
        response = ObjectMapper.Map<GatewayRegistrationKeyEntity, GatewayRegistrationKeyDto>(keyEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> UpdateGateway(UpdateGatewayRequestDto request)
    {
      bool response = false;
      try
      {
        var gatewayEntity = ObjectMapper.Map<GatewayDto, GatewayEntity>(request.Gateway);
        var eventBusEntity = ObjectMapper.Map<EventBusDto, EventBusEntity>(request.EventBus);
        response = await gatewayDomainService.UpdateGateway(gatewayEntity, eventBusEntity, request.UseDefault);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> CancelOngoingGatewayRegistration(Guid gatewayId)
    {
      bool response = false;
      try
      {
        await gatewayDomainService.CancelOngoingGatewayRegistration(gatewayId);
        response = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task AcceptPendingGateway(AcceptPendingGatewayRequestDto request)
    {
      try
      {
        await gatewayDomainService.AcceptPendingGateway(request.GatewayId, request.Name);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task RejectPendingGateway(Guid gatewayId)
    {
      try
      {
        await gatewayDomainService.RejectPendingGateway(gatewayId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }
  }
}
