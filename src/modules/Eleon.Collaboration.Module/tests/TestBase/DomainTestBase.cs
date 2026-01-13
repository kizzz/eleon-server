using System;
using System.Collections.Generic;
using System.Reflection;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using System.Runtime.CompilerServices;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Documents;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Localization;
using VPortal.Collaboration.Feature.Module.Push;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Users;
using Volo.Abp.Uow;

namespace CollaborationModule.Test.TestBase;

public abstract class DomainTestBase : MockingTestBase
{
  protected static IHttpContextAccessor CreateHttpContextAccessor(string apiKeyId = null)
  {
    return HttpContextTestHelpers.CreateHttpContextAccessor(apiKeyId);
  }

  protected static MockIdentityUserManager CreateUserManager(Guid userId, string userName, List<string> roles = null, List<Guid> orgUnitIds = null)
  {
    var manager = new MockIdentityUserManager();
    var user = new IdentityUser(userId, userName, $"{userName}@example.com", null);

    if (orgUnitIds != null)
    {
      foreach (var orgUnitId in orgUnitIds)
      {
        user.AddOrganizationUnit(orgUnitId);
      }
    }

    manager.AddUser(user);

    if (roles != null)
    {
      foreach (var role in roles)
      {
        manager.AddUserRole(userId, role);
      }
    }

    return manager;
  }

  protected static IStringLocalizer<CollaborationResource> CreateLocalizer(string key, string value)
  {
    return LocalizationTestHelpers.CreateLocalizer<CollaborationResource>(key, value);
  }

  protected static IClock CreateClock(DateTime now)
  {
    return TimeTestHelpers.CreateClock(now);
  }

  protected static ChatMessagePushManager CreateChatMessagePushManager(
      IVportalLogger<ChatMessagePushManager> logger = null,
      IObjectMapper objectMapper = null,
      IDistributedEventBus eventBus = null,
      IUserChatSettingRepository userChatSettingRepository = null,
      IChatMemberRepository chatMemberRepository = null,
      IChatRoomRepository chatRoomRepository = null)
  {
    var mapper = objectMapper ?? Substitute.For<IObjectMapper>();
    mapper.Map<ChatRoomEntity, Messaging.Module.ETO.ChatRoomEto>(Arg.Any<ChatRoomEntity>())
      .Returns(new Messaging.Module.ETO.ChatRoomEto());
    mapper.Map<ChatMessageEntity, Messaging.Module.ETO.ChatMessageEto>(Arg.Any<ChatMessageEntity>())
      .Returns(new Messaging.Module.ETO.ChatMessageEto());

    return new ChatMessagePushManager(
      logger ?? TestMockHelpers.CreateMockLogger<ChatMessagePushManager>(),
      mapper,
      eventBus ?? TestMockHelpers.CreateMockEventBus(),
      userChatSettingRepository ?? Substitute.For<IUserChatSettingRepository>(),
      chatMemberRepository ?? Substitute.For<IChatMemberRepository>(),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>());
  }

  protected ChatMessageDomainService CreateChatMessageDomainService(
      IVportalLogger<ChatMessageDomainService> logger = null,
      ChatDocumentManager documentManager = null,
      ChatUserHelperService userHelper = null,
      ChatMessagePushManager pushManager = null,
      ICurrentUser currentUser = null,
      IChatRoomRepository chatRoomRepository = null,
      IChatMessageRepository messageRepository = null,
      IObjectMapper objectMapper = null,
      IDistributedEventBus eventBus = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new ChatMessageDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<ChatMessageDomainService>(),
      documentManager ?? new ChatDocumentManager(
        TestMockHelpers.CreateMockLogger<ChatDocumentManager>(),
        eventBus ?? TestMockHelpers.CreateMockEventBus(),
        guidGenerator ?? CreateMockGuidGenerator()),
      userHelper ?? new ChatUserHelperService(CreateUserManager(Guid.NewGuid(), "user"), Substitute.For<IChatMemberRepository>()),
      pushManager ?? CreateChatMessagePushManager(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>(),
      messageRepository ?? Substitute.For<IChatMessageRepository>(),
      objectMapper ?? Substitute.For<IObjectMapper>(),
      eventBus ?? TestMockHelpers.CreateMockEventBus());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected ChatMemberDomainService CreateChatMemberDomainService(
      IVportalLogger<ChatMemberDomainService> logger = null,
      ICurrentUser currentUser = null,
      IdentityUserManager userManager = null,
      IChatRoomRepository chatRoomRepository = null,
      ChatUserHelperService chatUserHelper = null,
      ChatMessageDomainService chatMessageDomainService = null,
      IChatMemberRepository chatMemberRepository = null,
      IHttpContextAccessor httpContextAccessor = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new ChatMemberDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<ChatMemberDomainService>(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      userManager ?? CreateUserManager(Guid.NewGuid(), "user"),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>(),
      chatUserHelper ?? new ChatUserHelperService(CreateUserManager(Guid.NewGuid(), "user"), Substitute.For<IChatMemberRepository>()),
      chatMessageDomainService ?? CreateChatMessageDomainService(),
      chatMemberRepository ?? Substitute.For<IChatMemberRepository>(),
      httpContextAccessor ?? CreateHttpContextAccessor());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected ChatRoomDomainService CreateChatRoomDomainService(
      IVportalLogger<ChatRoomDomainService> logger = null,
      ICurrentUser currentUser = null,
      ChatMemberDomainService chatMemberDomainService = null,
      ChatMessageDomainService chatMessageDomainService = null,
      ChatUserHelperService chatUserHelper = null,
      IDistributedEventBus eventBus = null,
      IStringLocalizer<CollaborationResource> localizer = null,
      IChatRoomRepository chatRoomRepository = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new ChatRoomDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<ChatRoomDomainService>(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      chatMemberDomainService ?? CreateChatMemberDomainService(),
      chatMessageDomainService ?? CreateChatMessageDomainService(),
      chatUserHelper ?? new ChatUserHelperService(CreateUserManager(Guid.NewGuid(), "user"), Substitute.For<IChatMemberRepository>()),
      eventBus ?? CreateMockEventBus(),
      localizer ?? CreateLocalizer("GroupChatNamePrefix", "Group"),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected static IUnitOfWorkManager CreateUnitOfWorkManager(IUnitOfWork unitOfWork = null)
  {
    return TestMockHelpers.CreateMockUnitOfWorkManager(unitOfWork);
  }

  protected UserChatSettingDomainService CreateUserChatSettingDomainService(
      IVportalLogger<UserChatSettingDomainService> logger = null,
      IUserChatSettingRepository settingRepository = null,
      ICurrentUser currentUser = null,
      ChatMemberDomainService chatMemberDomainService = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new UserChatSettingDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<UserChatSettingDomainService>(),
      settingRepository ?? Substitute.For<IUserChatSettingRepository>(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      chatMemberDomainService ?? CreateChatMemberDomainService());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected DocumentConversationDomainService CreateDocumentConversationDomainService(
      IVportalLogger<DocumentConversationDomainService> logger = null,
      IChatRoomRepository chatRoomRepository = null,
      ChatMemberDomainService chatMemberDomainService = null,
      ChatMessageDomainService chatMessageDomainService = null,
      ChatRoomDomainService chatRoomDomainService = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new DocumentConversationDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<DocumentConversationDomainService>(),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>(),
      chatMemberDomainService ?? CreateChatMemberDomainService(),
      chatMessageDomainService ?? CreateChatMessageDomainService(),
      chatRoomDomainService ?? CreateChatRoomDomainService());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected SupportTicketDomainService CreateSupportTicketDomainService(
      IVportalLogger<SupportTicketDomainService> logger = null,
      ICurrentUser currentUser = null,
      ChatMemberDomainService chatMemberDomainService = null,
      ChatMessageDomainService chatMessageDomainService = null,
      ChatRoomDomainService chatRoomDomainService = null,
      IStringLocalizer<CollaborationResource> localizer = null,
      IDistributedEventBus eventBus = null,
      IChatRoomRepository chatRoomRepository = null,
      IGuidGenerator guidGenerator = null,
      IClock clock = null)
  {
    var service = new SupportTicketDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<SupportTicketDomainService>(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      chatMemberDomainService ?? CreateChatMemberDomainService(),
      chatMessageDomainService ?? CreateChatMessageDomainService(),
      chatRoomDomainService ?? CreateChatRoomDomainService(),
      localizer ?? CreateLocalizer("SupportTicketNamePrefix", "Ticket"),
      eventBus ?? TestMockHelpers.CreateMockEventBus(),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }

  protected static UnitOfWorkManager CreateDummyUnitOfWorkManager()
  {
    return (UnitOfWorkManager)RuntimeHelpers.GetUninitializedObject(typeof(UnitOfWorkManager));
  }

  protected ChatInteractionDomainService CreateChatInteractionDomainService(
      IVportalLogger<ChatInteractionDomainService> logger = null,
      ICurrentUser currentUser = null,
      IChatMessageRepository chatMessageRepository = null,
      IChatRoomRepository chatRoomRepository = null,
      ChatMessageDomainService chatMessageDomainService = null,
      ChatMemberDomainService chatMemberDomainService = null,
      ChatUserHelperService chatUserHelperService = null,
      ChatDocumentManager chatDocumentManager = null,
      UserChatSettingDomainService userChatSettingDomainService = null,
      IUnitOfWorkManager unitOfWorkManager = null,
      IClock clock = null,
      IChatMemberRepository chatMemberRepository = null,
      IHttpContextAccessor httpContextAccessor = null,
      IDistributedEventBus distributedEventBus = null,
      IGuidGenerator guidGenerator = null)
  {
    var service = new ChatInteractionDomainService(
      logger ?? TestMockHelpers.CreateMockLogger<ChatInteractionDomainService>(),
      currentUser ?? CreateMockCurrentUser(Guid.NewGuid(), "user"),
      chatMessageRepository ?? Substitute.For<IChatMessageRepository>(),
      chatRoomRepository ?? Substitute.For<IChatRoomRepository>(),
      chatMessageDomainService ?? CreateChatMessageDomainService(),
      chatMemberDomainService ?? CreateChatMemberDomainService(),
      chatUserHelperService ?? new ChatUserHelperService(CreateUserManager(Guid.NewGuid(), "user"), Substitute.For<IChatMemberRepository>()),
      chatDocumentManager ?? new ChatDocumentManager(TestMockHelpers.CreateMockLogger<ChatDocumentManager>(), TestMockHelpers.CreateMockEventBus(), guidGenerator ?? CreateMockGuidGenerator()),
      userChatSettingDomainService ?? CreateUserChatSettingDomainService(),
      unitOfWorkManager ?? TestMockHelpers.CreateMockUnitOfWorkManager(),
      clock ?? CreateClock(DateTime.UtcNow),
      chatMemberRepository ?? Substitute.For<IChatMemberRepository>(),
      httpContextAccessor ?? CreateHttpContextAccessor(),
      distributedEventBus ?? TestMockHelpers.CreateMockEventBus());

    SetLazyServiceProvider(service, guidGenerator ?? CreateMockGuidGenerator(), clock ?? CreateClock(DateTime.UtcNow));
    return service;
  }
}
