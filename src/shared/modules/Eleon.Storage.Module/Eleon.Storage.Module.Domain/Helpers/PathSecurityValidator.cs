using System;
using System.IO;

namespace VPortal.Storage.Module.Helpers
{
  /// <summary>
  /// Validates and normalizes file paths to prevent directory traversal attacks.
  /// </summary>
  public static class PathSecurityValidator
  {
    /// <summary>
    /// Normalizes and validates a relative path against a base path to prevent directory traversal.
    /// </summary>
    /// <param name="basePath">The base directory path</param>
    /// <param name="relativePath">The relative path to validate</param>
    /// <returns>The normalized full path if valid</returns>
    /// <exception cref="ArgumentException">Thrown if path is invalid or attempts traversal</exception>
    public static string NormalizeAndValidate(string basePath, string relativePath)
    {
      if (string.IsNullOrWhiteSpace(basePath))
        throw new ArgumentException("Base path cannot be null or empty", nameof(basePath));

      if (string.IsNullOrWhiteSpace(relativePath))
        throw new ArgumentException("Relative path cannot be null or empty", nameof(relativePath));

      // Reject rooted paths (absolute paths)
      if (Path.IsPathRooted(relativePath))
        throw new ArgumentException("Blob name cannot be a rooted path", nameof(relativePath));

      // Reject parent directory references
      if (relativePath.Contains(".."))
        throw new ArgumentException("Blob name cannot contain parent directory references (..)", nameof(relativePath));

      // Combine paths
      var combined = Path.Combine(basePath, relativePath);
      
      // Get full paths for comparison (normalizes separators and resolves .)
      var fullCombined = Path.GetFullPath(combined);
      var fullBase = Path.GetFullPath(basePath);

      // Verify combined path is within base path (case-insensitive on Windows)
      if (!fullCombined.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Blob name resolves outside base path", nameof(relativePath));

      return fullCombined;
    }

    /// <summary>
    /// Validates a remote path (e.g., SFTP) against a base path using string comparison.
    /// For remote paths, we can't use Path.GetFullPath, so we use simpler validation.
    /// </summary>
    /// <param name="basePath">The base directory path (remote, using forward slashes)</param>
    /// <param name="relativePath">The relative path to validate</param>
    /// <returns>The normalized combined path if valid</returns>
    /// <exception cref="ArgumentException">Thrown if path is invalid or attempts traversal</exception>
    public static string NormalizeAndValidateRemote(string basePath, string relativePath)
    {
      if (string.IsNullOrWhiteSpace(basePath))
        throw new ArgumentException("Base path cannot be null or empty", nameof(basePath));

      if (string.IsNullOrWhiteSpace(relativePath))
        throw new ArgumentException("Relative path cannot be null or empty", nameof(relativePath));

      // Normalize separators
      basePath = basePath.Replace("\\", "/").TrimEnd('/');
      relativePath = relativePath.Replace("\\", "/");

      // Reject absolute paths (starting with /)
      if (relativePath.StartsWith("/"))
        throw new ArgumentException("Blob name cannot be an absolute path", nameof(relativePath));

      // Reject parent directory references
      if (relativePath.Contains(".."))
        throw new ArgumentException("Blob name cannot contain parent directory references (..)", nameof(relativePath));

      // Combine paths
      var combined = basePath + "/" + relativePath;
      
      // Normalize: remove consecutive slashes and resolve . references
      combined = combined.Replace("//", "/").Replace("/./", "/");
      
      // Verify combined path starts with base path
      if (!combined.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Blob name resolves outside base path", nameof(relativePath));

      return combined;
    }
  }
}
