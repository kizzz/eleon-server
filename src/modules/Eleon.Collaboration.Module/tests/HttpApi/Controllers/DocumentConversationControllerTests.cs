using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Controllers;
using VPortal.Collaboration.Feature.Module.DocumentConversations;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.DocumentConversations;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class DocumentConversationControllerTests
{
  [Fact]
  public async Task GetDocumentConversationInfo_ShouldReturnResponse()
  {
    var appService = Substitute.For<IDocumentConversationAppService>();
    var dto = new DocumentConversationInfoDto();
    appService.GetDocumentConversationInfo(Arg.Any<string>(), Arg.Any<string>()).Returns(dto);

    var controller = new DocumentConversationController(appService, TestMockHelpers.CreateMockLogger<DocumentConversationController>());

    var result = await controller.GetDocumentConversationInfo("Doc", "1");

    result.ShouldBe(dto);
  }

  [Fact]
  public async Task SendDocumentChatMessagesAsync_ShouldInvokeAppService()
  {
    var appService = Substitute.For<IDocumentConversationAppService>();
    var controller = new DocumentConversationController(appService, TestMockHelpers.CreateMockLogger<DocumentConversationController>());

    await controller.SendDocumentChatMessagesAsync(new List<DocumentChatMessageDto>());

    await appService.Received(1).SendDocumentChatMessagesAsync(Arg.Any<List<DocumentChatMessageDto>>());
  }
}
