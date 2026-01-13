using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.ChatMembers;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class ChatMemberAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task AddChatMembers_ShouldReturnTrue_WhenSuccess()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, NSubstitute.Arg.Any<bool>(), NSubstitute.Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Regular) });
    chatMemberRepository.InsertManyAsync(NSubstitute.Arg.Any<List<ChatMemberEntity>>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var messageRepository = NSubstitute.Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(NSubstitute.Arg.Any<ChatMessageEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var domainService = CreateChatMemberDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      userManager: CreateUserManager(currentUserId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      chatMessageDomainService: chatMessageDomainService);

    var appService = new ChatMemberAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<ChatMemberAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var request = new AddChatMembersRequestDto
    {
      ChatId = chatId,
      UserIds = new List<Guid> { Guid.NewGuid() }
    };

    var result = await appService.AddChatMembers(request);

    result.ShouldBeTrue();
  }

  [Fact]
  public async Task JoinChat_ShouldReturnTrue_WhenSuccess()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, NSubstitute.Arg.Any<bool>(), NSubstitute.Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true });

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, currentUserId).Returns((ChatMemberEntity)null);
    chatMemberRepository.InsertAsync(NSubstitute.Arg.Any<ChatMemberEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMemberEntity>());

    var messageRepository = NSubstitute.Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(NSubstitute.Arg.Any<ChatMessageEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var domainService = CreateChatMemberDomainService(
      currentUser: CreateMockCurrentUser(currentUserId, "user"),
      userManager: CreateUserManager(currentUserId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      chatMessageDomainService: chatMessageDomainService);

    var appService = new ChatMemberAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<ChatMemberAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var result = await appService.JoinChat(chatId);

    result.ShouldBeTrue();
  }
}
