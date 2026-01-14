using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Templating.Module.Eleon.Templating.Module.Domain.Constanst;
using Eleon.Templating.Module.Templates;
using Logging.Module;
using Scriban;
using Scriban.Runtime;
using System.Text.Json;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Eleon.Templating.Module.Templates;

public class TemplateManager : DomainService
{
  private readonly ITemplateRepository _templateRepository;
  private readonly IVportalLogger<TemplateManager> _logger;

  public TemplateManager(ITemplateRepository templateRepository, IVportalLogger<TemplateManager> logger)
  {
    _templateRepository = templateRepository;
    _logger = logger;
  }

  public async Task<KeyValuePair<int, List<Template>>> GetListAsync(
    string sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    string searchQuery = null,
      TemplateType? type = null,
      TextFormat? format = null)
  {
    List<Template> systemTemplates = [];
    KeyValuePair<int, List<Template>> result = default;
    try
    {
      if (skipCount == 0 && type.HasValue)
      {
        systemTemplates = GetList(type.Value);
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
          systemTemplates = systemTemplates
              .Where(t => t.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
              .ToList();
        }
      }
      var dbTemplates = await _templateRepository.GetListAsync(
          sorting,
          maxResultCount,
          skipCount,
          searchQuery,
          type,
          format
         );

      var allTemplates = dbTemplates.Value;

      foreach (var sysTemplate in systemTemplates)
      {
        if (!allTemplates.Any(t => t.Name == sysTemplate.Name && t.Type == sysTemplate.Type))
        {
          allTemplates.Insert(0, sysTemplate);
        }
      }

      result = new KeyValuePair<int, List<Template>>(
          systemTemplates.Count,
          allTemplates.OrderByDescending(t => t.IsSystem).ToList());
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }

    return result;
  }

  public async Task<Template> CreateAsync(
        Template? templateToAdd = null,
        CancellationToken cancellationToken = default)
  {
    // Validate uniqueness of (Name, Type)
    var existing = await _templateRepository.FindByNameAndTypeAsync(templateToAdd.Name, templateToAdd.Type, cancellationToken);

    if (existing != null)
    {
      throw new BusinessException(ModuleErrorCodes.TemplateNameTypeAlreadyExists)
          .WithData("Name", templateToAdd.Name)
          .WithData("Type", templateToAdd.Type);
    }
    var isSystem = false;
    existing = FindConstantById(templateToAdd.Id, templateToAdd.Type);
    if (existing != null)
    {
      isSystem = true;
    }
    // Validate format rules
    ValidateFormatForType(templateToAdd.Type, templateToAdd.Format);

    var templateId = existing?.Id ?? GuidGenerator.Create();
    var template = new Template(templateId);

    template.Name = templateToAdd.Name;
    template.Type = templateToAdd.Type;
    template.Format = templateToAdd.Format;
    template.TemplateContent = templateToAdd.TemplateContent;
    template.RequiredPlaceholders = templateToAdd.RequiredPlaceholders;
    template.IsSystem = isSystem;
    template.TemplateId = templateToAdd.TemplateId;

    // Validate template content
    await ValidateTemplateAsync(template, cancellationToken);

    return await _templateRepository.InsertAsync(template, cancellationToken: cancellationToken);
  }

  public async Task<Template> UpdateAsync(
      Template templateToUpdate,
      CancellationToken cancellationToken = default)
  {
    Template template = null;

    if (templateToUpdate.IsSystem)
    {
      template = await _templateRepository.FindAsync(templateToUpdate.Id, cancellationToken: cancellationToken);
      if (template == null)
      {
        return await CreateAsync(templateToUpdate, cancellationToken);
      }
    }
    else
    {
      template = await _templateRepository.GetAsync(templateToUpdate.Id, cancellationToken: cancellationToken);
    }

    // Validate uniqueness of (Name, Type) if name or type changed
    if (template.Name != templateToUpdate.Name || template.Type != templateToUpdate.Type)
    {
      var existing = await _templateRepository.FindByNameAndTypeAsync(templateToUpdate.Name, templateToUpdate.Type, cancellationToken);
      if (existing != null && existing.Id == templateToUpdate.Id)
      {
        existing = null; // Same entity, allow update
      }

      if (existing != null)
      {
        throw new BusinessException(ModuleErrorCodes.TemplateNameTypeAlreadyExists)
            .WithData("Name", templateToUpdate.Name)
            .WithData("Type", templateToUpdate.Type);
      }
    }

    // Validate format rules
    ValidateFormatForType(templateToUpdate.Type, templateToUpdate.Format);

    template.Update(templateToUpdate.Name, templateToUpdate.Type, templateToUpdate.Format, templateToUpdate.TemplateContent, templateToUpdate.RequiredPlaceholders, templateToUpdate.TemplateId);

    // Validate template content
    await ValidateTemplateAsync(template, cancellationToken);

    return await _templateRepository.UpdateAsync(template, cancellationToken: cancellationToken);
  }

  public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var template = await _templateRepository.GetAsync(id, cancellationToken: cancellationToken);

    // Prevent deleting system templates
    if (template.IsSystem)
    {
      throw new BusinessException(ModuleErrorCodes.CannotDeleteSystemTemplate);
    }

    await _templateRepository.DeleteAsync(template, cancellationToken: cancellationToken);
  }

  public async Task<Template> GetByNameAndTypeAsync(
      string name,
      TemplateType type,
      CancellationToken cancellationToken = default)
  {
    var template = await _templateRepository.FindByNameAndTypeAsync(name, type, cancellationToken) ?? FindConstantByName(name, type);

    if (template == null)
    {
      throw new BusinessException(ModuleErrorCodes.TemplateNotFound)
          .WithData("Name", name)
          .WithData("Type", type);
    }

    return template;
  }

  public async Task<Template> ResetAsync(
    Guid id,
    TemplateType type,
    CancellationToken cancellationToken = default)
  {
    var system = FindConstantById(id, type);

    return system ?? throw new BusinessException(ModuleErrorCodes.TemplateNotFound)
          .WithData("Name", id)
          .WithData("Type", type);
  }


  public async Task<string> ApplyTemplateAsync(
      string templateName,
      TemplateType templateType,
      Dictionary<string, string> placeholders,
      CancellationToken cancellationToken = default)
  {
    var template = await _templateRepository.FindByNameAndTypeAsync(templateName, templateType, cancellationToken: cancellationToken) ?? FindConstantByName(templateName, templateType);

    if (template == null)
    {
      throw new BusinessException(ModuleErrorCodes.TemplateNotFound)
          .WithData("Name", templateName)
          .WithData("Type", templateType);
    }

    return template.Format switch
    {
      TextFormat.Plaintext => ApplyPlaintextTemplate(template.TemplateContent, placeholders),
      TextFormat.Scriban => ApplyScribanTemplate(template.TemplateContent, placeholders),
      TextFormat.Json => ApplyJsonTemplate(template.TemplateContent, placeholders),
      _ => throw new BusinessException(ModuleErrorCodes.InvalidTemplateFormat)
          .WithData("Format", template.Format)
    };
  }

  public async Task ValidateTemplateAsync(
      Template template,
      CancellationToken cancellationToken = default)
  {
    // Validate JSON format
    if (template.Format == TextFormat.Json)
    {
      try
      {
        JsonDocument.Parse(template.TemplateContent);
      }
      catch (JsonException ex)
      {
        throw new BusinessException(ModuleErrorCodes.InvalidJsonTemplate)
            .WithData("Error", ex.Message);
      }
    }

    // Validate required placeholders for system templates
    if (template.IsSystem && !string.IsNullOrWhiteSpace(template.RequiredPlaceholders))
    {
      var requiredPlaceholders = template.RequiredPlaceholders
          .Split(';', StringSplitOptions.RemoveEmptyEntries)
          .Select(p => p.Trim())
          .Where(p => !string.IsNullOrWhiteSpace(p))
          .ToList();

      var templatePlaceholders = ExtractPlaceholders(template.TemplateContent, template.Format);

      var missingPlaceholders = requiredPlaceholders
          .Where(rp => !templatePlaceholders.Contains(rp, StringComparer.OrdinalIgnoreCase))
          .ToList();

      if (missingPlaceholders.Any())
      {
        throw new BusinessException(ModuleErrorCodes.MissingRequiredPlaceholder)
            .WithData("Placeholders", string.Join(", ", missingPlaceholders));
      }
    }
  }

  private void ValidateFormatForType(TemplateType type, TextFormat format)
  {
    var isValid = type switch
    {
      TemplateType.Action => format == TextFormat.Plaintext || format == TextFormat.Json,
      TemplateType.Notification => format == TextFormat.Plaintext || format == TextFormat.Scriban,
      _ => false
    };

    if (!isValid)
    {
      throw new BusinessException(ModuleErrorCodes.InvalidFormatForTemplateType)
          .WithData("Type", type)
          .WithData("Format", format);
    }
  }

  private string ApplyPlaintextTemplate(string template, Dictionary<string, string> placeholders)
  {
    return Regex.Replace(
        template,
        @"\{([^}]+)\}",
        match =>
        {
          var placeholderKey = match.Groups[1].Value;
          var found = placeholders.FirstOrDefault(p =>
                  string.Equals(p.Key, placeholderKey, StringComparison.OrdinalIgnoreCase));

          return found.Key != null ? found.Value : string.Empty;
        },
        RegexOptions.IgnoreCase);
  }

  private string ApplyScribanTemplate(string template, Dictionary<string, string> placeholders)
  {
    var context = new TemplateContext();
    var scriptObject = new ScriptObject();

    foreach (var kvp in placeholders)
    {
      scriptObject.Add(kvp.Key, kvp.Value);
    }

    context.PushGlobal(scriptObject);

    var parsed = Scriban.Template.Parse(template);
    if (parsed.HasErrors)
    {
      var errors = string.Join(", ", parsed.Messages.Select(x => x.Message));
      throw new BusinessException(ModuleErrorCodes.InvalidTemplateFormat)
          .WithData("Error", errors);
    }

    return parsed.Render(context);
  }

  private string ApplyJsonTemplate(string template, Dictionary<string, string> placeholders)
  {
    // Validate JSON first
    try
    {
      JsonDocument.Parse(template);
    }
    catch (JsonException ex)
    {
      throw new BusinessException(ModuleErrorCodes.InvalidJsonTemplate)
          .WithData("Error", ex.Message);
    }

    // Then replace placeholders as plaintext
    return ApplyPlaintextTemplate(template, placeholders);
  }

  private List<string> ExtractPlaceholders(string template, TextFormat format)
  {
    return format switch
    {
      TextFormat.Plaintext or TextFormat.Json => ExtractPlaintextPlaceholders(template),
      TextFormat.Scriban => ExtractScribanPlaceholders(template),
      _ => new List<string>()
    };
  }

  private List<string> ExtractPlaintextPlaceholders(string template)
  {
    var matches = Regex.Matches(template, @"\{([^}]+)\}");
    return matches
        .Cast<Match>()
        .Select(m => m.Groups[1].Value.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
  }

  private List<string> ExtractScribanPlaceholders(string template)
  {
    var placeholders = new List<string>();
    var parsed = Scriban.Template.Parse(template);

    if (parsed.HasErrors)
    {
      return placeholders;
    }

    // Extract variables from Scriban template
    // This is a simplified approach - Scriban templates can have complex expressions
    var variableMatches = Regex.Matches(template, @"\{\{\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*\}\}");
    return variableMatches
        .Cast<Match>()
        .Select(m => m.Groups[1].Value.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
  }
  private List<Template> GetList(TemplateType type)
  {
    if (type == TemplateType.Action)
    {
      return SystemTemplatesConsts.ActionTemplates.Values.ToList();
    }
    else if (type == TemplateType.Notification)
    {
      return SystemTemplatesConsts.NotificationTemplates.Values.ToList();
    }

    throw new Exception("Unsupported template type.");
  }

  private Template? FindConstantById(Guid id, TemplateType type)
  {
    if (type == TemplateType.Action)
    {
      if (SystemTemplatesConsts.ActionTemplates.TryGetValue(id, out var template))
      {
        return template;
      }
    }
    else if (type == TemplateType.Notification)
    {
      if (SystemTemplatesConsts.NotificationTemplates.TryGetValue(id, out var template))
      {
        return template;
      }
    }

    return null;
  }
  private Template? FindConstantByName(string Name, TemplateType type)
  {
    if (type == TemplateType.Action)
    {
      return SystemTemplatesConsts.ActionTemplates.SingleOrDefault(x => x.Value.Name == Name).Value;
    }
    else if (type == TemplateType.Notification)
    {
      return SystemTemplatesConsts.NotificationTemplates.SingleOrDefault(x => x.Value.Name == Name).Value;
    }

    return null;
  }
}

