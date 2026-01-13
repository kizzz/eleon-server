using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using MassTransit.Futures.Contracts;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
public class NotificatorBaseHelperService
{
  private readonly ILogger<NotificatorBaseHelperService> _logger;
  private readonly IConfiguration _configuration;
  private readonly ICurrentTenant _currentTenant;

  public NotificatorBaseHelperService(IServiceProvider serviceProvider)
  {
    _logger = serviceProvider.GetRequiredService<ILogger<NotificatorBaseHelperService>>();
    _configuration = serviceProvider.GetRequiredService<IConfiguration>();
    _currentTenant = serviceProvider.GetService<ICurrentTenant>();
  }
  public string GetTenantName()
  {
    if (string.IsNullOrEmpty(_currentTenant?.Name))
    {
      var configName = _configuration["ApplicationName"];

      if (string.IsNullOrEmpty(configName))
      {
        return "Host";
      }

      return configName + " Host";
    }

    return _currentTenant.Name;
  }

  public List<string> FilterEmails(List<string> emails)
  {
    var disableUserEmails = _configuration.GetValue("DebugSettings:EmailTelemetry:DisableUserEmails", false);
    var devEmails = _configuration.GetSection("DebugSettings:EmailTelemetry:DevEmails").Get<string[]>() ?? Array.Empty<string>();
    var allowedEmails = _configuration.GetSection("DebugSettings:EmailTelemetry:AllowedEmails").Get<string[]>() ?? Array.Empty<string>();

    var result = emails
        .Select(x => x?.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .WhereIf(disableUserEmails, x =>
            devEmails.Contains(x, StringComparer.OrdinalIgnoreCase) ||
            allowedEmails.Any(pattern => IsEmailMatch(x, pattern)))
        .Where(x => !x.EndsWith("@example.com"))
        .Distinct()
        .ToList();
    return result;
  }

  private static bool IsEmailMatch(string email, string pattern)
  {
    if (string.IsNullOrWhiteSpace(pattern))
      return false;

    // Exact match (no wildcards)
    if (!pattern.Contains('*') && !pattern.Contains('?'))
      return string.Equals(email, pattern, StringComparison.OrdinalIgnoreCase);

    // Convert wildcard pattern to regex
    var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
    return Regex.IsMatch(email, regexPattern, RegexOptions.IgnoreCase);
  }

  public string RenderSystemMessage(EleonsoftNotification notification, SystemNotificationType type, string template, string templateType)
  {
    if (string.IsNullOrEmpty(template) || string.IsNullOrEmpty(templateType))
    {
      template = _configuration.GetValue<string>("DebugSettings:DefaultTemplate");
      templateType = _configuration.GetValue<string>("DebugSettings:DefaultTemplateType");
    }

    if (string.IsNullOrEmpty(template) || string.IsNullOrEmpty(templateType))
    {
      template = NotificatorConstants.TelegramScribanTemplate;
      templateType = "Scriban";
    }

    // . was replaced with _ to support Scriban keys
    var replacements = GetPlaceholdersReplacements(notification, type);

    string result = string.Empty;

    if (templateType.Equals("Scriban", StringComparison.OrdinalIgnoreCase))
    {
      try
      {
        result = DynamicTemplateRenderer.RenderWithScriban(template, replacements);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to render Scriban template");
        result = DynamicTemplateRenderer.RenderWithReplace(NotificatorConstants.DefaultSystemMessageTemplate, replacements);
      }
    }
    else
    {
      result = DynamicTemplateRenderer.RenderWithReplace(NotificatorConstants.DefaultSystemMessageTemplate, replacements);
    }

    return result;
  }

  public static Dictionary<string, string> GetPlaceholdersReplacements(EleonsoftNotification notification, SystemNotificationType type)
  {
    var replacements = new Dictionary<string, string>(type.ExtraProperties.ToDictionary(x => x.Key.Replace('.', '_'), x => x.Value), StringComparer.OrdinalIgnoreCase);
    if (!string.IsNullOrWhiteSpace(replacements.GetOrDefault("time"))) replacements["time"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
    replacements["priority"] = notification.Priority.ToString();
    replacements["logLevel"] = type.LogLevel.ToString();
    replacements["message"] = notification.Message;
    replacements["extraProperties"] = type.ExtraProperties.Any() ? string.Join(";\n", type.ExtraProperties.Where(x => (x.Value?.Length ?? 0) < 100).Select(kvp => $"{kvp.Key}: {kvp.Value}")) : "None";
    replacements["endline"] = "\n";
    return replacements;
  }

  public bool IsEmail(string? input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return false;

    return EmailRegex.IsMatch(input);
  }

  public string GetValidatedSubjectOrDefault(string subject)
  {
    if (!string.IsNullOrWhiteSpace(subject))
    {
      return subject;
    }

    return $"{GetTenantName()} Notification";
  }

  private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  public bool IsPhone(string? input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return false;

    return PhoneRegex.IsMatch(input);
  }

  private static readonly Regex PhoneRegex = new Regex(@"^\+?[0-9]{7,15}$",  // allows + and 7–15 digits
      RegexOptions.Compiled);
}
