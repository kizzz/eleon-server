using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;


public class CheckConfigurationValidatorResult
{
  public bool IsValid { get; }
  public bool IsApplied { get; }
  public string? ErrorMessage { get; }

  public bool IsErrored => IsApplied && !IsValid;

  private CheckConfigurationValidatorResult(bool isValid, bool isApplied, string? errorMessage)
  {
    IsValid = isValid;
    IsApplied = isApplied;
    ErrorMessage = errorMessage;
  }

  public static CheckConfigurationValidatorResult NotApplied => new(false, false, null);

  public static CheckConfigurationValidatorResult Invalid(string message) => new(false, true, message);

  public static CheckConfigurationValidatorResult Create(bool valid, string? errorMessage = null)
  {
    return new CheckConfigurationValidatorResult(valid, true, valid ? null : errorMessage);
  }
}
