using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Localization;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{
  // deprecated use chat room domain service with tag support
  [Obsolete("Use ChatRoomDomainService instead")]
  public class SupportTicketDomainService : DomainService
  {
    private readonly IVportalLogger<SupportTicketDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly ChatMemberDomainService chatMemberDomainService;
    private readonly ChatMessageDomainService chatMessageDomainService;
    private readonly ChatRoomDomainService chatRoomDomainService;
    private readonly IDistributedEventBus eventBus;
    private readonly IChatRoomRepository chatRoomRepository;

    public IStringLocalizer<CollaborationResource> L { get; }

    public SupportTicketDomainService(
        IVportalLogger<SupportTicketDomainService> logger,
        ICurrentUser currentUser,
        ChatMemberDomainService chatMemberDomainService,
        ChatMessageDomainService chatMessageDomainService,
        ChatRoomDomainService chatRoomDomainService,
        IStringLocalizer<CollaborationResource> L,
        IDistributedEventBus eventBus,
        IChatRoomRepository chatRoomRepository)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.chatMemberDomainService = chatMemberDomainService;
      this.chatMessageDomainService = chatMessageDomainService;
      this.chatRoomDomainService = chatRoomDomainService;
      this.L = L;
      this.eventBus = eventBus;
      this.chatRoomRepository = chatRoomRepository;
    }

    [Obsolete("Use ChatRoomDomainService instead")]
    public async Task<ChatRoomEntity> CreateSupportTicket(string title, List<Guid> initialMembers)
    {
      string ticketName = await GetTicketName(title);
      return await chatRoomDomainService.CreateChatAsync(ticketName, null, ChatRoomType.SupportTicket, initialMembers.ToDictionary(x => x, x => (ChatMemberRole?)null), isPublic: true, tags: ["support"]);
    }

    [Obsolete("Use ChatRoomDomainService instead")]
    public async Task CloseSupportTicket(Guid ticketChatRoomId)
    {
      var member = await chatMemberDomainService.EnsureCurrentUserAdminRights(ticketChatRoomId);
      var chat = await chatRoomRepository.GetAsync(ticketChatRoomId);
      chat.Status = ChatRoomStatus.Closed;
      await chatRoomRepository.UpdateAsync(chat, true);
      await chatMessageDomainService.AddChatClosedMessage(ticketChatRoomId, currentUser.Id.Value);
    }

    [Obsolete("Use ChatRoomDomainService instead")]
    public async Task<ChatRoomEntity> ForceRemoveSupportTickets(Guid userId)
    {
      var tickets = await chatRoomRepository.GetChatsByOwner(userId, [ChatRoomType.SupportTicket]);
      foreach (var ticket in tickets)
      {
        await chatMemberDomainService.ForceRemoveChatMembers(ticket.Id);
        await chatRoomRepository.DeleteAsync(ticket.Id, true);
      }

      return null;
    }

    private async Task<string> GetTicketName(string title)
    {
      string number = await GetSeriaNumber("SupportTicket");
      string titleTrimmed = title.IsNullOrWhiteSpace() ? string.Empty : $": {title.Trim()}";
      return $"{L["SupportTicketNamePrefix"]} #{number}{titleTrimmed}";
    }

    private async Task<string> GetSeriaNumber(string type)
    {
      var request = new GetDocumentSeriaNumberMsg
      {
        ObjectType = "SupportTicket",
        Prefix = Prefixes.ObjectTypePrefixes[type]
      };
      var response = await eventBus.RequestAsync<DocumentSeriaNumberGotMsg>(request);
      return response.SeriaNumber;
    }
  }
}
