using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Collaboration.Feature.Module.ChatRooms;

namespace VPortal.Collaboration.Feature.Module.SupportTickets
{
  public interface ISupportTicketAppService : IApplicationService
  {
    Task<ChatRoomDto> CreateSupportTicket(CreateSupportTicketRequestDto request);
    Task<bool> CloseSupportTicket(Guid ticketChatRoomId);
  }
}
