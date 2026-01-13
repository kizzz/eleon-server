using Common.Module.Constants;
using System;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys
{
  public class IdentityApiKeyDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string RefId { get; set; }
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
