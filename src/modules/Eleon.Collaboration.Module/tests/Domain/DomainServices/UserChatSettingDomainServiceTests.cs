using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Domain.DomainServices;

public class UserChatSettingDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task SetChatMute_ShouldCreateSetting_WhenMissing()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
      });

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: CreateUserManager(userId, "user"));

    var settingRepository = Substitute.For<IUserChatSettingRepository>();
    settingRepository.GetChatSettingAsync(userId, chatId).Returns((UserChatSettingEntity)null);
    settingRepository.InsertAsync(Arg.Any<UserChatSettingEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<UserChatSettingEntity>());

    var service = CreateUserChatSettingDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      settingRepository: settingRepository);

    await service.SetChatMute(chatId, true);

    await settingRepository.Received(1).InsertAsync(Arg.Any<UserChatSettingEntity>(), true, Arg.Any<CancellationToken>());
    await settingRepository.Received(1).UpdateAsync(Arg.Any<UserChatSettingEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task SetChatArchivedAsync_ShouldUpdateExistingSetting()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular)
      });

    var chatMemberDomainService = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: CreateUserManager(userId, "user"));

    var existing = new UserChatSettingEntity(Guid.NewGuid(), userId, chatId) { IsArchived = false };

    var settingRepository = Substitute.For<IUserChatSettingRepository>();
    settingRepository.GetChatSettingAsync(userId, chatId).Returns(existing);

    var service = CreateUserChatSettingDomainService(
      currentUser: currentUser,
      chatMemberDomainService: chatMemberDomainService,
      settingRepository: settingRepository);

    await service.SetChatArchivedAsync(chatId, true);

    existing.IsArchived.ShouldBeTrue();
    await settingRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
    await settingRepository.Received(1).UpdateAsync(existing, true, Arg.Any<CancellationToken>());
  }
}
