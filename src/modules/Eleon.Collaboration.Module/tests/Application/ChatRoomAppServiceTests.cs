using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class ChatRoomAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task CreateChatAsync_ShouldReturnNull_WhenDomainFails()
  {
    var chatRoomRepository = NSubstitute.Substitute.For<IChatRoomRepository>();
    chatRoomRepository.InsertAsync(NSubstitute.Arg.Any<ChatRoomEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(Task.FromException<ChatRoomEntity>(new Exception("domain failure")));

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.InsertManyAsync(NSubstitute.Arg.Any<List<ChatMemberEntity>>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var domainService = CreateChatRoomDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberDomainService: CreateChatMemberDomainService(chatMemberRepository: chatMemberRepository));

    var appService = new ChatRoomAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<ChatRoomAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(Guid.NewGuid(), "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var request = new CreateChatRequestDto
    {
      ChatName = "Team",
      InitialMembers = new Dictionary<Guid, ChatMemberRole?>(),
      SetOwner = false,
      IsPublic = false
    };

    var result = await appService.CreateChatAsync(request);

    result.ShouldBeNull();
  }

  
}
