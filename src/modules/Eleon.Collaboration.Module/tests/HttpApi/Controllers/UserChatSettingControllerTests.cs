using System;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Controllers;
using VPortal.Collaboration.Feature.Module.UserChatSettings;

namespace CollaborationModule.Test.HttpApi.Controllers;

public class UserChatSettingControllerTests
{
  [Fact]
  public async Task SetChatMute_ShouldReturnResponse()
  {
    var appService = Substitute.For<IUserChatSettingAppService>();
    appService.SetChatMute(Arg.Any<Guid>(), Arg.Any<bool>()).Returns(true);

    var controller = new UserChatSettingController(appService, TestMockHelpers.CreateMockLogger<UserChatSettingController>());

    var result = await controller.SetChatMute(Guid.NewGuid(), true);

    result.ShouldBeTrue();
  }

  [Fact]
  public async Task SetChatArchivedAsync_ShouldReturnResponse()
  {
    var appService = Substitute.For<IUserChatSettingAppService>();
    appService.SetChatArchivedAsync(Arg.Any<Guid>(), Arg.Any<bool>()).Returns(true);

    var controller = new UserChatSettingController(appService, TestMockHelpers.CreateMockLogger<UserChatSettingController>());

    var result = await controller.SetChatArchivedAsync(Guid.NewGuid(), true);

    result.ShouldBeTrue();
  }
}
