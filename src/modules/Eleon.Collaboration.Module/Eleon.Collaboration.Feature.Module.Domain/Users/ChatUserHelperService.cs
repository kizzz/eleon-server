using Common.Module.Extensions;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.TenantManagement.Module.Users;

namespace VPortal.Collaboration.Feature.Module.Users
{
  public class ChatUserHelperService : ITransientDependency
  {
    private readonly IdentityUserManager userManager;
    private readonly IChatMemberRepository chatMemberRepository;

    public ChatUserHelperService(IdentityUserManager userManager, IChatMemberRepository chatMemberRepository)
    {
      this.userManager = userManager;
      this.chatMemberRepository = chatMemberRepository;
    }

    public async Task<ChatMemberInfo> GetUserInfo(Guid userId) => (await GetUsersInfo(userId.ToSingleItemList())).First();

    public async Task<List<ChatMemberInfo>> GetUsersInfo(List<Guid> userIds)
    {
      var result = new List<ChatMemberInfo>();
      foreach (var userId in userIds)
      {
        var user = await userManager.FindByIdAsync(userId.ToString());

        result.Add(new ChatMemberInfo()
        {
          Id = userId,
          Name = user == null ? string.Empty : GetUserName(user),
          UserName = user?.UserName ?? string.Empty,
          Picture = user?.GetProfilePictureThumbnail(),
        });
      }

      return result;
    }

    public async Task FillMembersInfo(List<ChatRoomEntity> chats)
    {
      var chatIds = chats.Select(c => c.Id).ToList();
      var chatMembers = await chatMemberRepository.GetByChats(chatIds);
      foreach (var chat in chats)
      {
        var cm = chatMembers.GetOrDefault(chat.Id);
        chat.MembersAmount = cm.Key;
        chat.ChatMemberIdsPreview = cm.Value?.Select(x => Guid.Parse(x.RefId)).ToList() ?? [];
      }

      var ids = chats.SelectMany(c => c.ChatMemberIdsPreview).Distinct().ToList();
      var users = await GetUsersInfo(ids);
      foreach (var chat in chats)
      {
        var chatMembersForChat = chatMembers.GetOrDefault(chat.Id);
        chat.ChatMembersPreview = chat.ChatMemberIdsPreview
            .Select(uid =>
            {
              var userPreview = users.FirstOrDefault(u => u.Id == uid);
              var role = chatMembersForChat.Value?.FirstOrDefault(m => m.RefId == userPreview?.Id.ToString())?.Role ?? ChatMemberRole.Regular;
              return new ChatMemberInfo
              {
                Id = userPreview?.Id ?? uid,
                Name = userPreview?.Name ?? string.Empty,
                Picture = userPreview?.Picture,
                UserName = userPreview?.UserName ?? string.Empty,
                Role = role
              };
            })
            .ToList();
      }
    }

    private static string GetUserName(IdentityUser user) => user.Name.NonEmpty() || user.Surname.NonEmpty() ? string.Join(" ", user.Name, user.Surname) : user.UserName;
  }
}
