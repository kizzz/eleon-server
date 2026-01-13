using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{

  public class UserChatSettingDomainService : DomainService
  {
    private readonly IVportalLogger<UserChatSettingDomainService> logger;
    private readonly IUserChatSettingRepository settingRepository;
    private readonly ICurrentUser currentUser;
    private readonly ChatMemberDomainService chatMemberDomainService;

    public UserChatSettingDomainService(
        IVportalLogger<UserChatSettingDomainService> logger,
        IUserChatSettingRepository settingRepository,
        ICurrentUser currentUser,
        ChatMemberDomainService chatMemberDomainService)
    {
      this.logger = logger;
      this.settingRepository = settingRepository;
      this.currentUser = currentUser;
      this.chatMemberDomainService = chatMemberDomainService;
    }

    public async Task SetChatMute(Guid chatId, bool isMuted)
    {
      var currentMember = await chatMemberDomainService.EnsureCurrentUserMembership(chatId);
      var setting = await GetOrAddUserSetting(currentUser.Id.Value, chatId);
      setting.IsChatMuted = isMuted;
      await settingRepository.UpdateAsync(setting, true);
    }

    public async Task SetChatArchivedAsync(Guid chatId, bool isArchived, Guid? userId = null)
    {
      var currentMember = await chatMemberDomainService.EnsureCurrentUserMembership(chatId);
      var setting = await GetOrAddUserSetting(userId ?? currentUser.Id.Value, chatId);
      setting.IsArchived = isArchived;
      await settingRepository.UpdateAsync(setting, true);
    }

    internal async Task<bool> IsChatMuted(Guid userId, Guid chatId)
    {
      var existing = await settingRepository.GetChatSettingAsync(userId, chatId);
      return existing?.IsChatMuted ?? false;
    }

    internal async Task<bool> IsChatArchived(Guid userId, Guid chatId)
    {
      var existing = await settingRepository.GetChatSettingAsync(userId, chatId);
      return existing?.IsArchived ?? false;
    }

    internal Task<UserChatSettingEntity> GetChatSettingAsync(Guid userId, Guid chatId)
    {
      return settingRepository.GetChatSettingAsync(userId, chatId);
    }

    private async Task<UserChatSettingEntity> GetOrAddUserSetting(Guid userId, Guid chatId)
    {
      var existing = await settingRepository.GetChatSettingAsync(userId, chatId);
      if (existing == null)
      {
        existing = new UserChatSettingEntity(Guid.NewGuid(), userId, chatId);
        await settingRepository.InsertAsync(existing, true);
      }

      return existing;
    }
  }
}
