using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace VPortal.Collaboration.Feature.Module.UserChatSettings
{
  [Authorize]
  public class UserChatSettingAppService : CollaborationAppService, IUserChatSettingAppService
  {
    private readonly IVportalLogger<UserChatSettingAppService> logger;
    private readonly UserChatSettingDomainService domainService;

    public UserChatSettingAppService(
        IVportalLogger<UserChatSettingAppService> logger,
        UserChatSettingDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> SetChatMute(Guid chatId, bool isMuted)
    {
      bool result = false;
      try
      {
        await domainService.SetChatMute(chatId, isMuted);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetChatArchivedAsync(Guid chatId, bool isArchived)
    {

      try
      {
        await domainService.SetChatArchivedAsync(chatId, isArchived);
        return true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return false;
    }
  }
}
