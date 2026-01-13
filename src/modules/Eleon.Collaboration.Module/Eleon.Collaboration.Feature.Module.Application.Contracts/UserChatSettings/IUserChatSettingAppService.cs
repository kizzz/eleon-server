using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Collaboration.Feature.Module.UserChatSettings
{
  public interface IUserChatSettingAppService : IApplicationService
  {
    Task<bool> SetChatMute(Guid chatId, bool isMuted);
    Task<bool> SetChatArchivedAsync(Guid chatId, bool isArchived);
  }
}
