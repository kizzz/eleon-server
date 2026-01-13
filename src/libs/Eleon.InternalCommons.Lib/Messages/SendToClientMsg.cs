using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Commons.Module.Messages;
public class SendToClientMsg
{
  public bool IsToAll { get; set; }
  public List<Guid> UserIds { get; set; } = new();
  public string Method { get; set; } = string.Empty;
  public object Data { get; set; } = new();
}
