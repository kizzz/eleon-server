using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CollaborationModule.Test.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using NSubstitute;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.ChatInteractions;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.ChatRooms;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.DocumentConversations;
using VPortal.Collaboration.Feature.Module.Entities;

namespace CollaborationModule.Test.TestBase;

public abstract class AppServiceTestBase : DomainTestBase
{

  protected static void SetAppServiceDependencies(ApplicationService service, IObjectMapper objectMapper, ICurrentUser currentUser)
  {
    ServiceProviderTestHelpers.SetAppServiceDependencies(service, objectMapper, currentUser);
  }

  protected static IObjectMapper CreateCollaborationObjectMapper()
  {
    var mapper = Substitute.For<IObjectMapper>();

    mapper.Map<ChatRoomEntity, ChatRoomDto>(Arg.Any<ChatRoomEntity>())
      .Returns(callInfo =>
      {
        var entity = callInfo.Arg<ChatRoomEntity>();
        return entity == null ? null : new ChatRoomDto { Id = entity.Id, Name = entity.Name, RefId = entity.RefId };
      });

    mapper.Map<List<ChatRoomEntity>, List<ChatRoomDto>>(Arg.Any<List<ChatRoomEntity>>())
      .Returns(callInfo => callInfo.Arg<List<ChatRoomEntity>>()?.Select(e => new ChatRoomDto { Id = e.Id, Name = e.Name, RefId = e.RefId }).ToList());

    mapper.Map<ChatMessageEntity, ChatMessageDto>(Arg.Any<ChatMessageEntity>())
      .Returns(callInfo =>
      {
        var entity = callInfo.Arg<ChatMessageEntity>();
        return entity == null ? null : new ChatMessageDto { Id = entity.Id, ChatRoomId = entity.ChatRoomId, Content = entity.Content, Sender = entity.Sender };
      });

    mapper.Map<List<ChatMessageEntity>, List<ChatMessageDto>>(Arg.Any<List<ChatMessageEntity>>())
      .Returns(callInfo => callInfo.Arg<List<ChatMessageEntity>>()?.Select(e => new ChatMessageDto { Id = e.Id, ChatRoomId = e.ChatRoomId, Content = e.Content, Sender = e.Sender }).ToList());

    mapper.Map<UserChatInfo, UserChatInfoDto>(Arg.Any<UserChatInfo>())
      .Returns(callInfo =>
      {
        var info = callInfo.Arg<UserChatInfo>();
        if (info == null)
        {
          return null;
        }

        return new UserChatInfoDto
        {
          Chat = info.Chat == null ? null : new ChatRoomDto { Id = info.Chat.Id, Name = info.Chat.Name },
          LastChatMessage = info.LastChatMessage == null ? null : new ChatMessageDto { Id = info.LastChatMessage.Id, ChatRoomId = info.LastChatMessage.ChatRoomId },
          UnreadCount = info.UnreadCount,
          IsArchived = info.IsArchived,
          IsChatMuted = info.IsChatMuted
        };
      });

    mapper.Map<List<UserChatInfo>, List<UserChatInfoDto>>(Arg.Any<List<UserChatInfo>>())
      .Returns(callInfo => callInfo.Arg<List<UserChatInfo>>()?.Select(info => new UserChatInfoDto
      {
        Chat = info.Chat == null ? null : new ChatRoomDto { Id = info.Chat.Id, Name = info.Chat.Name },
        LastChatMessage = info.LastChatMessage == null ? null : new ChatMessageDto { Id = info.LastChatMessage.Id, ChatRoomId = info.LastChatMessage.ChatRoomId },
        UnreadCount = info.UnreadCount,
        IsArchived = info.IsArchived,
        IsChatMuted = info.IsChatMuted
      }).ToList());

    mapper.Map<DocumentConversationInfo, DocumentConversationInfoDto>(Arg.Any<DocumentConversationInfo>())
      .Returns(callInfo =>
      {
        var info = callInfo.Arg<DocumentConversationInfo>();
        return info == null ? null : new DocumentConversationInfoDto
        {
          ChatRoom = info.ChatRoom == null ? null : new ChatRoomDto { Id = info.ChatRoom.Id, Name = info.ChatRoom.Name },
          IsMember = info.IsMember
        };
      });

    return mapper;
  }
}
