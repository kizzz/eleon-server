using System;
using System.Collections.Generic;
using System.IO;
using Common.EventBus.Module;
using Eleon.Storage.Lib.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using SharedModule.modules.Blob.Module.Models;
using Volo.Abp.EventBus.Distributed;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Test helpers for Storage Module testing.
/// </summary>
public static class StorageTestHelpers
{
    /// <summary>
    /// Creates a temporary directory for FileSystem tests.
    /// Returns the directory path. Caller is responsible for cleanup.
    /// </summary>
    public static string CreateTempDirectory(string prefix = "StorageTest")
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{prefix}_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    /// <summary>
    /// Safely deletes a temporary directory and all its contents.
    /// </summary>
    public static void DeleteTempDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // Ignore deletion errors in tests
        }
    }

    /// <summary>
    /// Sets up event bus mock to return a StorageProviderDto response for GetStorageProviderMsg.
    /// </summary>
    public static void SetupGetStorageProviderResponse(
        IResponseCapableEventBus eventBus,
        StorageProviderDto provider)
    {
        var response = BuildGetStorageProviderResponse(provider);
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            eventBus, response);
    }

    /// <summary>
    /// Sets up event bus mock to return a StorageProviderDto response based on provider ID.
    /// </summary>
    public static void SetupGetStorageProviderResponse(
        IResponseCapableEventBus eventBus,
        Func<GetStorageProviderMsg, GetStorageProviderResponseMsg> responseFactory)
    {
        EventBusTestHelpers.SetupEventBusRequestAsync<GetStorageProviderMsg, GetStorageProviderResponseMsg>(
            eventBus, responseFactory);
    }


    /// <summary>
    /// Builds a StorageProviderDto for testing.
    /// </summary>
    public static StorageProviderDto BuildStorageProviderDto(
        string type = StorageProviderDomainConstants.StorageTypeFileSystem,
        Dictionary<string, string> settings = null,
        Guid? id = null,
        string name = "Test Provider")
    {
        settings ??= new Dictionary<string, string>();
        
        var settingsList = new List<StorageProviderSettingDto>();
        foreach (var kvp in settings)
        {
            settingsList.Add(new StorageProviderSettingDto
            {
                Key = kvp.Key,
                Value = kvp.Value,
                Id = Guid.NewGuid(),
                StorageProviderId = id ?? Guid.NewGuid()
            });
        }

        return new StorageProviderDto
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            StorageProviderTypeName = type,
            IsActive = true,
            IsTested = false,
            Settings = settingsList
        };
    }

    /// <summary>
    /// Builds a GetStorageProviderResponseMsg for testing.
    /// </summary>
    public static GetStorageProviderResponseMsg BuildGetStorageProviderResponse(
        StorageProviderDto provider)
    {
        return new GetStorageProviderResponseMsg
        {
            StorageProvider = provider
        };
    }

    /// <summary>
    /// Builds FileSystem settings dictionary.
    /// </summary>
    public static Dictionary<string, string> BuildFileSystemSettings(string basePath)
    {
        return new Dictionary<string, string>
        {
            { "BasePath", basePath }
        };
    }

    /// <summary>
    /// Builds SFTP settings dictionary.
    /// </summary>
    public static Dictionary<string, string> BuildSftpSettings(
        string host,
        string user,
        string basePath,
        int port = 22,
        string password = null)
    {
        var settings = new Dictionary<string, string>
        {
            { "Host", host },
            { "Port", port.ToString() },
            { "UserName", user },
            { "BasePath", basePath }
        };

        if (!string.IsNullOrWhiteSpace(password))
        {
            settings["Password"] = password;
        }

        return settings;
    }

    /// <summary>
    /// Builds GoogleDrive settings dictionary.
    /// </summary>
    public static Dictionary<string, string> BuildGoogleDriveSettings(string settingsGroup = null)
    {
        var settings = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(settingsGroup))
        {
            settings["SettingsGroup"] = settingsGroup;
        }
        return settings;
    }

    /// <summary>
    /// Validates that a path does not contain traversal sequences.
    /// Returns true if path is safe, false otherwise.
    /// </summary>
    public static bool IsPathSafe(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return true;

        // Reject mixed directory separators
        if (path.Contains('/') && path.Contains('\\'))
            return false;

        // Check for directory traversal
        if (path.Contains("..") || path.Contains("../") || path.Contains("..\\"))
            return false;

        // Check for absolute paths (Windows)
        if (path.Length >= 2 && path[1] == ':' && char.IsLetter(path[0]))
            return false;

        // Check for absolute paths (Unix)
        if (path.StartsWith("/") || path.StartsWith("\\"))
            return false;

        return true;
    }

    /// <summary>
    /// Ensures a path is contained within a base path (prevents escaping).
    /// </summary>
    public static bool IsPathContainedIn(string fullPath, string basePath)
    {
        if (string.IsNullOrWhiteSpace(fullPath) || string.IsNullOrWhiteSpace(basePath))
            return false;

        var normalizedFull = Path.GetFullPath(fullPath);
        var normalizedBase = Path.GetFullPath(basePath);

        return normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
    }
}
