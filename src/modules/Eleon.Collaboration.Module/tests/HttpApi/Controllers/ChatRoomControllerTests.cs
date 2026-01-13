using System;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.Controllers;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class ChatRoomControllerTests
{
  [Fact]
  public async Task CreateChatAsync_ShouldReturnAppServiceResponse()
  {
    var appService = Substitute.For<IChatRoomAppService>();
    var dto = new ChatRoomDto { Id = Guid.NewGuid(), Name = "Chat" };
    appService.CreateChatAsync(Arg.Any<CreateChatRequestDto>()).Returns(dto);

    var controller = new ChatRoomController(appService, TestMockHelpers.CreateMockLogger<ChatRoomController>());

    var result = await controller.CreateChatAsync(new CreateChatRequestDto { ChatName = "Chat" });

    result.ShouldBe(dto);
  }

  [Fact]
  public async Task GetChatsList_ShouldReturnResponse()
  {
    var appService = Substitute.For<IChatRoomAppService>();
    var dto = new PagedResultDto<ChatRoomDto>(1, new[] { new ChatRoomDto { Id = Guid.NewGuid() } });
    appService.GetChatsList(Arg.Any<ChatListRequestDto>()).Returns(dto);

    var controller = new ChatRoomController(appService, TestMockHelpers.CreateMockLogger<ChatRoomController>());

    var result = await controller.GetChatsList(new ChatListRequestDto());

    result.ShouldBe(dto);
  }
}
