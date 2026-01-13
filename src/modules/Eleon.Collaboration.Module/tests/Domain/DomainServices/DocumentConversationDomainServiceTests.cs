using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using Common.EventBus.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace CollaborationModule.Test.Domain.DomainServices;

public class DocumentConversationDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task GetDocumentConversationInfo_ShouldReturnExistingChat_WhenFound()
  {
    var chatId = Guid.NewGuid();
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetByRefId(Arg.Any<string>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user");
    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      userManager: userManager);
    var service = CreateDocumentConversationDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: chatMemberDomainService);

    var result = await service.GetDocumentConversationInfo("Doc", "123");

    result.ShouldNotBeNull();
    result.ChatRoom.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetDocumentConversationInfo_ShouldCreateChat_WhenMissing()
  {
    var createdChat = new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.Group);

    var insertedChats = new Dictionary<Guid, ChatRoomEntity>();
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetByRefId(Arg.Any<string>()).Returns((ChatRoomEntity)null);
    chatRoomRepository.InsertAsync(Arg.Any<ChatRoomEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo =>
      {
        var chat = callInfo.Arg<ChatRoomEntity>();
        insertedChats[chat.Id] = chat;
        return chat;
      });
    chatRoomRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(callInfo =>
      {
        var chatId = callInfo.Arg<Guid>();
        if (insertedChats.TryGetValue(chatId, out var chat))
        {
          return chat;
        }
        return new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened };
      });
    chatRoomRepository.GetAsync(Arg.Any<Guid>())
      .Returns(callInfo =>
      {
        var chatId = callInfo.Arg<Guid>();
        if (insertedChats.TryGetValue(chatId, out var chat))
        {
          return chat;
        }
        return new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened };
      });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);
    chatMemberRepository.GetByMember(Arg.Any<Guid>(), Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>());

    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user", new List<string> { "User" });
    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      currentUser: currentUser,
      userManager: userManager);

    var eventBus = Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    ((IResponseCapableEventBus)eventBus).RequestAsync<DocumentSeriaNumberGotMsg>(Arg.Any<object>(), Arg.Any<int>())
      .Returns(new DocumentSeriaNumberGotMsg { SeriaNumber = "100" });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var chatRoomDomainService = CreateChatRoomDomainService(
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository,
      eventBus: eventBus,
      localizer: CreateLocalizer("GroupChatNamePrefix", "Group"),
      currentUser: currentUser);

    var service = CreateDocumentConversationDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: chatMemberDomainService,
      chatRoomDomainService: chatRoomDomainService);

    var result = await service.GetDocumentConversationInfo("Doc", "123");

    result.ShouldNotBeNull();
    result.ChatRoom.ShouldNotBeNull();
  }

  [Fact]
  public async Task SendDocumentMessages_ShouldCreateConversation_WhenMissing()
  {
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetByRefId(Arg.Any<string>()).Returns((ChatRoomEntity)null);
    chatRoomRepository.InsertAsync(Arg.Any<ChatRoomEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatRoomEntity>());
    chatRoomRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(callInfo => new ChatRoomEntity(callInfo.Arg<Guid>(), ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByChat(Arg.Any<Guid>(), Arg.Any<List<ChatMemberType>>())
      .Returns(new List<ChatMemberEntity>());
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var eventBus = Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    ((IResponseCapableEventBus)eventBus).RequestAsync<DocumentSeriaNumberGotMsg>(Arg.Any<object>(), Arg.Any<int>())
      .Returns(new DocumentSeriaNumberGotMsg { SeriaNumber = "200" });
    TestMockHelpers.SetupEventBusPublishAsync(eventBus);

    var userChatSettingRepository = Substitute.For<IUserChatSettingRepository>();
    userChatSettingRepository.GetMutedUsers(Arg.Any<Guid>()).Returns(new List<Guid>());
    userChatSettingRepository.UnarchiveChatAsync(Arg.Any<Guid>(), Arg.Any<List<Guid>>()).Returns(0);

    var chatMessagePushManager = CreateChatMessagePushManager(
      eventBus: eventBus,
      userChatSettingRepository: userChatSettingRepository,
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository,
      pushManager: chatMessagePushManager,
      eventBus: eventBus);

    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var chatRoomDomainService = CreateChatRoomDomainService(
      chatMemberDomainService: chatMemberDomainService,
      chatRoomRepository: chatRoomRepository,
      eventBus: eventBus,
      localizer: CreateLocalizer("GroupChatNamePrefix", "Group"));

    var service = CreateDocumentConversationDomainService(
      chatRoomRepository: chatRoomRepository,
      chatRoomDomainService: chatRoomDomainService,
      chatMessageDomainService: chatMessageDomainService);

    await service.SendDocumentMessages(new List<DocumentChatMessageEto>
    {
      new DocumentChatMessageEto
      {
        DocumentObjectType = "Doc",
        DocumentId = Guid.NewGuid().ToString(),
        LocalizationKey = "Key",
        LocalizationParams = new[] { "p" },
        MessageSeverity = ChatMessageSeverity.Info
      }
    });

    await messageRepository.Received(1)
      .InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }
}
