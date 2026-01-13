//using Microsoft.AspNetCore.Authorization;
//using System.Threading.Tasks;
//using Volo.Abp.AspNetCore.SignalR;
//using Logging.Module;

//namespace VPortal.DocMessageLog.Module.Hubs;

//[Authorize]
//[HubRoute("hubs/doc-message-log/push-logs-hub")]
//public class PushDocMessageLogHub : AbpHub<IPushDocMessageLogClient>
//{
//    private readonly IVportalLogger<PushDocMessageLogHub> _logger;

//    public PushDocMessageLogHub(
//        IVportalLogger<PushDocMessageLogHub> logger)
//    {
//        _logger = logger;
//    }

//    public override async Task OnConnectedAsync()
//    {
//        await Groups.AddToGroupAsync(Context.ConnectionId, new DocMessageLogTenantGroupId(CurrentTenant).ToString());
//        await base.OnConnectedAsync();
//    }
//}
