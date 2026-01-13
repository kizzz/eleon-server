namespace Common.Module.Extensions
{
  public static class GuidExtensions
  {
    private static readonly string tempMarker = "00000000";
    public static bool IsTempGuid(this Guid guid)
    {
      return guid.ToString().StartsWith(tempMarker);
    }

    public static bool IsNotNullOrDefault(this Guid? guid)
        => guid != null && guid != Guid.Empty;
  }
}
