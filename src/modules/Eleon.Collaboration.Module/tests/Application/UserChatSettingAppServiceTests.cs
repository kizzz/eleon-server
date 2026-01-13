using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.UserChatSettings;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Application;

public class UserChatSettingAppServiceTests : AppServiceTestBase
{
  [Fact]
  public async Task SetChatMute_ShouldReturnTrue_WhenSuccess()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatMemberRepository = NSubstitute.Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, NSubstitute.Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular) });

    var settingRepository = NSubstitute.Substitute.For<IUserChatSettingRepository>();
    settingRepository.GetChatSettingAsync(userId, chatId).Returns((UserChatSettingEntity)null);
    settingRepository.InsertAsync(NSubstitute.Arg.Any<UserChatSettingEntity>(), true, NSubstitute.Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<UserChatSettingEntity>());

    var domainService = CreateUserChatSettingDomainService(
      currentUser: CreateMockCurrentUser(userId, "user"),
      chatMemberDomainService: CreateChatMemberDomainService(
        currentUser: CreateMockCurrentUser(userId, "user"),
        userManager: CreateUserManager(userId, "user"),
        chatMemberRepository: chatMemberRepository),
      settingRepository: settingRepository);

    var appService = new UserChatSettingAppService(
      Eleon.TestsBase.Lib.TestHelpers.TestMockHelpers.CreateMockLogger<UserChatSettingAppService>(),
      domainService);

    var mapper = CreateCollaborationObjectMapper();
    var currentUser = CreateMockCurrentUser(userId, "user");
    SetAppServiceDependencies(appService, mapper, currentUser);

    var result = await appService.SetChatMute(chatId, true);

    result.ShouldBeTrue();
  }
}
