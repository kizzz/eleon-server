using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Logging.Module.ErrorHandling.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedModule.modules.Helpers.Module;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;
public static class HealthCheckStaticPageHelper
{
  private static readonly JsonSerializerOptions JsonOpts = new()
  {
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  public static async Task HandleHealthCheckUrl(HttpContext context, bool shouldCheckHealth)
  {
    var result = true;

    if (shouldCheckHealth)
    {
      // Try new coordinator first, fall back to old manager
      var coordinator = context.RequestServices.GetService<EleonsoftSdk.modules.HealthCheck.Module.Core.IHealthRunCoordinator>();
      if (coordinator != null)
      {
        var snapshot = coordinator.GetLatestSnapshot();
        if (snapshot?.HealthCheck?.Reports == null || snapshot.HealthCheck.Reports.Any(x => x.Status == HealthCheckStatus.Failed))
        {
          result = false;
        }
      }
      else
      {
        // Fallback to old manager
        var healthCheckManager = context.RequestServices.GetRequiredService<HealthCheckManager>();
        var latestHealthCheck = healthCheckManager.GetLatestHealthCheck();
        if (latestHealthCheck?.HealthCheck?.Reports == null || latestHealthCheck.HealthCheck.Reports.Any(x => x.Status == HealthCheckStatus.Failed))
        {
          result = false;
        }
      }
    }

    if (!context.Response.HasStarted)
    {
      context.Response.StatusCode = result ? StatusCodes.Status200OK : EleonsoftStatusCodes.Default.UnhealthyService;
      await context.Response.WriteAsync(result.ToString().ToLower());
    }
  }

  public static async Task HandleUIPageAsync(HttpContext context)
  {
    if (context.Request.Method == HttpMethod.Post.Method)
    {
      // Try new coordinator first, fall back to old manager
      var coordinator = context.RequestServices.GetService<EleonsoftSdk.modules.HealthCheck.Module.Core.IHealthRunCoordinator>();
      if (coordinator != null)
      {
        // Trigger health check in background
        _ = Task.Run(async () => await coordinator.RunAsync(
          type: HealthCheckDefaults.HealthCheckTypes.Manual,
          initiatorName: context.User?.Identity?.Name ?? HealthCheckDefaults.Inititors.Unauthorized,
          options: null,
          ct: default));
      }
      else
      {
        // Fallback to old manager
        var healthCheckManager = context.RequestServices.GetRequiredService<HealthCheckManager>();
        _ = Task.Run(async () => await healthCheckManager.ExecuteHealthCheckAsync(
          HealthCheckDefaults.HealthCheckTypes.Manual,
          context.User.FindUserId()?.ToString() ?? HealthCheckDefaults.Inititors.Unauthorized));
      }

      context.Response.StatusCode = StatusCodes.Status202Accepted;
      return;
    }

    var wantsJson =
            context.Request.Query.TryGetValue("format", out var fmt) && fmt.ToString().Equals("json", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Headers["Accept"].ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase);

    if (wantsJson)
    {
      await JsonResponse(context);
    }
    else
    {
      await HtmlResponse(context);
    }
  }

  public static async Task JsonResponse(HttpContext context)
  {
    // Try new coordinator first, fall back to old manager
    HealthCheckItem? item = null;
    var coordinator = context.RequestServices.GetService<EleonsoftSdk.modules.HealthCheck.Module.Core.IHealthRunCoordinator>();
    if (coordinator != null)
    {
      var snapshot = coordinator.GetLatestSnapshot();
      if (snapshot != null)
      {
        item = new HealthCheckItem { HealthCheck = snapshot.HealthCheck };
      }
    }
    else
    {
      var healthCheckManager = context.RequestServices.GetRequiredService<HealthCheckManager>();
      item = healthCheckManager.GetLatestHealthCheck();
    }

    var check = item;

    if (!context.Response.HasStarted)
    {
      context.Response.ContentType = "application/json; charset=utf-8";
      context.Response.StatusCode = StatusCodes.Status200OK;
      await context.Response.WriteAsync(JsonSerializer.Serialize(check, JsonOpts));
    }
  }

  public static async Task HtmlResponse(HttpContext context)
  {
    var options = context.RequestServices.GetRequiredService<IOptions<HealthCheckOptions>>().Value;

    var basePath = options.UI.Path?.TrimEnd('/') ?? "/healthchecks-ui";
    var jsonPath = $"{basePath}/?format=json";
    var triggerUrl = $"{basePath}/";

    // Try new coordinator first, fall back to old manager
    HealthCheckItem? latest = null;
    var coordinator = context.RequestServices.GetService<EleonsoftSdk.modules.HealthCheck.Module.Core.IHealthRunCoordinator>();
    if (coordinator != null)
    {
      var snapshot = coordinator.GetLatestSnapshot();
      if (snapshot != null)
      {
        latest = new HealthCheckItem { HealthCheck = snapshot.HealthCheck };
      }
    }
    else
    {
      var healthCheckManager = context.RequestServices.GetRequiredService<HealthCheckManager>();
      latest = healthCheckManager.GetLatestHealthCheck();
    }

    var navigates = new List<(string Name, string Url)>()
        {
            ("Raw JSON", jsonPath),
        };

    if (options.RestartEnabled)
    {
      navigates.Add(("Restart", basePath + "/restart"));
    }

    navigates.AddRange(options.UI.Navigates.Where(x => x.Enabled).Select(x => (x.Name, x.Path)));

    var html = GenerateHtml(options, latest, basePath, triggerUrl, navigates);

    if (!context.Response.HasStarted)
    {
      context.Response.ContentType = "text/html; charset=utf-8";
      context.Response.StatusCode = StatusCodes.Status200OK;
      await context.Response.WriteAsync(html);
    }
  }

  public static string GenerateHtml(
          HealthCheckOptions options,
          HealthCheckItem latest,
          string pagePath,
          string triggerUrl,
          List<(string Name, string Url)> navigates)
  {
    var sb = new StringBuilder();
    sb.AppendLine("<!doctype html>");
    sb.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\"/>");
    sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/>");
    sb.AppendLine("<title>Health Checks</title>");
    sb.AppendLine("<style>");
    sb.AppendLine(GenerateCss());
    sb.AppendLine("</style>");
    sb.AppendLine("</head><body>");
    sb.AppendLine("<div class=\"container\">");

    // Header
    sb.AppendLine("<header class=\"header\">");
    sb.AppendLine($"<h1>Health Checks<span class=\"service\">{Html(options.ApplicationName)}</span></h1>");
    sb.AppendLine("<div class=\"actions\">");

    sb.AppendLine($"<a class=\"btn download-btn\" data-report-json='{Html(JsonSerializer.Serialize(latest?.HealthCheck ?? new HealthCheckEto(), JsonOpts))}' data-service-name='{Html(options.ApplicationName)}' data-check-name='{Html("HealthCheck")}'>Download</a>");
    sb.AppendLine($"<a class=\"btn copy-btn\" data-report-json='{Html(JsonSerializer.Serialize(latest?.HealthCheck ?? new HealthCheckEto(), JsonOpts))}' data-service-name='{Html(options.ApplicationName)}' data-check-name='{Html("HealthCheck")}'>Copy</a>");
    foreach (var navigate in navigates)
    {
      sb.AppendLine($"<a class=\"btn\" href=\"#\" onclick=\"window.location.href = replacePathBase('{pagePath}', '{navigate.Url}')\">{navigate.Name}</a>");
    }
    if (options.UI.ManualHealthCheckEnabled)
      sb.AppendLine($"<button class=\"btn success\" id=\"runBtn\" data-url=\"{Html(triggerUrl)}\">Run health check</button>");
    sb.AppendLine("</div>");
    sb.AppendLine("</header>");

    // Latest card
    sb.AppendLine("<section class=\"card\">");
    sb.AppendLine("<div class=\"card-header\">Latest Result</div>");
    sb.AppendLine("<div class=\"card-body\">");
    if (latest?.HealthCheck != null)
    {
      var hc = latest.HealthCheck;
      sb.AppendLine("<div class=\"summary\">");
      sb.AppendLine(Badge(latest.DeliveringSuccessful ? "Delivered" : latest.DeliveringFailed ? $"Delivering Failed ({latest.DeliveringFailsCount} times)" : "Collecting...", latest.DeliveringSuccessful ? "success" : latest.DeliveringFailed ? "error" : "warn"));
      sb.AppendLine(Badge(GetHealthCheckStatus(hc).ToString(), StatusClass(GetHealthCheckStatus(hc))));
      sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Version:</span><span class=\"v\">{Html(VersionHelper.Version)}</span></span>");
      if (!string.IsNullOrWhiteSpace(hc.Type))
        sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Type:</span><span class=\"v\">{Html(hc.Type)}</span></span>");
      if (!string.IsNullOrWhiteSpace(hc.InitiatorName))
        sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Initiator:</span><span class=\"v\">{Html(hc.InitiatorName)}</span></span>");
      sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Created:</span><span class=\"v\">{Html(hc.CreationTime.ToString("u"))}</span></span>");
      sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Id:</span><span class=\"v mono\">{hc.Id}</span></span>");
      //sb.AppendLine($"<span class=\"kv\"><span class=\"k\">TenantId:</span><span class=\"v mono\">{latest.TenantId?.ToString() ?? "Host"}</span></span>");
      sb.AppendLine("</div>");
      sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Startup Type:</span><span class=\"v\">{Html(StartupDiagnostics.GetPrettyStartupType())}</span></span>");
      sb.AppendLine($"<span class=\"kv\"><span class=\"k\">Uptime:</span><span class=\"v\">{Html(StartupDiagnostics.GetUptime().ToString())}</span></span>");


      // Reports
      sb.AppendLine("<div class=\"reports\">");
      sb.AppendLine("<h3>Reports</h3>");
      if (hc.Reports?.Count > 0)
      {
        sb.AppendLine("<div class=\"table\">");
        sb.AppendLine("<div class=\"thead\">");
        sb.AppendLine("<div>Check</div><div>Status</div><div>Public</div><div>Message</div><div>Extra</div><div>Actions</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"tbody\">");
        foreach (var r in hc.Reports)
        {
          sb.AppendLine("<div class=\"tr\">");
          sb.AppendLine($"<div>{Html(r.CheckName)}</div>");
          sb.AppendLine($"<div>{Badge(r.Status.ToString(), StatusClass(r.Status))}</div>");
          sb.AppendLine($"<div>{(r.IsPublic ? "Yes" : "No")}</div>");
          sb.AppendLine($"<div class=\"message\">{Html(r.Message)}</div>");
          sb.AppendLine($"<div class=\"extra\"><pre>{Html(JsonSerializer.Serialize(r.ExtraInformation ?? new(), JsonOpts))}</pre></div>");
          sb.AppendLine($"<div class=\"table-actions\"><button class=\"btn action-button copy-btn\" data-report-json='{Html(JsonSerializer.Serialize(r, JsonOpts))}' data-service-name='{Html(r.ServiceName)}' data-check-name='{Html(r.CheckName)}'>Copy</button><button class=\"btn action-button download-btn\" data-report-json='{Html(JsonSerializer.Serialize(r, JsonOpts))}' data-service-name='{Html(r.ServiceName)}' data-check-name='{Html(r.CheckName)}'>Download</button></div>");
          sb.AppendLine("</div>");
        }
        sb.AppendLine("</div></div>");
      }
      else
      {
        sb.AppendLine("<p class=\"muted\">No reports.</p>");
      }
      sb.AppendLine("</div>");
    }
    else
    {
      sb.AppendLine("<p class=\"muted\">No results yet.</p>");
    }
    sb.AppendLine("</div></section>");

    // Footer + JS bootstrap (includes auto-refresh & trigger call)
    sb.AppendLine("<footer class=\"footer\">");
    sb.AppendLine("<span class=\"muted\">Auto-refreshes until delivered.</span>");
    sb.AppendLine("</footer>");

    sb.AppendLine("<script>");
    sb.AppendLine(GenerateJs(latest?.Collecting ?? true, pagePath));
    sb.AppendLine("</script>");

    sb.AppendLine("</div></body></html>");
    return sb.ToString();
  }

  public static string GenerateCss()
  {
    return @"
:root {
  --bg: #0f1115;
  --card: #151822;
  --text: #e8ecf1;
  --muted: #9aa4b3;
  --primary: #3b82f6;
  --success: #1b9e4b;
  --warn: #f59e0b;
  --error: #ef4444;
  --border: #232838;
  --mono: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
  --radius: 14px;
}
* { box-sizing: border-box; }
html, body { margin:0; padding:0; background:var(--bg); color:var(--text); font-family: system-ui, -apple-system, Segoe UI, Roboto, Ubuntu, 'Helvetica Neue', Arial, 'Noto Sans', 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol'; }
.container { max-width: 1100px; margin: 40px auto; padding: 0 20px; }
.header { display:flex; align-items: center; justify-content: space-between; gap:16px; margin-bottom: 24px; }
.header h1 { font-size: 26px; margin:0; }
.header .service { font-weight: 600; font-size: 14px; margin-left: 10px; padding: 4px 8px; background: #0b0d12; border: 1px solid var(--border); border-radius: 8px; color: var(--muted); }
.actions { display:flex; gap:10px; flex-wrap: wrap; }
.btn { appearance:none; border:1px solid var(--border); background:#0b0d12; color:var(--text); padding:10px 14px; border-radius: 10px; text-decoration:none; font-weight:600; font-size:14px; cursor:pointer; }
.btn:hover { border-color:#2f3650; }
.primary { background: var(--primary); border-color: transparent; }
.success { background: var(--success); border-color: transparent; }
.primary:hover { filter: brightness(1.1); }
.card { background:var(--card); border:1px solid var(--border); border-radius: var(--radius); overflow: hidden; }
.card-header { padding:14px 16px; font-weight:700; border-bottom:1px solid var(--border); background:#10131b; }
.card-body { padding:16px; }
.summary { display:flex; gap:10px; flex-wrap: wrap; align-items:center; margin-bottom: 10px; }
.badge { display:inline-flex; align-items:center; gap:6px; padding:6px 10px; border-radius:999px; font-weight:700; font-size:12px; border:1px solid var(--border); background:#0b0d12; }
.badge.success { background: rgba(34,197,94,0.15); border-color: rgba(34,197,94,0.35); }
.badge.warn { background: rgba(245,158,11,0.16); border-color: rgba(245,158,11,0.35); }
.badge.error { background: rgba(239,68,68,0.16); border-color: rgba(239,68,68,0.38); }
.kv { display:inline-flex; gap:6px; padding:6px 8px; border-radius:8px; border:1px dashed var(--border); background:#0b0d12; font-size:12px; }
.kv .k { color: var(--muted); }
.kv .v { font-weight:700; }
.mono { font-family: var(--mono); }
.reports { margin-top: 6px; }
.reports h3 { margin: 10px 0; font-size: 16px; }
//.table { display:grid; gap:0; border:1px solid var(--border); border-radius: 12px; overflow: hidden; }
//.thead, .tr { display:grid; grid-template-columns: 1.2fr 1.2fr 0.9fr 0.8fr 0.6fr 1.5fr 1.1fr; align-items: start; }
//.thead { background:#10131b; font-weight:700; color:var(--muted); border-bottom:1px solid var(--border); }
//.thead > div, .tr > div { padding: 10px 12px; border-bottom:1px solid var(--border); }
//.tbody .tr:nth-child(even) { background: #121625; }
.message { white-space: pre-wrap; word-break: break-word; }
.extra pre { margin:0; max-width: calc(100% - 0px); max-height: 180px; overflow:auto; font-family: var(--mono); font-size: 12px; background:#0b0d12; padding:10px; border-radius: 8px; border:1px solid var(--border); }
.muted { color: var(--muted); }
.footer { display:flex; justify-content:flex-end; margin-top: 12px; font-size:12px; color: var(--muted); }


/* --- Reports table layout --- */
.reports .table {
  border: 1px solid var(--panel-border, #2a2f3a);
  border-radius: 8px;
  overflow: hidden;
  background: var(--panel-bg, #0f1218);
}

/* Make header row and data rows share the same grid */
.reports .thead,
.reports .tbody .tr {
  display: grid;
  /* tweak these to taste */
  grid-template-columns:
      140px         /* Check    */
      90px          /* Status   */
      70px          /* Public   */
      180px  /* Message */
      380px /* Extra   */
      60px;         /* Actions  */
  gap: 12px;
  align-items: start;
}

.reports .thead {
  position: sticky;
  top: 0;
  z-index: 1;
  background: var(--panel-header, #141924);
  font-weight: 600;
  border-bottom: 1px solid var(--panel-border, #2a2f3a);
  padding: 12px;
}

.reports .tbody { display: grid; grid-auto-rows: auto; }
.reports .tbody .tr { padding: 12px; border-top: 1px solid rgba(255,255,255,0.06); }
.reports .tbody .tr:nth-child(even) { background: rgba(255,255,255,0.02); }

/* Status badge cell doesn’t need to wrap */
.reports .tbody .tr > div:nth-child(3) { white-space: nowrap; }

/* Message should wrap nicely */
.reports .message {
  white-space: pre-wrap;        /* keep newlines */
  word-break: break-word;       /* break long tokens */
  overflow-wrap: anywhere;
}

/* Extra: keep JSON readable but contained */
.reports .extra pre {
  margin: 0;
  padding: 8px;
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 6px;
  max-height: 220px;            /* prevents row from getting huge */
  overflow: auto;               /* scroll if it’s long/wide */
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, ""Liberation Mono"", monospace;
  font-size: 12.5px;
  line-height: 1.4;
}

.table-actions {
  display: flex;
  flex-direction: column; /* or row */
  gap: 6px;
  flex-wrap: wrap;
}

/* Small screens: stack “Message” and “Extra” to full width */
@media (max-width: 1100px) {
  .reports .thead,
  .reports .tbody .tr {
    grid-template-columns:
      120px 160px 80px 140px 60px;
  }
  .reports .message,
  .reports .extra {
    grid-column: 1 / -1;  /* take whole row */
  }
}

/* If viewport is narrower, allow horizontal scroll of the whole table */
.reports .table { overflow-x: auto; }
.reports .thead, .reports .tbody .tr { min-width: 980px; } /* ensures x-scroll appears */
";
  }

  public static string GenerateJs(bool collecting, string basePath)
  {
    // The page reloads every 10s until Delivered = true.
    return $@"
function replacePathBase(removePath, addPath) {{
  const u = new URL(window.location.href);

  // Normalize remove path
  if (!removePath.startsWith(""/"")) {{
    removePath = ""/"" + removePath;
  }}

  // Normalize add path
  if (!addPath.startsWith(""/"")) {{
    addPath = ""/"" + addPath;
  }}

  // Remove the given path if it matches the end of pathname
  if (u.pathname.endsWith(removePath)) {{
    u.pathname = u.pathname.slice(0, -removePath.length);
  }}

  // Ensure no trailing slash before adding new path
  if (u.pathname.endsWith(""/"")) {{
    u.pathname = u.pathname.slice(0, -1);
  }}

  // Handle case where addPath contains query (e.g. ""/?format=json"")
  let pathPart = addPath;
  let queryPart = """";
  if (addPath.includes(""?"")) {{
    const split = addPath.split(""?"");
    pathPart = split[0]; // may be ""/"" or something like ""/swagger""
    queryPart = split[1]; // everything after ?
  }}

  // Normalize pathPart
  if (pathPart && !pathPart.startsWith(""/"")) {{
    pathPart = ""/"" + pathPart;
  }}

  // Add new path
  if (u.pathname == '/'){{
    u.pathname = pathPart || """";
  }}
  else {{
    u.pathname += pathPart || """";
  }}


  // Replace query if provided
  if (queryPart) {{
    u.search = ""?"" + queryPart;
  }}

  return u.toString();
}}

// Function to copy text to clipboard
async function copyToClipboard(text) {{
  try {{
    if (navigator.clipboard && window.isSecureContext) {{
      await navigator.clipboard.writeText(text);
      return true;
    }} else {{
      // Fallback for older browsers or non-secure contexts
      const textArea = document.createElement('textarea');
      textArea.value = text;
      textArea.style.position = 'fixed';
      textArea.style.left = '-999999px';
      textArea.style.top = '-999999px';
      document.body.appendChild(textArea);
      textArea.focus();
      textArea.select();
      const result = document.execCommand('copy');
      document.body.removeChild(textArea);
      return result;
    }}
  }} catch (err) {{
    console.error('Failed to copy to clipboard:', err);
    return false;
  }}
}}

// Function to download JSON file
function downloadJsonFile(jsonData, filename) {{
  try {{
    const blob = new Blob([jsonData], {{ type: 'application/json' }});
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.style.display = 'none';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }} catch (err) {{
    console.error('Failed to download file:', err);
  }}
}}

// Function to generate filename with format: serviceName_checkName_date.json
function generateFilename(serviceName, checkName) {{
  const now = new Date();
  const dateStr = new Date().toISOString().replace(""T"", ""_"").replace(/:/g, ""-"").split(""."")[0];
  const sanitizedServiceName = (serviceName || 'unknown').replace(/[^a-zA-Z0-9_-]/g, '_');
  const sanitizedCheckName = (checkName || 'unknown').replace(/[^a-zA-Z0-9_-]/g, '_');
  return `${{sanitizedServiceName}}_${{sanitizedCheckName}}_${{dateStr}}.json`;
}}

// Function to show temporary feedback
function showFeedback(button, message, duration = 2000) {{
  const originalText = button.textContent;
  button.textContent = message;
  button.disabled = true;
  setTimeout(() => {{
    button.textContent = originalText;
    button.disabled = false;
  }}, duration);
}}

(function() {{
  var collecting = {(collecting ? "true" : "false")};
  if (collecting) {{
    setTimeout(function() {{ location.reload(); }}, 10000);
  }}

  var runBtn = document.getElementById('runBtn');
  if (runBtn) {{
    runBtn.addEventListener('click', async function() {{
      var url = replacePathBase('{basePath}', runBtn.getAttribute('data-url'));
      runBtn.disabled = true;
      runBtn.textContent = 'Running...';
      try {{
        await fetch(url, {{ method: 'POST' }});
      }} catch (e) {{
        console.error(e);
      }} finally {{
        // Give the backend a moment to enqueue/start, then refresh.
        setTimeout(function() {{ location.reload(); }}, 600);
      }}
    }});
  }}

  // Add event listeners for copy buttons
  document.addEventListener('click', async function(e) {{
    if (e.target.classList.contains('copy-btn')) {{
      e.preventDefault();
      const button = e.target;
      const reportJson = button.getAttribute('data-report-json');

      if (reportJson) {{
        const success = await copyToClipboard(reportJson);
        if (success) {{
          showFeedback(button, 'Copied!', 1500);
        }} else {{
          showFeedback(button, 'Failed', 1500);
        }}
      }}
    }}

    // Add event listeners for download buttons
    if (e.target.classList.contains('download-btn')) {{
      e.preventDefault();
      const button = e.target;
      const reportJson = button.getAttribute('data-report-json');
      const serviceName = button.getAttribute('data-service-name');
      const checkName = button.getAttribute('data-check-name');

      if (reportJson) {{
        const filename = generateFilename(serviceName, checkName);
        downloadJsonFile(reportJson, filename);
        showFeedback(button, 'Downloaded!', 1500);
      }}
    }}
  }});
}})();
";
  }

  // -------------- helpers --------------

  private static string Html(string? s)
      => string.IsNullOrEmpty(s) ? "" : HtmlEncoder.Default.Encode(s);

  private static string Badge(string text, string cls)
      => $"<span class=\"badge {Html(cls)}\">{Html(text)}</span>";

  private static string StatusClass(HealthCheckStatus status) => status switch
  {
    HealthCheckStatus.OK => "success",
    HealthCheckStatus.InProgress => "warn",
    HealthCheckStatus.Failed => "error",
    _ => "error"
  };

  private static HealthCheckStatus GetHealthCheckStatus(HealthCheckEto check)
  {
    return check.Reports.Any(x => x.Status == HealthCheckStatus.Failed) ? HealthCheckStatus.Failed :
        (check.Reports.Count > 0 && check.Reports.All(x => x.Status == HealthCheckStatus.OK)) ? HealthCheckStatus.OK : HealthCheckStatus.InProgress;
  }

  public static async Task HandleServiceRestart(HttpContext context)
  {
    var restartService = context.RequestServices.GetRequiredService<RestartService>();

    restartService.Restart();

    await context.Response.WriteAsync("Service is restarting...");

    return;
  }
}
