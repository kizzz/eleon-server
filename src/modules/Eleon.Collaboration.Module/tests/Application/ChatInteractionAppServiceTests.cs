using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.ChatInteractions;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class ChatInteractionAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task SendTextMessage_ShouldReturnNull_WhenDomainFails()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, NSubstitute.Arg.Any<bool>(), NSubstitute.Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(Task.FromException<List<ChatMemberEntity>>(new Exception("domain failure")));

    var messageRepository = NSubstitute.Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(NSubstitute.Arg.Any<ChatMessageEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      userManager: CreateUserManager(currentUserId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var domainService = CreateChatInteractionDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      chatRoomRepository: chatRoomRepository,
      chatMessageRepository: messageRepository,
      chatMemberRepository: chatMemberRepository,
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService);

    var appService = new ChatInteractionAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<ChatInteractionAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var request = new SendTextMessageRequestDto
    {
      ChatId = chatId,
      Message = "hello",
      Severity = ChatMessageSeverity.Info
    };

    var result = await appService.SendTextMessage(request);

    result.ShouldBeNull();
  }

  [Fact]
  public async Task GetChatMessages_ShouldReturnNull_WhenDomainFails()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, NSubstitute.Arg.Any<bool>(), NSubstitute.Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(Task.FromException<List<ChatMemberEntity>>(new Exception("domain failure")));

    var messageRepository = NSubstitute.Substitute.For<IChatMessageRepository>();
    messageRepository.GetLastChatMessages(chatId, 0, 10)
      .Returns(new List<ChatMessageEntity>
      {
        new ChatMessageEntity(Guid.NewGuid()) { ChatRoomId = chatId, Content = "one" },
        new ChatMessageEntity(Guid.NewGuid()) { ChatRoomId = chatId, Content = "two" }
      });

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      userManager: CreateUserManager(currentUserId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository);

    var domainService = CreateChatInteractionDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      chatRoomRepository: chatRoomRepository,
      chatMessageRepository: messageRepository,
      chatMemberRepository: chatMemberRepository,
      chatMemberDomainService: chatMemberDomainService);

    var appService = new ChatInteractionAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<ChatInteractionAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var result = await appService.GetChatMessages(chatId, 0, 10);

    result.ShouldBeNull();
  }
}
