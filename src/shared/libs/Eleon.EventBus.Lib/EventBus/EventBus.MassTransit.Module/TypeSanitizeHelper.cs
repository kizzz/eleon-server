using System.Text.RegularExpressions;

namespace Common.Module.Helpers
{
  public class TypeSanitizeHelper
  {
    public static string SanitizeEventTypeName(string eventTypeName)
    {
      if (string.IsNullOrWhiteSpace(eventTypeName))
      {
        throw new ArgumentException("Event type name cannot be null or whitespace.", nameof(eventTypeName));
      }

      // Convert to lowercase to standardize queue names
      eventTypeName = eventTypeName.ToLowerInvariant();

      // Replace spaces with hyphens
      eventTypeName = eventTypeName.Replace(" ", "-");

      // Remove any characters that are not alphanumeric, hyphen, or underscore
      eventTypeName = Regex.Replace(eventTypeName, @"[^a-z0-9-_]", string.Empty);

      // Truncate to a maximum length (e.g., 50 characters), if needed by your queueing system
      int maxLength = 50;
      if (eventTypeName.Length > maxLength)
      {
        eventTypeName = eventTypeName.Substring(0, maxLength);
      }

      return eventTypeName;
    }
  }
}
