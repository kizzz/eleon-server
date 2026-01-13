using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.ApiKey;

[Common.Module.Events.DistributedEvent]
public class ApiKeyResponseMsg
{
  public string KeyId { get; set; }
  public string Name { get; set; }
  public string RefId { get; set; }
  public DateTime? ExpiredAt { get; set; }
  public bool AllowAuthorize { get; set; }
  public string Key { get; set; }
  public string KeySecret { get; set; }
  public string Data { get; set; }
  public bool Found { get; set; }
}
