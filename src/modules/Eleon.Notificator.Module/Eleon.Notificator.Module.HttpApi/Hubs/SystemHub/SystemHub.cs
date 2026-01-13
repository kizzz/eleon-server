using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.HttpApi.Hubs.SystemHub;


[AllowAnonymous]
[HubRoute("hubs/system/system-events-hub")]
public class SystemHub : AbpHub
{
}
