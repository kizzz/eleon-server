using Messaging.Module.ETO;
using VPortal.Identity.Module.Entities;

namespace ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
public class ApiKeyValidationResult
{
  public ApiKeyValidationResult()
  {
  }

  public ApiKeyValidationResult(bool isValid, string errorMessage, IdentityApiKeyEto apiKey)
  {
    IsValid = isValid;
    ErrorMessage = errorMessage;
    ApiKey = apiKey;
  }

  public bool IsValid { get; set; }
  public string ErrorMessage { get; set; }
  public IdentityApiKeyEto ApiKey { get; set; }
}
