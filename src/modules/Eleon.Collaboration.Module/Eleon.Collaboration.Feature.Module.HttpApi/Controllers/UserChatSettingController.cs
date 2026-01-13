using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Collaboration.Feature.Module.UserChatSettings;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/UserChatSetting")]
  public class UserChatSettingController : ChatController, IUserChatSettingAppService
  {
    private readonly IUserChatSettingAppService appService;
    private readonly IVportalLogger<UserChatSettingController> _logger;

    public UserChatSettingController(
        IUserChatSettingAppService appService,
        IVportalLogger<UserChatSettingController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("SetChatArchived")]
    public async Task<bool> SetChatArchivedAsync(Guid chatId, bool isArchived)
    {

      var response = await appService.SetChatArchivedAsync(chatId, isArchived);


      return response;
    }

    [HttpPost("SetChatMute")]
    public async Task<bool> SetChatMute(Guid chatId, bool isMuted)
    {

      var response = await appService.SetChatMute(chatId, isMuted);


      return response;
    }
  }
}
