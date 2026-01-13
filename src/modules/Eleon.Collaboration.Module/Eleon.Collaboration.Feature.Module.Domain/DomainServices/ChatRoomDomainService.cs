using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Localization;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Users;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{

  public class ChatRoomDomainService : DomainService
  {
    private readonly IVportalLogger<ChatRoomDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly ChatMemberDomainService chatMemberDomainService;
    private readonly ChatMessageDomainService chatMessageDomainService;
    private readonly ChatUserHelperService chatUserService;
    private readonly IDistributedEventBus eventBus;
    private readonly IChatRoomRepository chatRoomRepository;

    public IStringLocalizer<CollaborationResource> L { get; }

    public ChatRoomDomainService(
        IVportalLogger<ChatRoomDomainService> logger,
        ICurrentUser currentUser,
        ChatMemberDomainService chatMemberDomainService,
        ChatMessageDomainService chatMessageDomainService,
        ChatUserHelperService chatUserService,
        IDistributedEventBus eventBus,
        IStringLocalizer<CollaborationResource> L,
        IChatRoomRepository chatRoomRepository)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.chatMemberDomainService = chatMemberDomainService;
      this.chatMessageDomainService = chatMessageDomainService;
      this.chatUserService = chatUserService;
      this.eventBus = eventBus;
      this.L = L;
      this.chatRoomRepository = chatRoomRepository;
    }

    public async Task<ChatRoomEntity> CreateChatAsync(
        string name,
        string refId = null,
        ChatRoomType type = ChatRoomType.Group,
        Dictionary<Guid, ChatMemberRole?> initialMembers = null,
        bool setOwner = true,
        bool isPublic = false,
        List<string> tags = null,
        List<string> allowedRoles = null,
        List<Guid> allowedOrgUnits = null,
        ChatMemberRole defaultRole = ChatMemberRole.Regular)
    {
      var chat = new ChatRoomEntity(GuidGenerator.Create(), type)
      {
        RefId = refId,
        Name = await GetGroupChatName(name),
        IsPublic = isPublic,
        DefaultRole = defaultRole,
      };

      if (tags != null)
      {
        chat.SetTags(tags);
      }

      if (allowedRoles != null || allowedOrgUnits != null)
      {
        chat.ViewChatPermissions ??= new List<ViewChatPermissionEntity>();
        chat.ViewChatPermissions.AddRange(allowedRoles?.Select(x => new ViewChatPermissionEntity(GuidGenerator.Create()) { EntityRef = x, EntityType = PermissionEntityType.Role, ChatId = chat.Id }) ?? []);
        chat.ViewChatPermissions.AddRange(allowedOrgUnits?.Select(x => new ViewChatPermissionEntity(GuidGenerator.Create()) { EntityRef = x.ToString(), EntityType = PermissionEntityType.OrgUnit, ChatId = chat.Id }) ?? []);
      }

      chat = await chatRoomRepository.InsertAsync(chat, true);


      initialMembers ??= new Dictionary<Guid, ChatMemberRole?>();
      if (setOwner)
      {
        initialMembers[currentUser.Id.Value] = ChatMemberRole.Owner;
      }

      await chatMemberDomainService.ForceAddUserMembers(chat.Id, initialMembers, chat.DefaultRole);

      if (setOwner)
      {
        await chatMemberDomainService.SetOwners(chat.Id, currentUser.Id.Value.ToSingleItemList());
        await chatMessageDomainService.AddChatCreatedMessage(chat.Id, currentUser.Id.Value, chat.CreationTime);
      }

      return chat;
    }

    public async Task<KeyValuePair<int, List<ChatRoomEntity>>> GetChatsList(string sorting, int take, int skip, string nameFilter = null)
    {
      var result = await chatRoomRepository.GetChatsList(sorting ?? "CreationTime", take, skip, nameFilter);
      await chatUserService.FillMembersInfo(result.Value);
      return result;
    }

    public async Task<List<ChatRoomEntity>> GetChatsByOwner(Guid userId, List<ChatRoomType> types)
    {
      return await chatRoomRepository.GetChatsByOwner(userId, types);
    }

    public async Task RenameChat(Guid chatId, string newName)
    {
      var member = await chatMemberDomainService.EnsureCurrentUserAdminRights(chatId);
      if (newName.IsNullOrWhiteSpace())
      {
        throw new Exception("The name should be non-empty.");
      }

      var chat = await chatRoomRepository.GetAsync(chatId);
      string oldName = chat.Name;
      chat.Name = newName;
      await chatRoomRepository.UpdateAsync(chat);

      await chatMessageDomainService.AddChatRenamedMessage(chatId, currentUser.Id.Value, oldName, newName);
    }

    public async Task<ChatRoomEntity> UpdateChatAsync(
        Guid chatId,
        string name,
        List<string> tags = null,
        bool isPublic = false,
        ChatMemberRole defaultRole = ChatMemberRole.Regular)
    {
      var member = await chatMemberDomainService.EnsureCurrentUserAdminRights(chatId);
      var chat = await chatRoomRepository.GetAsync(chatId);
      var prevName = chat.Name;
      chat.SetTags(tags);
      chat.DefaultRole = defaultRole;
      chat.Name = await GetGroupChatName(name);
      chat.IsPublic = isPublic;

      chat = await chatRoomRepository.UpdateAsync(chat);

      if (prevName != chat.Name)
      {
        await chatMessageDomainService.AddChatRenamedMessage(chatId, currentUser.Id.Value, prevName, chat.Name);
      }

      return chat;
    }

    public async Task CloseAsync(Guid chatId)
    {
      var member = await chatMemberDomainService.EnsureCurrentUserOwner(chatId);
      var chat = await chatRoomRepository.GetAsync(chatId);
      chat.Status = ChatRoomStatus.Closed;
      await chatRoomRepository.UpdateAsync(chat);
      await chatMessageDomainService.AddChatClosedMessage(chatId, currentUser.Id.Value);
    }

    private async Task<string> GetGroupChatName(string name)
    {
      if (name.IsNullOrWhiteSpace())
      {
        string number = await GetSeriaNumber("Chat");
        return $"{L["GroupChatNamePrefix"]} #{number}";
      }

      return name.Trim();
    }

    private async Task<string> GetSeriaNumber(string type)
    {
      var request = new GetDocumentSeriaNumberMsg
      {
        ObjectType = "Chat",
        Prefix = Prefixes.ObjectTypePrefixes[type]
      };
      var response = await eventBus.RequestAsync<DocumentSeriaNumberGotMsg>(request);
      return response.SeriaNumber;
    }
  }
}
