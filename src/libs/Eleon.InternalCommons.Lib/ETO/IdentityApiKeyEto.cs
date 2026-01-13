using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.ETO
{
  public class IdentityApiKeyEto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string RefId { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public bool Invalidated { get; set; }
    public ApiKeyType Type { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreationTime { get; set; }
    public bool AllowAuthorize { get; set; }
    public string KeySecret { get; set; }
    public string Data { get; set; }
  }
}
