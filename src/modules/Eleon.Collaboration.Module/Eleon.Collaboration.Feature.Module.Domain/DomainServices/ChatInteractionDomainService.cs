using Common.EventBus.Module;
using Commons.Module.Messages.Storage;
using EleonsoftAbp.Auth;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Documents;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Users;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{

  public class ChatInteractionDomainService : DomainService
  {
    private readonly IVportalLogger<ChatInteractionDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly IChatMessageRepository chatMessageRepository;
    private readonly IChatRoomRepository chatRoomRepository;
    private readonly ChatMessageDomainService chatMessageDomainService;
    private readonly ChatMemberDomainService chatMemberDomainService;
    private readonly ChatUserHelperService chatUserHelperService;
    private readonly ChatDocumentManager chatDocumentManager;
    private readonly UserChatSettingDomainService userChatSettingDomainService;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IClock clock;
    private readonly IChatMemberRepository chatMemberRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedEventBus _distributedEventBus;

    public ChatInteractionDomainService(
        IVportalLogger<ChatInteractionDomainService> logger,
        ICurrentUser currentUser,
        IChatMessageRepository chatMessageRepository,
        IChatRoomRepository chatRoomRepository,
        ChatMessageDomainService chatMessageDomainService,
        ChatMemberDomainService chatMemberDomainService,
        ChatUserHelperService chatUserHelperService,
        ChatDocumentManager chatDocumentManager,
        UserChatSettingDomainService userChatSettingDomainService,
        IUnitOfWorkManager unitOfWorkManager,
        IClock clock,
        IChatMemberRepository chatMemberRepository,
        IHttpContextAccessor httpContextAccessor,
        IDistributedEventBus distributedEventBus)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.chatMessageRepository = chatMessageRepository;
      this.chatRoomRepository = chatRoomRepository;
      this.chatMessageDomainService = chatMessageDomainService;
      this.chatMemberDomainService = chatMemberDomainService;
      this.chatUserHelperService = chatUserHelperService;
      this.chatDocumentManager = chatDocumentManager;
      this.userChatSettingDomainService = userChatSettingDomainService;
      this.unitOfWorkManager = unitOfWorkManager;
      this.clock = clock;
      this.chatMemberRepository = chatMemberRepository;
      _httpContextAccessor = httpContextAccessor;
      _distributedEventBus = distributedEventBus;
    }

    public async Task<ChatMessageEntity> SendTextMessage(Guid chatId, string text, ChatMessageSeverity severity)
    {
      // skip permission validation if the request is made by an API key
      if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.GetApiKeyId()))
      {
        await EnsureWritePermission(chatId);
      }

      return await chatMessageDomainService.AddTextMessage(chatId, text, severity);
    }

    public async Task<ChatMessageEntity> SendDocumentMessage(Guid chatId, string filename, string documentBase64)
    {
      await EnsureWritePermission(chatId);
      return await chatMessageDomainService.AddDocumentMessage(chatId, filename, documentBase64);
    }

    public async Task<string> RetreiveDocumentMessageContent(Guid messageId)
    {
      var msg = await chatMessageRepository.GetAsync(messageId);
      await EnsureReadPermission(msg.ChatRoomId);
      var msgContent = JsonConvert.DeserializeObject<DocumentMessage>(msg.Content);
      var response = await _distributedEventBus.RequestAsync<GetFromStorageResponseMsg>(new GetFromStorageMsg { SettingsGroup = "Chat", BlobName = msgContent.BlobRefId });
      return response.Base64Data;
    }

    public async Task<KeyValuePair<int, List<UserChatInfo>>> GetLastChats(
        int skip,
        int take,
        string nameFilter,
        List<ChatRoomType> chatRoomTypes,
        List<string> tags = null,
        bool isArchived = false,
        bool isChannels = false)
    {
      try
      {
        var memberSpec = await chatMemberDomainService.GetCurrentMemberSpec();
        var chatInfos = await chatMessageRepository.GetLastChatsInfo(
            memberSpec,
            chatRoomTypes,
            skip,
            take,
            nameFilter,
            isArchived,
            isChannels && !isArchived,
            tags);
        await FillChatsInfo(chatInfos.Value);

        return KeyValuePair.Create((int)chatInfos.Key, chatInfos.Value);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<UserChatInfo> GetByIdAsync(Guid chatId)
    {
      try
      {
        var memberSpec = await chatMemberDomainService.GetCurrentMemberSpec();
        var chatInfo = await chatMessageRepository.GetChatInfoAsync(chatId, memberSpec);
        var chatTotalCount = await chatMemberRepository.GetMembershipsCount(memberSpec);

        await FillChatsInfo([chatInfo]);

        return chatInfo;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<List<ChatMessageEntity>> OpenChat(Guid chatId, int limit)
    {
      var currentMember = await EnsureReadPermission(chatId);

      var messages = await chatMessageRepository.GetLastChatMessages(chatId, 0, limit);
      await FillMessagesInfo(messages, currentMember?.LastModificationTime);

      if (currentMember != null)
      {
        currentMember.LastViewedByUser = clock.Now;
        await chatMemberRepository.UpdateAsync(currentMember, true);
      }

      return messages;
    }

    public async Task AckMessageReceived(Guid messageId)
    {
      using var uow = unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
      var msg = await chatMessageRepository.GetAsync(messageId);
      var currentMember = await EnsureMembership(msg.ChatRoomId);
      if (currentMember != null)
      {
        // Make idempotent: only update if message is newer than current read state
        var lastViewed = currentMember.LastViewedByUser ?? DateTime.MinValue;
        if (msg.CreationTime > lastViewed)
        {
          currentMember.LastViewedByUser = msg.CreationTime;
          await chatMemberRepository.UpdateAsync(currentMember, true);
        }
      }

      await uow.CompleteAsync();
    }

    public async Task<List<ChatMessageEntity>> GetChatMessages(Guid chatId, int skip, int take)
    {
      var currentMember = await EnsureReadPermission(chatId);
      var msgs = await chatMessageRepository.GetLastChatMessages(chatId, skip, take);
      await FillMessagesInfo(msgs, currentMember?.LastModificationTime);
      return msgs;
    }

    private async Task<ChatMemberEntity> EnsureMembership(Guid chatId)
    {
      return await chatMemberDomainService.EnsureCurrentUserMembership(chatId);
    }

    private async Task<ChatMemberEntity> EnsureWritePermission(Guid chatId)
    {
      return await chatMemberDomainService.EnsureWritePermission(chatId);
    }

    private async Task<ChatMemberEntity> EnsureReadPermission(Guid chatId)
    {
      return await chatMemberDomainService.EnsureReadPermission(chatId);
    }

    private async Task FillChatsInfo(List<UserChatInfo> chatInfos)
    {
      var msgs = chatInfos.Select(x => x.LastChatMessage).ToList();
      string currentUserId = currentUser.Id.ToString();
      foreach (var chatInfo in chatInfos)
      {
        var msg = chatInfo.LastChatMessage;
        msg.Outcoming = msg.Sender == currentUserId && msg.SenderType == ChatMessageSenderType.User;
        bool unread = msg.CreationTime > chatInfo.Chat.LastModificationTime;
        msg.Unread = unread && !msg.Outcoming;
        if (UserSender(chatInfo.LastChatMessage, out Guid userId))
        {
          chatInfo.LastChatMessage.SenderInfo =
              msgs.FirstOrDefault(x => x.Sender == chatInfo.LastChatMessage.Sender && x.SenderType == ChatMessageSenderType.User)?.SenderInfo
              ?? await chatUserHelperService.GetUserInfo(userId);
        }
      }

      var chats = chatInfos.Select(x => x.Chat).ToList();
      await chatUserHelperService.FillMembersInfo(chats);
    }

    private async Task FillMessagesInfo(List<ChatMessageEntity> messages, DateTime? lastModificationTime)
    {
      string currentUserId = currentUser.Id.ToString();
      foreach (var msg in messages)
      {
        msg.Outcoming = msg.Sender == currentUserId && msg.SenderType == ChatMessageSenderType.User;
        bool unread = msg.CreationTime > lastModificationTime;
        msg.Unread = unread && !msg.Outcoming;
        if (UserSender(msg, out Guid userId))
        {
          msg.SenderInfo =
              messages.FirstOrDefault(x => x.Sender == msg.Sender && x.SenderType == ChatMessageSenderType.User)?.SenderInfo
              ?? await chatUserHelperService.GetUserInfo(userId);
        }
      }
    }

    private bool UserSender(ChatMessageEntity msg, out Guid userId)
    {
      if (msg.SenderType == ChatMessageSenderType.User && Guid.TryParse(msg.Sender, out Guid parsedUserId))
      {
        userId = parsedUserId;
        return true;
      }

      userId = default;
      return false;
    }
  }
}
