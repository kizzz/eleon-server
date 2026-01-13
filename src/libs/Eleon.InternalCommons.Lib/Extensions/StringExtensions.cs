namespace Common.Module.Extensions
{
  public static class StringExtensions
  {
    public static bool NonEmpty(this string str) => !string.IsNullOrWhiteSpace(str);
  }
}
