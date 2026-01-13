using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using Common.EventBus.Module;
using Messaging.Module.Messages;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.SupportTickets;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class SupportTicketAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task CreateSupportTicket_ShouldThrow_WhenDomainFails()
  {
    var eventBus = NSubstitute.Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    ((IResponseCapableEventBus)eventBus).RequestAsync<DocumentSeriaNumberGotMsg>(NSubstitute.Arg.Any<object>(), NSubstitute.Arg.Any<int>())
      .Returns(Task.FromException<DocumentSeriaNumberGotMsg>(new Exception("domain failure")));

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.InsertAsync(NSubstitute.Arg.Any<ChatRoomEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatRoomEntity>());

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(NSubstitute.Arg.Any<List<ChatMemberEntity>>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatRoomDomainService = CreateChatRoomDomainService(
      chatMemberDomainService: CreateChatMemberDomainService(chatMemberRepository: chatMemberRepository),
      chatRoomRepository: chatRoomRepository);

    var domainService = CreateSupportTicketDomainService(
      eventBus: eventBus,
      chatRoomDomainService: chatRoomDomainService);

    var appService = new SupportTicketAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<SupportTicketAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(Guid.NewGuid(), "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var request = new CreateSupportTicketRequestDto
    {
      Title = "Issue",
      InitialMembers = new List<Guid>()
    };

    var result = await appService.CreateSupportTicket(request);

    result.ShouldBeNull();
  }

  [Fact]
  public async Task CloseSupportTicket_ShouldReturnTrue_WhenSuccess()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Administrator) });

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, NSubstitute.Arg.Any<bool>(), NSubstitute.Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.SupportTicket));

    var messageRepository = NSubstitute.Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(NSubstitute.Arg.Any<ChatMessageEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatRoomRepository: chatRoomRepository,
      messageRepository: messageRepository);

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      userManager: CreateUserManager(userId, "user"),
      chatMemberRepository: chatMemberRepository,
      chatRoomRepository: chatRoomRepository,
      chatMessageDomainService: chatMessageDomainService);

    var domainService = CreateSupportTicketDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatMemberDomainService: chatMemberDomainService,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    var appService = new SupportTicketAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<SupportTicketAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(userId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var result = await appService.CloseSupportTicket(chatId);

    result.ShouldBeTrue();
  }
}
