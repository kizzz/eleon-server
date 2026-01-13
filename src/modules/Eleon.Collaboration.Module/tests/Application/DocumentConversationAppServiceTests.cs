using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.DocumentConversations;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class DocumentConversationAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task GetDocumentConversationInfo_ShouldReturnNull_WhenDomainFails()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetByRefId(NSubstitute.Arg.Any<string>())
      .Returns(Task.FromException<ChatRoomEntity>(new Exception("domain failure")));

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Regular) });

    var domainService = CreateDocumentConversationDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: CreateMockCurrentUser(currentUserId, "user"),
        userManager: CreateUserManager(currentUserId, "user"),
        chatMemberRepository: chatMemberRepository));

    var appService = new DocumentConversationAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<DocumentConversationAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var result = await appService.GetDocumentConversationInfo("Doc", "1");

    result.ShouldBeNull();
  }
}
