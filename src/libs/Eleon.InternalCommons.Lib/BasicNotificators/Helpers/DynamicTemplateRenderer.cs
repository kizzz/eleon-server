using Eleon.Logging.Lib.SystemLog.Logger;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.TextTemplating;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;

public static class DynamicTemplateRenderer
{
  public static string RenderTemplate(string template, Dictionary<string, string> args, string type = "replace")
  {
    if (string.Equals(type, "scriban", StringComparison.OrdinalIgnoreCase))
    {
      return RenderWithScriban(template, args);
    }
    else
    {
      return RenderWithReplace(template, args);
    }
  }
  public static string RenderWithReplace(string template, Dictionary<string, string> args)
  {
    ArgumentNullException.ThrowIfNull(template, nameof(template));

    // Replace ExtraProperties with pattern {property.key}
    var result = System.Text.RegularExpressions.Regex.Replace(
        template,
        @"\{([^}]+)\}",
        match =>
        {
          var propertyKey = match.Groups[1].Value;

          var foundProperty = args.FirstOrDefault(p =>
                  string.Equals(p.Key, propertyKey, StringComparison.OrdinalIgnoreCase));

          return foundProperty.Key != null ? foundProperty.Value : string.Empty;
        },
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    return result;
  }

  public static string RenderWithScriban(string template, Dictionary<string, string> args)
  {
    // Create Scriban context and register arguments as variables
    var context = new TemplateContext();
    var scriptObject = new ScriptObject();

    foreach (var kvp in args)
    {
      scriptObject.Add(kvp.Key, kvp.Value);
    }

    context.PushGlobal(scriptObject);

    // Parse and render the template
    var parsed = Template.Parse(template);
    if (parsed.HasErrors)
    {
      var errors = string.Join(", ", parsed.Messages.Select(x => x.Message));
      throw new InvalidOperationException($"Template parse errors: {errors}");
    }

    return parsed.Render(context);
  }
}
