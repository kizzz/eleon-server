using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.System;
public interface ISystemAppHubContext
{
  Task SendToAsync(string method, object data, List<Guid> userIds);
  Task SendToAllAsync(string method, object data);
}
