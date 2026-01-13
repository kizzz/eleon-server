using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Common.Lib.Helpers;
public class ValidationHelper
{
  public static CheckConfigurationValidatorResult Validate(string validatorName, string key, IConfiguration configuration)
  {
    var value = configuration[key];
    switch (validatorName.ToLower())
    {
      case Validators.Required:
        return CheckConfigurationValidatorResult.Create(!string.IsNullOrWhiteSpace(value), $"{key} was empty");
      case Validators.Email:
        try
        {
          var addr = new System.Net.Mail.MailAddress(value);
          return CheckConfigurationValidatorResult.Create(addr.Address == value, $"{key} is not an email empty");
        }
        catch (Exception ex)
        {
          return CheckConfigurationValidatorResult.Create(false, $"{key} parse email failed with error: {ex.Message}");
        }
      case Validators.Url:
        return CheckConfigurationValidatorResult.Create(
            Uri.TryCreate(value, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps),
            $"{key} is not valid url");
      case Validators.Numeric:
        return CheckConfigurationValidatorResult.Create(double.TryParse(value, out _), $"{key} is not numric");
      case Validators.Alphanumeric:
        return CheckConfigurationValidatorResult.Create(value?.All(char.IsLetterOrDigit) ?? false, $"{key} is not alphanumeric");
      case Validators.Boolean:
        return CheckConfigurationValidatorResult.Create(bool.TryParse(value, out _), $"{key} is not boolean");
      case Validators.Guid:
        return CheckConfigurationValidatorResult.Create(Guid.TryParse(value, out _), $"{key} is not guid");
      case Validators.Ip:
        return CheckConfigurationValidatorResult.Create(System.Net.IPAddress.TryParse(value, out _), $"{key} is not ip address");
      case Validators.Date:
        return CheckConfigurationValidatorResult.Create(DateTime.TryParse(value, out _), $"{key} is not date");
      case Validators.Timespan:
        return CheckConfigurationValidatorResult.Create(TimeSpan.TryParse(value, out _), $"{key} is not timespan");
      default:
        return CheckConfigurationValidatorResult.NotApplied;
    }
  }

  public static class Validators
  {
    public const string Required = "required";
    public const string Email = "email";
    public const string Url = "url";
    public const string Numeric = "numeric";
    public const string Alphanumeric = "alphanumeric";
    public const string Boolean = "boolean";
    public const string Guid = "guid";
    public const string Ip = "ip";
    public const string Date = "date";
    public const string Timespan = "timespan";
  }
}
