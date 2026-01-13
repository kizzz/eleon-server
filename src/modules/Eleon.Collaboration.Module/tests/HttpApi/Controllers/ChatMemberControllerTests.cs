using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.ChatMembers;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Controllers;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class ChatMemberControllerTests
{
  [Fact]
  public async Task AddChatMembers_ShouldReturnResponse()
  {
    var appService = Substitute.For<IChatMemberAppService>();
    appService.AddChatMembers(Arg.Any<AddChatMembersRequestDto>()).Returns(true);

    var controller = new ChatMemberController(appService, TestMockHelpers.CreateMockLogger<ChatMemberController>());

    var result = await controller.AddChatMembers(new AddChatMembersRequestDto());

    result.ShouldBeTrue();
  }

  [Fact]
  public async Task GetChatMembers_ShouldReturnResponse()
  {
    var appService = Substitute.For<IChatMemberAppService>();
    var list = new List<ChatMemberInfo> { new ChatMemberInfo { Id = Guid.NewGuid() } };
    appService.GetChatMembers(Arg.Any<Guid>()).Returns(list);

    var controller = new ChatMemberController(appService, TestMockHelpers.CreateMockLogger<ChatMemberController>());

    var result = await controller.GetChatMembers(Guid.NewGuid());

    result.ShouldBe(list);
  }
}
