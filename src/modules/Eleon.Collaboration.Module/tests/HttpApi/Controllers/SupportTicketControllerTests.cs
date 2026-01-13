using System;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.Controllers;
using VPortal.Collaboration.Feature.Module.SupportTickets;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class SupportTicketControllerTests
{
  [Fact]
  public async Task CloseSupportTicket_ShouldReturnResponse()
  {
    var appService = Substitute.For<ISupportTicketAppService>();
    appService.CloseSupportTicket(Arg.Any<Guid>()).Returns(true);

    var controller = new SupportTicketController(appService, TestMockHelpers.CreateMockLogger<SupportTicketController>());

    var result = await controller.CloseSupportTicket(Guid.NewGuid());

    result.ShouldBeTrue();
  }

  [Fact]
  public async Task CreateSupportTicket_ShouldReturnResponse()
  {
    var appService = Substitute.For<ISupportTicketAppService>();
    var dto = new ChatRoomDto { Id = Guid.NewGuid() };
    appService.CreateSupportTicket(Arg.Any<CreateSupportTicketRequestDto>()).Returns(dto);

    var controller = new SupportTicketController(appService, TestMockHelpers.CreateMockLogger<SupportTicketController>());

    var result = await controller.CreateSupportTicket(new CreateSupportTicketRequestDto());

    result.ShouldBe(dto);
  }
}
