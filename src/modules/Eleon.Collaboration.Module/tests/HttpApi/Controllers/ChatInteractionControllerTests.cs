using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.ChatInteractions;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.Controllers;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class ChatInteractionControllerTests
{
  [Fact]
  public async Task GetChatMessages_ShouldReturnResponse()
  {
    var appService = Substitute.For<IChatInteractionAppService>();
    var list = new List<ChatMessageDto> { new ChatMessageDto { Id = Guid.NewGuid() } };
    appService.GetChatMessages(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>()).Returns(list);

    var controller = new ChatInteractionController(appService, TestMockHelpers.CreateMockLogger<ChatInteractionController>());

    var result = await controller.GetChatMessages(Guid.NewGuid(), 0, 10);

    result.ShouldBe(list);
  }

  [Fact]
  public async Task GetLastChats_ShouldReturnResponse()
  {
    var appService = Substitute.For<IChatInteractionAppService>();
    var dto = new PagedResultDto<UserChatInfoDto>(0, new List<UserChatInfoDto>());
    appService.GetLastChats(Arg.Any<LastChatsRequestDto>()).Returns(dto);

    var controller = new ChatInteractionController(appService, TestMockHelpers.CreateMockLogger<ChatInteractionController>());

    var result = await controller.GetLastChats(new LastChatsRequestDto());

    result.ShouldBe(dto);
  }
}
