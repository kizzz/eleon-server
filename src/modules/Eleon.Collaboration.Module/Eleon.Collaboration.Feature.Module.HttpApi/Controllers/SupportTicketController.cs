using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.SupportTickets;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/SupportTickets")]
  public class SupportTicketController : ChatController, ISupportTicketAppService
  {
    private readonly ISupportTicketAppService appService;
    private readonly IVportalLogger<SupportTicketController> _logger;

    public SupportTicketController(
        ISupportTicketAppService appService,
        IVportalLogger<SupportTicketController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("CloseSupportTicket")]
    public async Task<bool> CloseSupportTicket(Guid ticketChatRoomId)
    {

      var response = await appService.CloseSupportTicket(ticketChatRoomId);


      return response;
    }

    [HttpPost("CreateSupportTicket")]
    public async Task<ChatRoomDto> CreateSupportTicket(CreateSupportTicketRequestDto request)
    {

      var response = await appService.CreateSupportTicket(request);


      return response;
    }
  }
}
