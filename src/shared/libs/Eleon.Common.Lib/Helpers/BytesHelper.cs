namespace Common.Module.Helpers
{
  public enum SizeUnits
  {
    B, KB, MB, GB, TB, PB, EB, ZB, YB
  }

  public static class BytesHelper
  {
    public static string ToString(long bytes, SizeUnits unit)
    {
      return GetSize(bytes, unit).ToString("0.00");
    }

    public static long GetBytes(double value, SizeUnits unit)
    {
      return (long)(value * Math.Pow(1024, (long)unit));
    }

    public static double GetSize(long bytes, SizeUnits unit)
    {
      return bytes / (double)Math.Pow(1024, (long)unit);
    }
  }
}
