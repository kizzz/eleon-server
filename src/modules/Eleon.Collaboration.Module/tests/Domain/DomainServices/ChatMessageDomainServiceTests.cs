using System;
using System.Threading;
using CollaborationModule.Test.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.Storage;
using Eleon.TestsBase.Lib.TestHelpers;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Domain.DomainServices;

public class ChatMessageDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task AddTextMessage_ShouldReturnMessage_WhenChatIsOpen()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddTextMessage(chatId, "hello", ChatMessageSeverity.Info);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    result.Content.ShouldBe("hello");
  }

  [Fact]
  public async Task AddTextMessage_ShouldThrow_WhenChatIsClosed()
  {
    var chatId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Closed });

    var messageRepository = Substitute.For<IChatMessageRepository>();

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    await Should.ThrowAsync<Exception>(async () => await service.AddTextMessage(chatId, "hello", ChatMessageSeverity.Info));
    await messageRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task AddDocumentMessage_ShouldThrow_WhenStorageFails()
  {
    var chatId = Guid.NewGuid();

    var eventBus = CreateMockResponseCapableEventBus();
    SetupEventBusRequestAsync<object, SaveToStorageResponseMsg>(
      eventBus, 
      new SaveToStorageResponseMsg { Success = false });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository,
      eventBus: (IDistributedEventBus)eventBus);

    await Should.ThrowAsync<Exception>(async () => await service.AddDocumentMessage(chatId, "doc.pdf", "ZmFrZQ=="));
    await messageRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task AddDocumentMessage_ShouldReturnMessage_WhenStorageSucceeds()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var eventBus = CreateMockResponseCapableEventBus();
    SetupEventBusRequestAsync<object, SaveToStorageResponseMsg>(
      eventBus,
      new SaveToStorageResponseMsg { Success = true });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var guidGenerator = CreateMockGuidGenerator();
    var service = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository,
      eventBus: (IDistributedEventBus)eventBus,
      guidGenerator: guidGenerator);

    var result = await service.AddDocumentMessage(chatId, "doc.pdf", "ZmFrZQ==");

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddTextMessage_ShouldReturnMessage_WhenContentIsEmpty()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddTextMessage(chatId, "", ChatMessageSeverity.Info);

    result.ShouldNotBeNull();
    result.Content.ShouldBe("");
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddTextMessage_ShouldThrow_WhenChatNotFound()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns((ChatRoomEntity)null);

    var messageRepository = Substitute.For<IChatMessageRepository>();

    var service = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    await Should.ThrowAsync<Exception>(async () => await service.AddTextMessage(chatId, "hello", ChatMessageSeverity.Info));
    await messageRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task AddMembersAddedMessage_ShouldReturnMessage_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var addedByUserId = Guid.NewGuid();
    var addedUsersIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddMembersAddedMessage(chatId, addedByUserId, addedUsersIds);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddMemberJoinedMessage_ShouldReturnMessage_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var joinedUserId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddMemberJoinedMessage(chatId, joinedUserId);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddMembersKickedMessage_ShouldReturnMessage_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var kickedByUserId = Guid.NewGuid();
    var kickedUsersIds = new List<Guid> { Guid.NewGuid() };

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddMembersKickedMessage(chatId, kickedByUserId, kickedUsersIds);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddChatCreatedMessage_ShouldReturnMessage_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var createdByUserId = Guid.NewGuid();
    var creationTime = DateTime.UtcNow;

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddChatCreatedMessage(chatId, createdByUserId, creationTime);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task AddUserLeftMessage_ShouldReturnMessage_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var service = CreateChatMessageDomainService(
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var result = await service.AddUserLeftMessage(chatId, userId);

    result.ShouldNotBeNull();
    result.ChatRoomId.ShouldBe(chatId);
    await messageRepository.Received(1).InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>());
  }
}
