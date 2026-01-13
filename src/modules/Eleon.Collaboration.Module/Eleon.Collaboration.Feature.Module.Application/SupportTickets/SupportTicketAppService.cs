using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.SupportTickets
{
  [Authorize]
  public class SupportTicketAppService : CollaborationAppService, ISupportTicketAppService
  {
    private readonly IVportalLogger<SupportTicketAppService> logger;
    private readonly SupportTicketDomainService supportTicketDomainService;

    public SupportTicketAppService(
        IVportalLogger<SupportTicketAppService> logger,
        SupportTicketDomainService supportTicketDomainService)
    {
      this.logger = logger;
      this.supportTicketDomainService = supportTicketDomainService;
    }

    public async Task<bool> CloseSupportTicket(Guid ticketChatRoomId)
    {
      bool result = false;
      try
      {
        await supportTicketDomainService.CloseSupportTicket(ticketChatRoomId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatRoomDto> CreateSupportTicket(CreateSupportTicketRequestDto request)
    {
      ChatRoomDto result = null;
      try
      {
        var entity = await supportTicketDomainService.CreateSupportTicket(request.Title, request.InitialMembers);
        result = ObjectMapper.Map<ChatRoomEntity, ChatRoomDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
