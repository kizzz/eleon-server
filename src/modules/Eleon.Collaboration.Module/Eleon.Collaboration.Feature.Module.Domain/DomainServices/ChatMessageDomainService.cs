using Common.EventBus.Module;
using Commons.Module.Messages.Storage;
using Logging.Module;
using Messaging.Module.ETO;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Chats;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Documents;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Push;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Users;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{

  public class ChatMessageDomainService : DomainService
  {
    private readonly IVportalLogger<ChatMessageDomainService> logger;
    private readonly ChatDocumentManager chatDocumentManager;
    private readonly ChatUserHelperService userHelper;
    private readonly ChatMessagePushManager messagePushManager;
    private readonly ICurrentUser currentUser;
    private readonly IChatRoomRepository chatRoomRepository;
    private readonly IChatMessageRepository messageRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly IDistributedEventBus _eventBus;

    public ChatMessageDomainService(
        IVportalLogger<ChatMessageDomainService> logger,
        ChatDocumentManager chatDocumentManager,
        ChatUserHelperService userHelper,
        ChatMessagePushManager messagePushManager,
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository,
        IChatMessageRepository messageRepository,
        IObjectMapper objectMapper,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.chatDocumentManager = chatDocumentManager;
      this.userHelper = userHelper;
      this.messagePushManager = messagePushManager;
      this.currentUser = currentUser;
      this.chatRoomRepository = chatRoomRepository;
      this.messageRepository = messageRepository;
      _objectMapper = objectMapper;
      _eventBus = eventBus;
    }

    public async Task<ChatMessageEntity> AddTextMessage(Guid chatId, string message, ChatMessageSeverity severity = ChatMessageSeverity.None)
    {
      return await AddChatMessage(currentUser.Id?.ToString(), currentUser.Id.HasValue ? ChatMessageSenderType.User : ChatMessageSenderType.System, ChatMessageType.PlainText, message, chatId, severity: severity);
    }

    public async Task<ChatMessageEntity> AddDocumentMessage(Guid chatId, string filename, string documentBase64)
    {
      string refId = GuidGenerator.Create().ToString();
      var saved = await _eventBus.RequestAsync<SaveToStorageResponseMsg>(new SaveToStorageMsg { SettingsGroup = "Chat", BlobName = refId, Base64Data = documentBase64 });
      if (!saved.Success)
      {
        throw new Exception($"Unable to save the document for the message");
      }
      var msg = new DocumentMessage()
      {
        BlobRefId = refId,
        Filename = filename,
      };

      return await AddChatMessage(
          currentUser.Id.ToString(),
          ChatMessageSenderType.User,
          ChatMessageType.Document,
          JsonConvert.SerializeObject(msg),
          chatId);
    }

    public async Task<ChatMessageEntity> AddMembersAddedMessage(Guid chatId, Guid addedByUserId, List<Guid> addedUsersIds)
    {
      return await AddSystemMessage(chatId, ChatMessageType.MembersAdded, new MembersAddedMessage()
      {
        AddedByUser = await userHelper.GetUserInfo(addedByUserId),
        AddedUsers = await userHelper.GetUsersInfo(addedUsersIds),
      });
    }

    public async Task<ChatMessageEntity> AddMemberJoinedMessage(Guid chatId, Guid joinedUserId)
    {
      return await AddSystemMessage(chatId, ChatMessageType.MemberJoined, new MemberJoinedMessage()
      {
        JoinedUser = await userHelper.GetUserInfo(joinedUserId),
      });
    }

    public async Task<ChatMessageEntity> AddMembersKickedMessage(Guid chatId, Guid kickedByUserId, List<Guid> kickedUsersIds)
    {
      return await AddSystemMessage(chatId, ChatMessageType.MembersKicked, new MembersKickedMessage()
      {
        KickedByUser = await userHelper.GetUserInfo(kickedByUserId),
        KickedUsers = await userHelper.GetUsersInfo(kickedUsersIds),
      });
    }

    public async Task<ChatMessageEntity> AddChatCreatedMessage(Guid chatId, Guid createdByUserId, DateTime chatCreationTime)
    {
      return await AddSystemMessage(
          chatId,
          ChatMessageType.ChatCreated,
          new ChatCreatedMessage()
          {
            CreatedByUser = await userHelper.GetUserInfo(createdByUserId),
            ChatCreationTime = chatCreationTime,
          },
          severity: ChatMessageSeverity.Warning);
    }

    public async Task<ChatMessageEntity> AddUserLeftMessage(Guid chatId, Guid userId)
    {
      return await AddSystemMessage(chatId, ChatMessageType.UserLeft, new UserLeftMessage()
      {
        User = await userHelper.GetUserInfo(userId),
      });
    }

    public async Task<ChatMessageEntity> AddChatClosedMessage(Guid chatId, Guid userId)
    {
      return await AddSystemMessage(
          chatId,
          ChatMessageType.ChatClosed,
          new ChatClosedMessage()
          {
            ClosedByUser = await userHelper.GetUserInfo(userId),
            ChatCloseTime = Clock.Now,
          },
          ensureOpened: false,
          severity: ChatMessageSeverity.Warning);
    }

    public async Task<ChatMessageEntity> AddChatRenamedMessage(Guid chatId, Guid userId, string oldName, string newName)
    {
      return await AddSystemMessage(chatId, ChatMessageType.ChatRenamed, new ChatRenamedMessage()
      {
        RenamedByUser = await userHelper.GetUserInfo(userId),
        OldName = oldName,
        NewName = newName,
      });
    }

    public async Task<ChatMessageEntity> AddLocalizedSystemMessage(Guid chatId, string localizationKey, string[] localizationParams, ChatMessageSeverity severity)
    {
      return await AddSystemMessage(
          chatId,
          ChatMessageType.LocalizedText,
          new LocalizedMessage()
          {
            LocalizationKey = localizationKey,
            LocalizationParams = localizationParams,
          },
          severity: severity);
    }

    private async Task EnsureChatOpened(Guid chatId)
    {
      var chat = await chatRoomRepository.GetAsync(chatId);
      if (chat.Status == ChatRoomStatus.Closed)
      {
        throw new Exception("Chat should be opened.");
      }
    }

    private async Task<ChatMessageEntity> AddSystemMessage<T>(
        Guid chatId,
        ChatMessageType type,
        T message,
        bool ensureOpened = true,
        ChatMessageSeverity severity = ChatMessageSeverity.Info)
    {
      if (ensureOpened)
      {
        await EnsureChatOpened(chatId);
      }

      string contentJson = JsonConvert.SerializeObject(message);

      var msg = new ChatMessageEntity(GuidGenerator.Create())
      {
        Sender = null,
        SenderType = ChatMessageSenderType.System,
        Type = type,
        Content = contentJson,
        ChatRoomId = chatId,
        Unread = true,
        SenderInfo = null,
        Severity = severity,
      };

      var result = await messageRepository.InsertAsync(msg, true);
      await messagePushManager.PushMessageAsync(msg);

      if (message is MembersKickedMessage kicked)
      {
        var chatRoom = await chatRoomRepository.GetAsync(chatId);

        await messagePushManager.SendPushNotificationAsync(new ChatPushMessageEto()
        {
          ChatRoom = _objectMapper.Map<ChatRoomEntity, ChatRoomEto>(chatRoom),
          Message = _objectMapper.Map<ChatMessageEntity, ChatMessageEto>(msg),
          MutedUsers = [],
        }, kicked.KickedUsers.Select(x => x.Id).ToList(), []);
      }

      return result;
    }

    private async Task<ChatMessageEntity> AddChatMessage(
        string sender,
        ChatMessageSenderType senderType,
        ChatMessageType type,
        string content,
        Guid chatId,
        bool ensureOpened = true,
        ChatMessageSeverity severity = ChatMessageSeverity.None)
    {
      if (ensureOpened)
      {
        await EnsureChatOpened(chatId);
      }

      ChatMemberInfo senderInfo = senderType == ChatMessageSenderType.User && Guid.TryParse(sender, out var senderId)
          ? await userHelper.GetUserInfo(senderId)
          : null;

      var msg = new ChatMessageEntity(GuidGenerator.Create())
      {
        Sender = sender,
        SenderType = senderType,
        Type = type,
        Content = content,
        ChatRoomId = chatId,
        Unread = true,
        SenderInfo = senderInfo,
        Severity = severity,
      };

      var result = await messageRepository.InsertAsync(msg, true);
      await messagePushManager.PushMessageAsync(msg);

      return result;
    }
  }
}
