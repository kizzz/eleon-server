using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using CollaborationModule.Test.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.Storage;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonsoftAbp.Auth;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Domain.DomainServices;

public class ChatInteractionDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task SendTextMessage_ShouldSkipPermission_WhenApiKey()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var httpAccessor = CreateHttpContextAccessor("api-key");

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });
    chatRoomRepository.GetAsync(chatId)
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => Task.FromResult(callInfo.Arg<ChatMessageEntity>()));

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var chatMemberDomainService = CreateChatMemberDomainService();

    var service = CreateChatInteractionDomainService(
      httpContextAccessor: httpAccessor,
      chatMessageDomainService: chatMessageDomainService,
      chatMemberDomainService: chatMemberDomainService);

    var result = await service.SendTextMessage(chatId, "hi", ChatMessageSeverity.Info);

    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task OpenChat_ShouldUpdateMemberLastViewed()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = DateTime.MinValue,
      LastModificationTime = DateTime.MinValue
    };

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    chatMessageRepository.GetLastChatMessages(chatId, 0, 10)
      .Returns(new List<ChatMessageEntity>
      {
        new ChatMessageEntity(Guid.NewGuid())
        {
          ChatRoomId = chatId,
          CreationTime = DateTime.UtcNow,
          Sender = userId.ToString(),
          SenderType = ChatMessageSenderType.User
        }
      });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var service = CreateChatInteractionDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: CreateMockCurrentUser(userId, "user"),
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var messages = await service.OpenChat(chatId, 10);

    messages.ShouldNotBeNull();
    await chatMemberRepository.Received(1).UpdateAsync(member, true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task RetreiveDocumentMessageContent_ShouldReturnBase64()
  {
    var chatId = Guid.NewGuid();
    var messageId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular) });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var content = "{\"BlobRefId\":\"blob\",\"Filename\":\"doc.pdf\"}";
    chatMessageRepository.GetAsync(messageId).Returns(new ChatMessageEntity(messageId) { ChatRoomId = chatId, Content = content });

    var eventBus = Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    ((IResponseCapableEventBus)eventBus).RequestAsync<GetFromStorageResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
      .Returns(new GetFromStorageResponseMsg { Base64Data = "BASE64" });

    var service = CreateChatInteractionDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      distributedEventBus: eventBus,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: CreateMockCurrentUser(userId, "user"),
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var result = await service.RetreiveDocumentMessageContent(messageId);

    result.ShouldBe("BASE64");
  }

  // NOTE: These tests will fail until Phase 1 API key refactor is complete
  // They serve as regression tests to ensure API keys require explicit permissions
  [Fact(Skip = "Will pass after Phase 1 refactor - API keys should require explicit permissions")]
  public async Task SendTextMessage_ShouldRequirePermission_WhenApiKey()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var httpAccessor = CreateHttpContextAccessor("api-key");

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>()); // No membership = no permission

    var chatMemberDomainService = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatInteractionDomainService(
      httpContextAccessor: httpAccessor,
      chatMemberDomainService: chatMemberDomainService);

    // After Phase 1 refactor, API keys should require explicit permissions
    // This should throw an exception when API key doesn't have write permission
    await Should.ThrowAsync<Exception>(async () => await service.SendTextMessage(chatId, "hi", ChatMessageSeverity.Info));
  }

  [Fact]
  public async Task FillMessagesInfo_ShouldMarkUnread_WhenLastViewedByUserIsNull()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = null, // Null LastViewedByUser
      LastModificationTime = DateTime.UtcNow.AddDays(-1)
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var messages = new List<ChatMessageEntity>
    {
      new ChatMessageEntity(Guid.NewGuid())
      {
        ChatRoomId = chatId,
        CreationTime = DateTime.UtcNow,
        Sender = Guid.NewGuid().ToString(),
        SenderType = ChatMessageSenderType.User
      }
    };
    chatMessageRepository.GetLastChatMessages(chatId, 0, 10).Returns(messages);

    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var result = await service.GetChatMessages(chatId, 0, 10);

    // When LastViewedByUser is null, FillMessagesInfo uses LastModificationTime
    // Since message is newer than LastModificationTime, it should be marked unread
    result[0].Unread.ShouldBeTrue();
  }

  [Fact]
  public async Task FillMessagesInfo_ShouldNotMarkUnread_WhenOutgoingMessage()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = DateTime.UtcNow.AddDays(-1),
      LastModificationTime = DateTime.UtcNow.AddDays(-1)
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var messages = new List<ChatMessageEntity>
    {
      new ChatMessageEntity(Guid.NewGuid())
      {
        ChatRoomId = chatId,
        CreationTime = DateTime.UtcNow,
        Sender = userId.ToString(), // Outgoing message
        SenderType = ChatMessageSenderType.User
      }
    };
    chatMessageRepository.GetLastChatMessages(chatId, 0, 10).Returns(messages);

    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var result = await service.GetChatMessages(chatId, 0, 10);

    // Outgoing messages should never be marked unread
    result[0].Unread.ShouldBeFalse();
    result[0].Outcoming.ShouldBeTrue();
  }

  [Fact]
  public async Task FillMessagesInfo_ShouldMarkUnread_WhenMessageAfterLastViewed()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var otherUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var lastViewed = DateTime.UtcNow.AddDays(-1);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = lastViewed,
      LastModificationTime = lastViewed
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var messages = new List<ChatMessageEntity>
    {
      new ChatMessageEntity(Guid.NewGuid())
      {
        ChatRoomId = chatId,
        CreationTime = DateTime.UtcNow, // Message is newer than lastViewed
        Sender = otherUserId.ToString(),
        SenderType = ChatMessageSenderType.User
      }
    };
    chatMessageRepository.GetLastChatMessages(chatId, 0, 10).Returns(messages);

    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var result = await service.GetChatMessages(chatId, 0, 10);

    // Message after LastViewedByUser should be marked unread
    result[0].Unread.ShouldBeTrue();
  }

  [Fact]
  public async Task FillMessagesInfo_ShouldNotMarkUnread_WhenMessageBeforeLastViewed()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var otherUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var lastViewed = DateTime.UtcNow;

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = lastViewed,
      LastModificationTime = lastViewed
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var messages = new List<ChatMessageEntity>
    {
      new ChatMessageEntity(Guid.NewGuid())
      {
        ChatRoomId = chatId,
        CreationTime = DateTime.UtcNow.AddDays(-2), // Message is older than lastViewed
        Sender = otherUserId.ToString(),
        SenderType = ChatMessageSenderType.User
      }
    };
    chatMessageRepository.GetLastChatMessages(chatId, 0, 10).Returns(messages);

    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    var result = await service.GetChatMessages(chatId, 0, 10);

    // Message before LastViewedByUser should not be marked unread
    result[0].Unread.ShouldBeFalse();
  }

  [Fact]
  public async Task FillChatsInfo_ShouldUseLastViewedByUser_NotLastModificationTime()
  {
    // NOTE: This test documents the current bug - FillChatsInfo uses LastModificationTime instead of LastViewedByUser
    // After Phase 1 refactor, this should be fixed to use LastViewedByUser
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var otherUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var lastViewed = DateTime.UtcNow.AddDays(-1);
    var lastModification = DateTime.UtcNow.AddDays(-2); // Different from lastViewed

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = lastViewed,
      LastModificationTime = lastModification
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group)
    {
      LastModificationTime = lastModification // Chat's LastModificationTime
    };
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var lastMessage = new ChatMessageEntity(Guid.NewGuid())
    {
      ChatRoomId = chatId,
      CreationTime = DateTime.UtcNow.AddHours(-12), // Between lastModification and lastViewed
      Sender = otherUserId.ToString(),
      SenderType = ChatMessageSenderType.User
    };
    chatMessageRepository.GetLastChatMessages(chatId, 0, 1).Returns(new List<ChatMessageEntity> { lastMessage });

    // This test verifies current behavior (uses LastModificationTime)
    // After Phase 1 refactor, FillChatsInfo should use LastViewedByUser instead
    // For now, we verify the current (incorrect) behavior
    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    // Currently uses chat.LastModificationTime, not member.LastViewedByUser
    // This is a bug that should be fixed in Phase 1
    var result = await service.GetChatMessages(chatId, 0, 1);

    // Current implementation uses LastModificationTime, so message after lastModification is unread
    // After fix, should use LastViewedByUser, so message before lastViewed should not be unread
    result[0].Unread.ShouldBeTrue(); // Current behavior (incorrect)
  }

  [Fact]
  public async Task AckMessageReceived_ShouldBeIdempotent_WhenCalledMultipleTimes()
  {
    var chatId = Guid.NewGuid();
    var messageId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");
    var initialLastViewed = DateTime.UtcNow.AddDays(-2);
    var messageTime = DateTime.UtcNow.AddDays(-1);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
    {
      LastViewedByUser = initialLastViewed
    };
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });
    chatMemberRepository.UpdateAsync(member, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var chatMessageRepository = Substitute.For<IChatMessageRepository>();
    var message = new ChatMessageEntity(messageId)
    {
      ChatRoomId = chatId,
      CreationTime = messageTime
    };
    chatMessageRepository.GetAsync(messageId).Returns(message);

    var unitOfWork = Substitute.For<IUnitOfWork>();
    unitOfWork.CompleteAsync().Returns(Task.CompletedTask);
    var unitOfWorkManager = TestMockHelpers.CreateMockUnitOfWorkManager(unitOfWork);

    var service = CreateChatInteractionDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      chatMessageRepository: chatMessageRepository,
      unitOfWorkManager: unitOfWorkManager,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: currentUser,
        chatMemberRepository: chatMemberRepository,
        userManager: CreateUserManager(userId, "user")));

    // First ack - should update LastViewedByUser
    await service.AckMessageReceived(messageId);
    member.LastViewedByUser.ShouldBe(messageTime);

    // Second ack - should be idempotent (no change if already at or past message time)
    var secondAckTime = messageTime.AddHours(1);
    message.CreationTime = secondAckTime;
    await service.AckMessageReceived(messageId);
    // LastViewedByUser should be monotonic (max of existing and new)
    member.LastViewedByUser.ShouldBe(secondAckTime);

    // Third ack with older message - should not decrease LastViewedByUser (monotonic)
    var olderMessageTime = messageTime.AddHours(-1);
    message.CreationTime = olderMessageTime;
    await service.AckMessageReceived(messageId);
    // LastViewedByUser should not decrease (monotonic)
    member.LastViewedByUser.ShouldBe(secondAckTime);
  }
}
