using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
public class AzureEwsSettingsDto
{
  // <summary>Azure AD Tenant (GUID or domain)</summary>
  public string TenantId { get; set; } = string.Empty;

  /// <summary>Azure AD App (Client) ID</summary>
  public string ClientId { get; set; } = string.Empty;

  /// <summary>Azure AD Client Secret (for confidential client)</summary>
  public string ClientSecret { get; set; } = string.Empty;

  /// <summary>EWS endpoint URL</summary>
  public string ExchangeUrl { get; set; } = "https://outlook.office365.com/EWS/Exchange.asmx";

  /// <summary>Optional SMTP address to impersonate</summary>
  public string? ImpersonatedSmtpAddress { get; set; }

  /// <summary>Optional X-AnchorMailbox value; defaults to ImpersonatedSmtpAddress if not set</summary>
  public string? AnchorMailbox { get; set; }

  /// <summary>Enable EWS tracing</summary>
  public bool TraceEnabled { get; set; } = false;

  /// <summary>DEMO ONLY. If true, ignores SSL cert errors. Do not use in production.</summary>
  public bool IgnoreServerCertificateErrors { get; set; } = false;
  public string[] EwsScopes { get; set; } = new[] { "https://outlook.office365.com/.default" };
}
