using Eleon.Common.Lib.Helpers;
using Eleon.Logging.Lib.SystemLog.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedModule.modules.AppSettings.Module;
public static class SettingsLoader
{
  private sealed record LayerSpec(
      string SettingsPath,
      string? ImportsBasePath,
      string[] ImportFiles,
      bool SuppressErrors = false
  );

  private static string RemovePostFix(this string input, string postFix)
  {
    if (input.EndsWith(postFix, StringComparison.OrdinalIgnoreCase))
    {
      return input.Substring(0, input.Length - postFix.Length);
    }
    return input;
  }

  public static IConfigurationRoot BuildConfig(
      string contentRoot,
      string baseSettingsFile,
      string environmentSettingsFile,
      string importsSection,
      bool reloadOnChange)
  {
    // Normalize entry points to absolute paths
    var normalizedBase = ToAbs(contentRoot, baseSettingsFile);
    var normalizedEnv = ToAbs(contentRoot, environmentSettingsFile);
    var resolvedSettings = ToAbs(contentRoot, environmentSettingsFile.RemovePostFix(".json") + ".Merged.json");

    if (File.Exists(resolvedSettings))
    {
      try
      {
        EleonsoftLog.Info($"Using pre-resolved settings file: {resolvedSettings}");
        return new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile(resolvedSettings, optional: false, reloadOnChange: reloadOnChange)
            .Build();
      }
      catch (Exception ex)
      {
        EleonsoftLog.Error("Error loading pre-resolved settings file, falling back to full resolution", ex);
      }
    }

    // 1) Merge BASE then ENV to get the effective ConfigImports & BaseSettings (ENV overrides BASE)
    var mergedTemp = new ConfigurationBuilder()
        .AddJsonFile(normalizedBase, optional: true, reloadOnChange: false)
        .AddJsonFile(normalizedEnv, optional: true, reloadOnChange: false)
        .Build();

    var effectiveImportFiles = mergedTemp.GetSection($"{importsSection}:Files").Get<string[]>() ?? Array.Empty<string>();
    var effectiveImportsBasePath = ExpandPath(mergedTemp[$"{importsSection}:BasePath"]);
    var parentRel = ExpandPath(mergedTemp[$"{importsSection}:BaseSettings"]);

    // 2) Collect parent chain if provided (oldest → newest)
    var builder = new ConfigurationBuilder().SetBasePath(contentRoot);

    bool allAddedSuccessfully = false;

    if (!string.IsNullOrWhiteSpace(parentRel))
    {
      var parentAbs = ToAbs(Path.GetDirectoryName(normalizedBase)!, parentRel!);
      var parentLayers = CollectLayersRecursive(
          entrySettingsPath: parentAbs,
          importsSection: importsSection,
          maxDepth: 32
      );

      allAddedSuccessfully = parentLayers.Key;

      foreach (var layer in parentLayers.Value)
      {
        // imports of the parent layer
        if (layer.ImportFiles.Length > 0)
        {
          var basePath = ResolveBasePathForLayer(layer.SettingsPath, layer.ImportsBasePath ?? string.Empty);
          foreach (var import in layer.ImportFiles)
          {
            var importAbs = ToAbs(basePath, import);
            allAddedSuccessfully &= builder.TryAddSafe(importAbs, optional: true, reloadOnChange: reloadOnChange) || layer.SuppressErrors;
          }
        }

        allAddedSuccessfully &= builder.TryAddSafe(layer.SettingsPath, optional: true, reloadOnChange: reloadOnChange) || layer.SuppressErrors;
      }
    }

    // 3) Apply effective imports for BASE (overridden by ENV), then BASE, then ENV last (top-most)
    if (!string.IsNullOrWhiteSpace(effectiveImportsBasePath) && effectiveImportFiles.Length > 0)
    {
      var importsRoot = ResolveBasePathForLayer(normalizedBase, effectiveImportsBasePath!);
      foreach (var import in effectiveImportFiles)
      {
        var importAbs = ToAbs(importsRoot, import);
        allAddedSuccessfully &= builder.TryAddSafe(importAbs, optional: true, reloadOnChange: reloadOnChange);
      }
    }

    allAddedSuccessfully &= builder.TryAddSafe(normalizedBase, reloadOnChange: reloadOnChange);
    allAddedSuccessfully &= builder.TryAddSafe(normalizedEnv, reloadOnChange: reloadOnChange);

    var config = builder.Build();

    try
    {
      if (allAddedSuccessfully)
      {
        SaveMerged(config, resolvedSettings, importsSection);
      }
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Error during settings loading", ex);
    }

    return config;
  }

  private static bool TryAddSafe(this IConfigurationBuilder builder, string path, bool optional = true, bool reloadOnChange = false)
  {
    if (File.Exists(path))
    {
      try
      {
        // executing this to verify that the file is valid JSON
        var validCfg = new ConfigurationBuilder().AddJsonFile(path, optional: false, reloadOnChange: false).Build();

        builder.AddJsonFile(path, optional: optional, reloadOnChange: reloadOnChange);
      }
      catch (Exception)
      {
        EleonsoftLog.Warn($"Skipping missing configuration file: {path}");
        return false;
      }
    }
    else
    {
      EleonsoftLog.Warn($"Skipping missing configuration file: {path}");
      return false;
    }

    return true;
  }

  // -------------------- internals --------------------

  private static void SaveMerged(IConfiguration config, string path, string importsSection)
  {
    static object? Coerce(string? s)
    {
      if (s is null) return null;
      var t = s.Trim();

      if (string.Equals(t, "null", StringComparison.OrdinalIgnoreCase)) return null;

      // bool
      if (bool.TryParse(t, out var b)) return b;

      // integer (long first to keep big numbers intact)
      if (long.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l)) return l;

      // floating point
      if (double.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d)) return d;

      // TimeSpan (e.g., "00:01:00")
      if (TimeSpan.TryParse(t, CultureInfo.InvariantCulture, out var ts)) return t; // keep as string unless you want ISO (write t)

      // fallback string
      return s;
    }

    static bool IsUnder(string path, string section)
        => path.Equals(section, StringComparison.OrdinalIgnoreCase) ||
           path.StartsWith(section + ":", StringComparison.OrdinalIgnoreCase);

    static object? BuildObject(IConfiguration section, string importsSection)
    {
      // Skip the imports subtree entirely
      if (IsUnder(section is IConfigurationSection sec ? sec.Path : string.Empty, importsSection))
        return null;

      var children = section.GetChildren().ToList();
      if (children.Count == 0)
      {
        var cfgSec = section as IConfigurationSection;
        return Coerce(cfgSec?.Value);
      }

      // Detect array: keys "0","1","2",...
      var allIndexes = new List<(int idx, IConfigurationSection child)>();
      bool looksLikeArray = true;
      foreach (var c in children)
      {
        if (int.TryParse(c.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx))
          allIndexes.Add((idx, c));
        else
        {
          looksLikeArray = false;
          break;
        }
      }

      if (looksLikeArray)
      {
        allIndexes.Sort((a, b) => a.idx.CompareTo(b.idx));
        var list = new List<object?>(allIndexes.Count);
        foreach (var (_, c) in allIndexes)
        {
          var v = BuildObject(c, importsSection);
          // Skip nulls from filtered branches
          if (v is not null || c.GetChildren().Any() || c.Value is not null)
            list.Add(v);
        }
        return list;
      }

      var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
      foreach (var c in children)
      {
        var v = BuildObject(c, importsSection);
        if (v is null && !c.GetChildren().Any() && c.Value is null) continue; // filtered out
        dict[c.Key] = v;
      }
      return dict;
    }

    var rootObj = BuildObject(config, importsSection) ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    var json = JsonSerializer.Serialize(rootObj, new JsonSerializerOptions
    {
      WriteIndented = true
    });

    File.WriteAllText(path, json);
  }



  private static KeyValuePair<bool, List<LayerSpec>> CollectLayersRecursive(
      string entrySettingsPath,
      string importsSection,
      int maxDepth)
  {
    var hasMissedFiles = false;
    var result = new List<LayerSpec>();
    var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    void Dfs(string settingsPath, int depth)
    {
      var abs = Path.GetFullPath(settingsPath);

      if (seen.Contains(abs))
        return;

      if (!File.Exists(abs))
      {
        hasMissedFiles = true;
        return; // skip missing
      }

      if (depth > maxDepth)
        throw new InvalidOperationException($"Settings recursion exceeded max depth {maxDepth}. Last: {abs}");

      if (!visiting.Add(abs))
        throw new InvalidOperationException($"Cycle detected while resolving BaseSettings chain. File: {abs}");

      // TEMP read to discover BaseSettings + imports
      var temp = new ConfigurationBuilder()
          .AddJsonFile(abs, optional: false, reloadOnChange: false)
          .Build();

      // BaseSettings may be at top-level or inside imports section
      var baseSettingsRel = ExpandPath(temp[$"{importsSection}:BaseSettings"]);

      // Recurse into parent FIRST
      if (!string.IsNullOrWhiteSpace(baseSettingsRel))
      {
        var parentAbs = ToAbs(Path.GetDirectoryName(abs)!, baseSettingsRel!);
        Dfs(parentAbs, depth + 1);
      }

      // Gather this layer's imports (BasePath/Files)
      var importsBasePath = ExpandPath(temp[$"{importsSection}:BasePath"]);
      var importFiles = temp.GetSection($"{importsSection}:Files").Get<string[]>() ?? Array.Empty<string>();
      var suppressErrors = bool.TryParse(temp[$"{importsSection}:SuppressErrors"], out var suppressVal) && suppressVal;

      result.Add(new LayerSpec(abs, importsBasePath, importFiles, suppressErrors));

      visiting.Remove(abs);
      seen.Add(abs);
    }

    Dfs(entrySettingsPath, 0);

    // At this point, 'result' is ordered from oldest parent → to the entry file (child)
    return KeyValuePair.Create(!hasMissedFiles, result);
  }

  private static string ResolveBasePathForLayer(string layerSettingsPath, string declaredBasePath)
  {
    // If declaredBasePath is absolute, keep it; otherwise resolve relative to the layer file's directory
    if (Path.IsPathRooted(declaredBasePath))
      return Path.GetFullPath(declaredBasePath);

    var dir = Path.GetDirectoryName(layerSettingsPath)!;
    return Path.GetFullPath(Path.Combine(dir, declaredBasePath));
  }

  private static string ToAbs(string baseDirOrAbs, string maybeRelative)
  {
    if (string.IsNullOrWhiteSpace(maybeRelative))
      return baseDirOrAbs; // nothing to resolve

    maybeRelative = ExpandPath(maybeRelative);

    if (Path.IsPathRooted(maybeRelative))
      return Path.GetFullPath(maybeRelative);

    return Path.GetFullPath(Path.Combine(baseDirOrAbs, maybeRelative));
  }

  private static string ExpandPath(string? raw)
  {
    if (string.IsNullOrWhiteSpace(raw))
      return string.Empty;

    var expanded = Environment.ExpandEnvironmentVariables(raw);

    if (expanded.StartsWith("~"))
    {
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      if (expanded.Length == 1)
        expanded = home;
      else if (expanded[1] == Path.DirectorySeparatorChar || expanded[1] == Path.AltDirectorySeparatorChar)
        expanded = Path.Combine(home, expanded.Substring(2));
    }

    return expanded;
  }

  public static IConfiguration LoadConfiguration(string? profile = null, bool reloadOnChange = true)
  {
    try
    {
      EleonsoftLog.Info("Loading application configuration...");

      var envConfig = "appsettings.json";
      if (!string.IsNullOrWhiteSpace(profile))
      {
        envConfig = $"appsettings.{profile}.json";
      }

      var config = BuildConfig(
          contentRoot: AppContext.BaseDirectory,
          baseSettingsFile: "appsettings.app.json",
          environmentSettingsFile: envConfig,
          importsSection: "ConfigImports",
          reloadOnChange: reloadOnChange);

      var errors = ValidateConfiguration(config);
      if (errors.Any())
      {
        EleonsoftLog.Error($"Configuration validation errors: {string.Join("; ", errors.Select(x => $"{x.Key}: {x.Value}"))}");
      }

      return config;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Info($"Configuration loading failed: {ex.Message}");
      return null;
    }
  }

  // Valdidate known sections

  public static Dictionary<string, string> ValidateConfiguration(IConfiguration configuration)
  {
    try
    {
      var result = new Dictionary<string, string>();

      if (configuration.GetSection("EventBus").Exists())
      {
        var errors = ValidateEventBus("EventBus", configuration);
        if (!string.IsNullOrWhiteSpace(errors))
        {
          result.Add("EventBus", errors);
        }
      }

      if (configuration.GetSection("SqlServer").Exists())
      {
        var errors = ValidateSql("SqlServer", configuration);
        if (!string.IsNullOrWhiteSpace(errors))
        {
          result.Add("SqlServer", errors);
        }
      }

      if (configuration.GetSection("Logger").Exists())
      {
        var errors = ValidateLogger("Logger", configuration);
        if (!string.IsNullOrWhiteSpace(errors))
        {
          result.Add("Logger", errors);
        }
      }

      if (configuration.GetSection("EleoncoreSdk").Exists())
      {
        var errors = ValidateSdk("EleoncoreSdk", configuration);
        if (!string.IsNullOrWhiteSpace(errors))
        {
          result.Add("EleoncoreSdk", errors);
        }
      }

      return result;
    }
    catch (Exception ex)
    {
      return new Dictionary<string, string> { { "ExecptionDuringValidationLoadedSettings", $"Exception during configuration validation: {ex.Message}" } };
    }
  }

  private static string ValidateEventBus(string sectionName, IConfiguration configuration)
  {
    var errorMessage = string.Empty;
    var result = ValidationHelper.Validate(ValidationHelper.Validators.Numeric, $"{sectionName}:Provider", configuration);
    if (result.IsErrored)
    {
      errorMessage += $"{sectionName}:Provider is not numeric. ";
    }

    {
      // todo if rabbitmq

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Host", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Host is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Port", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Port is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:VirtualHost", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:VirtualHost is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Username", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Username is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Password", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:Password is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Boolean, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:UseSsl", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:UseSsl is required. ";
      }

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:DefaultQueuePrefix", configuration);
      if (result.IsErrored)
      {
        errorMessage += $"{sectionName}:ProviderOptionsJson:RabbitMqOptions:DefaultQueuePrefix is required. ";
      }
    }

    return errorMessage;
  }

  private static string ValidateSql(string sectionName, IConfiguration configuration)
  {
    var errorMessage = string.Empty;
    var result = ValidationHelper.Validate(ValidationHelper.Validators.Numeric, $"{sectionName}:CompatibilityLevel", configuration);
    if (result.IsErrored)
    {
      errorMessage += $"{sectionName}:CompatibilityLevel is not numeric. ";
    }

    return errorMessage;
  }

  private static string ValidateSdk(string sectionName, IConfiguration configuration)
  {
    var errorMessage = string.Empty;

    // BaseHost (url)
    var result = ValidationHelper.Validate(ValidationHelper.Validators.Url, $"{sectionName}:BaseHost", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:BaseHost is not a valid url. ";

    // BaseEleonsoftHost (url)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Url, $"{sectionName}:BaseEleonsoftHost", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:BaseEleonsoftHost is not a valid url. ";

    // BasePath (optional) – no validation (can be empty)

    // UseOAuthAuthorization (boolean)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Boolean, $"{sectionName}:UseOAuthAuthorization", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:UseOAuthAuthorization must be boolean. ";

    // OAuthUrl required & must be url when UseOAuthAuthorization == true
    var useOAuth = bool.TryParse(configuration[$"{sectionName}:UseOAuthAuthorization"], out var useOAuthVal) && useOAuthVal;
    if (useOAuth)
    {
      result = ValidationHelper.Validate(ValidationHelper.Validators.Url, $"{sectionName}:OAuthUrl", configuration);
      if (result.IsErrored)
        errorMessage += $"{sectionName}:OAuthUrl is required and must be a valid url when UseOAuthAuthorization is true. ";
    }

    // UseApiAuthorization (boolean)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Boolean, $"{sectionName}:UseApiAuthorization", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:UseApiAuthorization must be boolean. ";

    // ApiAuthUrl/ApiKey/ApiKeySecret required when UseApiAuthorization == true
    var useApiAuth = bool.TryParse(configuration[$"{sectionName}:UseApiAuthorization"], out var useApiAuthVal) && useApiAuthVal;
    if (useApiAuth)
    {
      result = ValidationHelper.Validate(ValidationHelper.Validators.Url, $"{sectionName}:ApiAuthUrl", configuration);
      if (result.IsErrored)
        errorMessage += $"{sectionName}:ApiAuthUrl is required and must be a valid url when UseApiAuthorization is true. ";

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ApiKey", configuration);
      if (result.IsErrored)
        errorMessage += $"{sectionName}:ApiKey is required when UseApiAuthorization is true. ";

      result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:ApiKeySecret", configuration);
      if (result.IsErrored)
        errorMessage += $"{sectionName}:ApiKeySecret is required when UseApiAuthorization is true. ";
    }

    // SecretKey (required)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:SecretKey", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:SecretKey is required. ";

    // AppKey (required)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:AppKey", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:AppKey is required. ";

    // IgnoreSslValidation (boolean)
    result = ValidationHelper.Validate(ValidationHelper.Validators.Boolean, $"{sectionName}:IgnoreSslValidation", configuration);
    if (result.IsErrored)
      errorMessage += $"{sectionName}:IgnoreSslValidation must be boolean. ";

    return errorMessage;
  }

  private static string ValidateLogger(string sectionName, IConfiguration configuration)
  {
    var errorMessage = string.Empty;
    var result = ValidationHelper.Validate(ValidationHelper.Validators.Required, $"{sectionName}:Level", configuration);
    if (result.IsErrored)
    {
      errorMessage += $"{sectionName}:Level is required. ";
    }

    return errorMessage;
  }

  public static IConfiguration ReadFromFiles(bool optional = true, bool reloadOnChange = true, params string[] fileNames)
  {
    var builder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory);
    foreach (var fileName in fileNames)
    {
      builder.AddJsonFile(fileName, optional, reloadOnChange);
    }
    return builder.Build();
  }
}
