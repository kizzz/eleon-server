using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using Common.EventBus.Module;
using Eleon.TestsBase.Lib.TestHelpers;
using Messaging.Module.Messages;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Users;

namespace CollaborationModule.Test.Domain.DomainServices;

public class ChatRoomDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task CreateChatAsync_ShouldUseLocalizedPrefix_WhenNameIsEmpty()
  {
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.InsertAsync(Arg.Any<ChatRoomEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatRoomEntity>());

    var eventBus = CreateMockResponseCapableEventBus();
    SetupEventBusRequestAsync<object, DocumentSeriaNumberGotMsg>(
      eventBus, 
      new DocumentSeriaNumberGotMsg { SeriaNumber = "007" });

    var guidGenerator = CreateMockGuidGenerator(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    var localizer = CreateLocalizer("GroupChatNamePrefix", "GroupChat");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      guidGenerator: guidGenerator);

    var service = CreateChatRoomDomainService(
      chatMemberDomainService: chatMemberDomainService,
      chatRoomRepository: chatRoomRepository,
      eventBus: (IDistributedEventBus)eventBus,
      localizer: localizer,
      guidGenerator: guidGenerator);

    var result = await service.CreateChatAsync(
      name: null,
      refId: null,
      type: ChatRoomType.Group,
      initialMembers: null,
      setOwner: false,
      isPublic: false,
      tags: null,
      allowedRoles: null,
      allowedOrgUnits: null,
      defaultRole: ChatMemberRole.Regular);

    result.ShouldNotBeNull();
    result.Name.ShouldBe("GroupChat #007");
  }

  [Fact]
  public async Task CreateChatAsync_ShouldTrimNameAndAssignPermissions()
  {
    ChatRoomEntity captured = null;
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.InsertAsync(Arg.Any<ChatRoomEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo =>
      {
        captured = callInfo.Arg<ChatRoomEntity>();
        return captured;
      });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatRoomDomainService(
      chatMemberDomainService: chatMemberDomainService,
      chatRoomRepository: chatRoomRepository);

    var allowedRoles = new List<string> { "Admin" };
    var allowedOrgUnits = new List<Guid> { Guid.Parse("22222222-2222-2222-2222-222222222222") };
    var tags = new List<string> { "alpha", string.Empty, "beta" };

    var result = await service.CreateChatAsync(
      name: "  Team Chat  ",
      refId: "ref-1",
      type: ChatRoomType.Group,
      initialMembers: new Dictionary<Guid, ChatMemberRole?>(),
      setOwner: false,
      isPublic: true,
      tags: tags,
      allowedRoles: allowedRoles,
      allowedOrgUnits: allowedOrgUnits,
      defaultRole: ChatMemberRole.Administrator);

    result.ShouldNotBeNull();
    captured.ShouldNotBeNull();
    captured.Name.ShouldBe("Team Chat");
    captured.RefId.ShouldBe("ref-1");
    captured.IsPublic.ShouldBeTrue();
    captured.DefaultRole.ShouldBe(ChatMemberRole.Administrator);
    captured.GetTags().ShouldBe(new List<string> { "alpha", "beta" });
    captured.ViewChatPermissions.Count.ShouldBe(2);
  }

  [Fact]
  public async Task RenameChat_ShouldThrow_WhenNewNameIsEmpty()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var userManager = CreateUserManager(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Administrator)
      });

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      userManager: userManager,
      chatMemberRepository: chatMemberRepository);

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();

    var service = CreateChatRoomDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      chatRoomRepository: chatRoomRepository);

    await Should.ThrowAsync<Exception>(async () => await service.RenameChat(chatId, "  "));

    await chatRoomRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default, default, default);
  }

  [Fact]
  public async Task CloseAsync_ShouldSetStatusToClosed()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var userManager = CreateUserManager(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Owner)
      });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group);
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);
    chatRoomRepository.UpdateAsync(chat, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var userChatSettingRepository = Substitute.For<IUserChatSettingRepository>();
    userChatSettingRepository.GetMutedUsers(chatId).Returns(new List<Guid>());
    userChatSettingRepository.UnarchiveChatAsync(chatId, Arg.Any<List<Guid>>()).Returns(0);

    var chatMessagePushManager = CreateChatMessagePushManager(
      userChatSettingRepository: userChatSettingRepository,
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository,
      pushManager: chatMessagePushManager);

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      userManager: userManager,
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      chatMessageDomainService: chatMessageDomainService);

    var service = CreateChatRoomDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.CloseAsync(chatId);

    chat.Status.ShouldBe(ChatRoomStatus.Closed);
  }

  [Fact]
  public async Task RenameChat_ShouldUpdateName_WhenValidNameProvided()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var userManager = CreateUserManager(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Administrator)
      });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { Name = "Old Name" };
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);
    chatRoomRepository.UpdateAsync(chat, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      userManager: userManager,
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatRoomDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.RenameChat(chatId, "New Name");

    chat.Name.ShouldBe("New Name");
    await chatRoomRepository.Received(1).UpdateAsync(chat, Arg.Any<bool>(), Arg.Any<CancellationToken>());
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task GetChatsList_ShouldReturnChats_WhenValid()
  {
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chats = new List<ChatRoomEntity>
    {
      new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.Group) { Name = "Chat 1" },
      new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.Group) { Name = "Chat 2" }
    };
    chatRoomRepository.GetChatsList(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>())
      .Returns(new KeyValuePair<int, List<ChatRoomEntity>>(2, chats));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByChats(Arg.Any<List<Guid>>())
      .Returns(new Dictionary<Guid, KeyValuePair<int, List<ChatMemberEntity>>>());

    var chatUserHelper = new ChatUserHelperService(CreateUserManager(Guid.NewGuid(), "user"), chatMemberRepository);

    var service = CreateChatRoomDomainService(
      chatRoomRepository: chatRoomRepository,
      chatUserHelper: chatUserHelper);

    var result = await service.GetChatsList("Name", 10, 0, null);

    result.Key.ShouldBe(2);
    result.Value.Count.ShouldBe(2);
    result.Value[0].Name.ShouldBe("Chat 1");
    result.Value[1].Name.ShouldBe("Chat 2");
  }

  [Fact]
  public async Task GetChatsByOwner_ShouldReturnChats_WhenValid()
  {
    var userId = Guid.NewGuid();
    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chats = new List<ChatRoomEntity>
    {
      new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.Group) { Name = "My Chat" }
    };
    chatRoomRepository.GetChatsByOwner(userId, Arg.Any<List<ChatRoomType>>())
      .Returns(chats);

    var service = CreateChatRoomDomainService(
      chatRoomRepository: chatRoomRepository);

    var result = await service.GetChatsByOwner(userId, new List<ChatRoomType> { ChatRoomType.Group });

    result.ShouldNotBeNull();
    result.Count.ShouldBe(1);
    result[0].Name.ShouldBe("My Chat");
  }

  [Fact]
  public async Task UpdateChatAsync_ShouldUpdateChat_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var userManager = CreateUserManager(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Administrator)
      });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { Name = "Old Name" };
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);
    chatRoomRepository.UpdateAsync(chat, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(chat);

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      userManager: userManager,
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatRoomDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    var tags = new List<string> { "tag1", "tag2" };
    var result = await service.UpdateChatAsync(
      chatId,
      "New Name",
      tags,
      isPublic: true,
      defaultRole: ChatMemberRole.Administrator);

    result.ShouldNotBeNull();
    result.Name.ShouldBe("New Name");
    result.IsPublic.ShouldBeTrue();
    result.DefaultRole.ShouldBe(ChatMemberRole.Administrator);
    result.GetTags().ShouldBe(tags);
    await chatRoomRepository.Received(1).UpdateAsync(chat, Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }
}
