using Common.Module.Constants;
using System;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
public class CreateApiKeyDto
{
  public string Name { get; set; }
  public string RefId { get; set; }
  public ApiKeyType Type { get; set; }
  public DateTime? ExpiresAt { get; set; }
  public bool AllowAuthorize { get; set; }
  public string Data { get; set; }
}
