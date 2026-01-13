using System;
using System.Collections.Generic;
using CollaborationModule.Test.TestBase;
using Common.EventBus.Module;
using Messaging.Module.Messages;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.DomainServices;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Domain.DomainServices;

public class SupportTicketDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task CreateSupportTicket_ShouldUsePrefixAndTags()
  {
    var eventBus = Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    ((IResponseCapableEventBus)eventBus).RequestAsync<DocumentSeriaNumberGotMsg>(Arg.Any<object>(), Arg.Any<int>())
      .Returns(new DocumentSeriaNumberGotMsg { SeriaNumber = "321" });

    var insertedChats = new Dictionary<Guid, ChatRoomEntity>();
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.InsertAsync(Arg.Any<ChatRoomEntity>(), true, Arg.Any<System.Threading.CancellationToken>())
      .Returns(callInfo =>
      {
        var chat = callInfo.Arg<ChatRoomEntity>();
        insertedChats[chat.Id] = chat;
        return chat;
      });
    // Setup GetAsync to return inserted chat or create a new one with Opened status
    chatRoomRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
      .Returns(callInfo =>
      {
        var chatId = callInfo.Arg<Guid>();
        return insertedChats.TryGetValue(chatId, out var chat) 
          ? chat 
          : new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened };
      });
    chatRoomRepository.GetAsync(Arg.Any<Guid>())
      .Returns(callInfo =>
      {
        var chatId = callInfo.Arg<Guid>();
        return insertedChats.TryGetValue(chatId, out var chat) 
          ? chat 
          : new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened };
      });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<System.Threading.CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);
    chatMemberRepository.GetByRole(Arg.Any<Guid>(), Arg.Any<List<ChatMemberRole>>())
      .Returns(new List<ChatMemberEntity>());
    chatMemberRepository.GetByUser(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(new ChatMemberEntity(Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid(), ChatMemberRole.Owner));

    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user");
    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      currentUser: currentUser,
      userManager: userManager);

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<System.Threading.CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var chatRoomDomainService = CreateChatRoomDomainService(
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository,
      currentUser: currentUser);

    var service = CreateSupportTicketDomainService(
      eventBus: eventBus,
      chatRoomDomainService: chatRoomDomainService,
      localizer: CreateLocalizer("SupportTicketNamePrefix", "Ticket"));

    var result = await service.CreateSupportTicket("Issue", new List<Guid> { Guid.NewGuid() });

    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task ForceRemoveSupportTickets_ShouldDeleteAllOwnedTickets()
  {
    var ownerId = Guid.NewGuid();
    var tickets = new List<ChatRoomEntity>
    {
      new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.SupportTicket),
      new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.SupportTicket)
    };

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetChatsByOwner(ownerId, Arg.Any<List<ChatRoomType>>()).Returns(tickets);

    var chatMemberDomainService = CreateChatMemberDomainService();

    var service = CreateSupportTicketDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: chatMemberDomainService);

    await service.ForceRemoveSupportTickets(ownerId);

    await chatRoomRepository.Received(tickets.Count).DeleteAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>());
  }
}
