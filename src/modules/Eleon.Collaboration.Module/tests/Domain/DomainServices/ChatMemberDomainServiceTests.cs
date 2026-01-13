using System;
using System.Collections.Generic;
using System.Threading;
using CollaborationModule.Test.TestBase;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Entities;
using NSubstitute;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace CollaborationModule.Test.Domain.DomainServices;

public class ChatMemberDomainServiceTests : DomainTestBase
{
  [Fact]
  public async Task AddMembers_ShouldThrow_ForPrivateChat()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Private));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Regular)
      });

    var userManager = CreateUserManager(currentUserId, "user");
    var currentUser = CreateMockCurrentUser(currentUserId, "user");

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      currentUser: currentUser);

    await Should.ThrowAsync<Exception>(async () => await service.AddMembers(chatId, new List<Guid> { userId }));
    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertManyAsync(default, default, default);
  }

  [Fact]
  public async Task AddMembers_ShouldThrow_WhenUserIdsEmpty()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Regular)
      });

    var userManager = CreateUserManager(currentUserId, "user");
    var currentUser = CreateMockCurrentUser(currentUserId, "user");

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      currentUser: currentUser);

    await Should.ThrowAsync<Exception>(async () => await service.AddMembers(chatId, new List<Guid>()));
    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertManyAsync(default, default, default);
  }

  [Fact]
  public async Task SetMemberRole_ShouldReject_WhenNonOwnerSetsAdmin()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var targetUserId = Guid.NewGuid();

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });

    var userManager = CreateUserManager(currentUserId, "admin");
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var service = CreateChatMemberDomainService(
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      currentUser: currentUser);

    await Should.ThrowAsync<Exception>(async () => await service.SetMemberRole(chatId, targetUserId, ChatMemberRole.Owner));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default, default, default);
  }

  [Fact]
  public async Task JoinChatAsync_ShouldReturn_WhenMemberAlreadyExists()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId)
      .Returns(new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular));

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository);

    await service.JoinChatAsync(chatId, userId);

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task JoinChatAsync_ShouldFail_WhenChatIsNotPublic()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = false });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);

    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user");

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager);

    await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () => await service.JoinChatAsync(chatId, userId));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task AddMembers_ShouldReturnMembers_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var newUserId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });
    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var userManager = CreateUserManager(currentUserId, "admin");
    userManager.AddUser(new Volo.Abp.Identity.IdentityUser(newUserId, "newuser", "newuser@example.com", null));
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      currentUser: currentUser,
      chatMessageDomainService: chatMessageDomainService);

    var result = await service.AddMembers(chatId, new List<Guid> { newUserId });

    result.ShouldNotBeNull();
    result.Count.ShouldBeGreaterThan(0);
    await chatMemberRepository.Received(1).InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task LeaveChat_ShouldRemoveMember_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular);
    // EnsureCurrentUserMembership is called first, which uses GetByMember
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });
    chatMemberRepository.DeleteAsync(Arg.Any<ChatMemberEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var userManager = CreateUserManager(userId, "user");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.LeaveChat(chatId, closeGroup: false);

    await chatMemberRepository.Received(1).DeleteAsync(Arg.Any<ChatMemberEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task KickMembers_ShouldRemoveMembers_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var kickedUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });
    var kickedMember = new ChatMemberEntity(Guid.NewGuid(), kickedUserId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByUser(chatId, kickedUserId).Returns(kickedMember);
    chatMemberRepository.DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var userManager = CreateUserManager(currentUserId, "admin");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.KickMembers(chatId, new List<Guid> { kickedUserId });

    await chatMemberRepository.Received(1).DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task CheckMembership_ShouldReturnTrue_WhenMemberExists()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var member = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { member });

    var userManager = CreateUserManager(userId, "user");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    var result = await service.CheckMembership(chatId);

    result.ShouldBeTrue();
  }

  [Fact]
  public async Task CheckMembership_ShouldReturnFalse_WhenMemberNotExists()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>());

    var userManager = CreateUserManager(userId, "user");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    var result = await service.CheckMembership(chatId);

    result.ShouldBeFalse();
  }

  [Fact]
  public async Task GetChatMembers_ShouldReturnMembers_WhenValid()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var userId2 = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(userId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var currentMember = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity> { currentMember });
    
    var allMembers = new List<ChatMemberEntity>
    {
      new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular) { Type = ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants.ChatMemberType.User },
      new ChatMemberEntity(Guid.NewGuid(), userId2.ToString(), chatId, ChatMemberRole.Administrator) { Type = ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants.ChatMemberType.User }
    };
    chatMemberRepository.GetByChat(chatId, Arg.Any<List<ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants.ChatMemberType>>())
      .Returns(allMembers);

    var userManager = CreateUserManager(userId, "user");
    userManager.AddUser(new Volo.Abp.Identity.IdentityUser(userId2, "user2", "user2@example.com", null));
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    var result = await service.GetChatMembers(chatId);

    result.ShouldNotBeNull();
    result.Count.ShouldBe(2);
  }

  [Fact]
  public async Task SetMemberRole_ShouldUpdateRole_WhenOwner()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var targetUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "owner");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Owner)
      });
    var targetMember = new ChatMemberEntity(Guid.NewGuid(), targetUserId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByUser(chatId, targetUserId).Returns(targetMember);
    chatMemberRepository.UpdateAsync(targetMember, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(targetMember);

    var userManager = CreateUserManager(currentUserId, "owner");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    await service.SetMemberRole(chatId, targetUserId, ChatMemberRole.Administrator);

    targetMember.Role.ShouldBe(ChatMemberRole.Administrator);
    await chatMemberRepository.Received(1).UpdateAsync(targetMember, Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task KickMembers_ShouldReject_WhenRegularMemberKicks()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var kickedUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "user");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Regular)
      });

    var userManager = CreateUserManager(currentUserId, "user");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    await Should.ThrowAsync<Exception>(async () => await service.KickMembers(chatId, new List<Guid> { kickedUserId }));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default, default, default);
  }

  [Fact]
  public async Task KickMembers_ShouldAllow_WhenAdminKicksRegular()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var kickedUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });
    var kickedMember = new ChatMemberEntity(Guid.NewGuid(), kickedUserId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByUser(chatId, kickedUserId).Returns(kickedMember);
    chatMemberRepository.DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var userManager = CreateUserManager(currentUserId, "admin");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.KickMembers(chatId, new List<Guid> { kickedUserId });

    await chatMemberRepository.Received(1).DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task KickMembers_ShouldReject_WhenAdminKicksOwner()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var kickedUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });
    var kickedMember = new ChatMemberEntity(Guid.NewGuid(), kickedUserId.ToString(), chatId, ChatMemberRole.Owner);
    chatMemberRepository.GetByUser(chatId, kickedUserId).Returns(kickedMember);

    var userManager = CreateUserManager(currentUserId, "admin");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager);

    await Should.ThrowAsync<Exception>(async () => await service.KickMembers(chatId, new List<Guid> { kickedUserId }));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default, default, default);
  }

  [Fact]
  public async Task KickMembers_ShouldAllow_WhenOwnerKicksAnyone()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var kickedUserId = Guid.NewGuid();
    var currentUser = CreateMockCurrentUser(currentUserId, "owner");

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Owner)
      });
    var kickedMember = new ChatMemberEntity(Guid.NewGuid(), kickedUserId.ToString(), chatId, ChatMemberRole.Administrator);
    chatMemberRepository.GetByUser(chatId, kickedUserId).Returns(kickedMember);
    chatMemberRepository.DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { Status = ChatRoomStatus.Opened });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var userManager = CreateUserManager(currentUserId, "owner");
    var service = CreateChatMemberDomainService(
      currentUser: currentUser,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService,
      chatRoomRepository: chatRoomRepository);

    await service.KickMembers(chatId, new List<Guid> { kickedUserId });

    await chatMemberRepository.Received(1).DeleteAsync(kickedMember, Arg.Any<bool>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task JoinChatAsync_ShouldAllow_WhenPublicChatWithMatchingRole()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var roleName = "Admin";

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true };
    chat.ViewChatPermissions.Add(new ViewChatPermissionEntity(Guid.NewGuid())
    {
      EntityType = PermissionEntityType.Role,
      EntityRef = roleName,
      ChatId = chatId
    });
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);
    chatMemberRepository.InsertAsync(Arg.Any<ChatMemberEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMemberEntity>());

    var currentUser = CreateMockCurrentUser(userId, "admin");
    currentUser.IsInRole(roleName).Returns(true);
    var userManager = CreateUserManager(userId, "admin", new List<string> { roleName });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService);

    await service.JoinChatAsync(chatId, userId);

    await chatMemberRepository.Received(1).InsertAsync(Arg.Any<ChatMemberEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task JoinChatAsync_ShouldReject_WhenPublicChatWithNonMatchingRole()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var requiredRole = "Admin";
    var userRole = "User";

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true };
    chat.ViewChatPermissions.Add(new ViewChatPermissionEntity(Guid.NewGuid())
    {
      EntityType = PermissionEntityType.Role,
      EntityRef = requiredRole,
      ChatId = chatId
    });
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);

    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user", new List<string> { userRole });

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager);

    await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () => await service.JoinChatAsync(chatId, userId));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task JoinChatAsync_ShouldAllow_WhenPublicChatWithMatchingOrgUnit()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var orgUnitId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true };
    chat.ViewChatPermissions.Add(new ViewChatPermissionEntity(Guid.NewGuid())
    {
      EntityType = PermissionEntityType.OrgUnit,
      EntityRef = orgUnitId.ToString(),
      ChatId = chatId
    });
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);
    chatMemberRepository.InsertAsync(Arg.Any<ChatMemberEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMemberEntity>());

    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user", null, new List<Guid> { orgUnitId });

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager,
      chatMessageDomainService: chatMessageDomainService);

    await service.JoinChatAsync(chatId, userId);

    await chatMemberRepository.Received(1).InsertAsync(Arg.Any<ChatMemberEntity>(), true, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task JoinChatAsync_ShouldReject_WhenPublicChatWithNonMatchingOrgUnit()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var requiredOrgUnitId = Guid.NewGuid();
    var userOrgUnitId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true };
    chat.ViewChatPermissions.Add(new ViewChatPermissionEntity(Guid.NewGuid())
    {
      EntityType = PermissionEntityType.OrgUnit,
      EntityRef = requiredOrgUnitId.ToString(),
      ChatId = chatId
    });
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);

    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user", null, new List<Guid> { userOrgUnitId });

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager);

    await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () => await service.JoinChatAsync(chatId, userId));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  // NOTE: These tests will fail until Phase 1 API key refactor is complete
  // They serve as regression tests to ensure API keys require explicit permissions
  [Fact(Skip = "Will pass after Phase 1 refactor - API keys should require explicit permissions")]
  public async Task JoinChatAsync_ShouldRequirePermission_WhenApiKey()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var httpAccessor = CreateHttpContextAccessor("api-key");

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    var chat = new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = false }; // Private chat
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(chat);

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByUser(chatId, userId).Returns((ChatMemberEntity)null);

    var service = CreateChatMemberDomainService(
      httpContextAccessor: httpAccessor,
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository);

    // After Phase 1 refactor, API keys should require explicit permissions
    // This should throw an exception when API key tries to join private chat
    await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () => await service.JoinChatAsync(chatId, userId));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact(Skip = "Will pass after Phase 1 refactor - API keys should require explicit permissions")]
  public async Task AddMembers_ShouldReject_WhenApiKeyWithoutPrivileges()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var apiKeyUserId = Guid.NewGuid();

    var httpAccessor = CreateHttpContextAccessor("api-key");

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>()); // API key has no membership

    var service = CreateChatMemberDomainService(
      httpContextAccessor: httpAccessor,
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository);

    // After Phase 1 refactor, API keys should require explicit privileges
    // This should throw an exception when API key doesn't have add members permission
    await Should.ThrowAsync<Exception>(async () => await service.AddMembers(chatId, new List<Guid> { userId }));

    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertManyAsync(default, default, default);
  }

  [Fact]
  public async Task JoinChatAsync_ShouldBeIdempotent_WhenCalledTwice()
  {
    var chatId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group) { IsPublic = true });

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    var existingMember = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), chatId, ChatMemberRole.Regular);
    chatMemberRepository.GetByUser(chatId, userId).Returns(existingMember);

    var currentUser = CreateMockCurrentUser(userId, "user");
    var userManager = CreateUserManager(userId, "user");

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      currentUser: currentUser,
      userManager: userManager);

    // First call - member already exists, should return without error
    await service.JoinChatAsync(chatId, userId);

    // Second call - should also return without error (idempotent)
    await service.JoinChatAsync(chatId, userId);

    // Should not insert duplicate members
    await chatMemberRepository.DidNotReceiveWithAnyArgs().InsertAsync(default, default, default);
  }

  [Fact]
  public async Task AddMembers_ShouldBeIdempotent_WhenSameUsersAddedTwice()
  {
    var chatId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();
    var newUserId = Guid.NewGuid();

    var chatRoomRepository = Substitute.For<IChatRoomRepository>();
    chatRoomRepository.GetAsync(chatId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
      .Returns(new ChatRoomEntity(chatId, ChatRoomType.Group));

    var chatMemberRepository = Substitute.For<IChatMemberRepository>();
    chatMemberRepository.GetByMember(chatId, Arg.Any<VPortal.Collaboration.Feature.Module.Specifications.ChatMemberSpecification>())
      .Returns(new List<ChatMemberEntity>
      {
        new ChatMemberEntity(Guid.NewGuid(), currentUserId.ToString(), chatId, ChatMemberRole.Administrator)
      });

    // First call: user doesn't exist
    chatMemberRepository.GetByUser(chatId, newUserId)
      .Returns((ChatMemberEntity)null, new ChatMemberEntity(Guid.NewGuid(), newUserId.ToString(), chatId, ChatMemberRole.Regular));

    chatMemberRepository.InsertManyAsync(Arg.Any<List<ChatMemberEntity>>(), true, Arg.Any<CancellationToken>())
      .Returns(System.Threading.Tasks.Task.CompletedTask);

    var userManager = CreateUserManager(currentUserId, "admin");
    userManager.AddUser(new Volo.Abp.Identity.IdentityUser(newUserId, "newuser", "newuser@example.com", null));
    var currentUser = CreateMockCurrentUser(currentUserId, "admin");

    var messageRepository = Substitute.For<IChatMessageRepository>();
    messageRepository.InsertAsync(Arg.Any<ChatMessageEntity>(), true, Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<ChatMessageEntity>());

    var chatMessageDomainService = CreateChatMessageDomainService(
      currentUser: currentUser,
      messageRepository: messageRepository,
      chatRoomRepository: chatRoomRepository);

    var service = CreateChatMemberDomainService(
      chatRoomRepository: chatRoomRepository,
      chatMemberRepository: chatMemberRepository,
      userManager: userManager,
      currentUser: currentUser,
      chatMessageDomainService: chatMessageDomainService);

    // First call - adds member
    var result1 = await service.AddMembers(chatId, new List<Guid> { newUserId });
    result1.ShouldNotBeNull();

    // Second call - member already exists, should handle gracefully (idempotent)
    // Note: Current implementation may throw or return null - this test documents expected idempotent behavior
    // After Phase 2 unique constraints, this should be truly idempotent
    var result2 = await service.AddMembers(chatId, new List<Guid> { newUserId });
    // Should not throw exception (idempotent)
  }
}
