using Eleon.Common.Lib.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.AppSettings.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;

public class CheckConfigurationService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<CheckConfigurationService> _logger;
  private readonly CheckConfigurationOptions _options;

  public CheckConfigurationService(
      IOptions<CheckConfigurationOptions> options,
      IConfiguration configuration,
      ILogger<CheckConfigurationService> logger)
  {
    _configuration = configuration;
    _logger = logger;
    _options = options.Value;
  }

  protected KeyValuePair<bool, Dictionary<string, CheckConfigurationValidatorResult>> _cachedResult;
  public virtual async Task<KeyValuePair<bool, Dictionary<string, CheckConfigurationValidatorResult>>> CheckAsync()
  {
    if (_cachedResult.Value == null)
    {
      _cachedResult = await ChechConfigurationsAsync(_options.Configurations, _configuration);
    }

    return _cachedResult;
  }


  public Task<KeyValuePair<bool, Dictionary<string, CheckConfigurationValidatorResult>>> ChechConfigurationsAsync(Dictionary<string, string> configurations, IConfiguration configuration, char sectionSeparator = '.')
  {
    if (configurations.IsNullOrEmpty())
    {
      _logger.LogWarning("No configurations found to validate. Skipping configuration validation.");
      return Task.FromResult(KeyValuePair.Create(true, new Dictionary<string, CheckConfigurationValidatorResult>()));
    }

    var result = new Dictionary<string, CheckConfigurationValidatorResult>();
    foreach (var option in configurations)
    {
      try
      {
        result[option.Key] = ValidationHelper.Validate(option.Value, option.Key.Replace(sectionSeparator, ':'), configuration);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error validating configuration: {option}", ex);
        result[option.Key] = CheckConfigurationValidatorResult.Invalid(ex.Message);
      }
    }

    var defaultSectionsValidationErrors = SettingsLoader.ValidateConfiguration(configuration);
    if (defaultSectionsValidationErrors.Count > 0)
    {
      foreach (var section in defaultSectionsValidationErrors)
      {
        result.Add(section.Key, CheckConfigurationValidatorResult.Invalid(section.Value));
      }
    }

    var errors = result.Where(r => r.Value.IsErrored).ToList();

    if (errors.Count == 0)
    {
      return Task.FromResult(KeyValuePair.Create(true, result));
    }

    foreach (var error in errors)
    {
      _logger.LogWarning($"Invalid configuration: {error.Key};\nErrors:{string.Join(";\n", error.Value.ErrorMessage)}");
    }

    return Task.FromResult(KeyValuePair.Create(false, result));
  }
}
