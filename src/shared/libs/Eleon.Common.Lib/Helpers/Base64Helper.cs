using System.Text.RegularExpressions;

namespace Common.Module.Helpers
{
  public partial class Base64Helper
  {
    [GeneratedRegex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$")]
    private static partial Regex Base64Regex();

    public static bool IsValidBase64(string stringToTest) => Base64Regex().IsMatch(stringToTest);

    public static void EnsureValidBase64(string stringToTest)
    {
      if (!IsValidBase64(stringToTest))
      {
        throw new Exception("The string is not a valid base64 string!");
      }
    }

    public static int GetBase64BytesCount(string base64String)
    {
      // Count '=' characters for padding
      int paddingCount = base64String.EndsWith("==") ? 2 : base64String.EndsWith('=') ? 1 : 0;

      // Adjusted length of the base64 string without padding
      int adjustedLength = base64String.Length - paddingCount;

      // Calculate the original size
      int originalSize = (int)(adjustedLength * 3 / 4);

      return originalSize;
    }
  }
}
