namespace Eleoncore.SDK.CoreEvents;

public static class TimeSpanExtensions
{
  public static string ToCronExpression(this TimeSpan timeSpan)
  {
    if (timeSpan.TotalSeconds < 60)
      return $"*/{Math.Max(1, (int)timeSpan.TotalSeconds)} * * * * *";

    if (timeSpan.TotalMinutes < 60)
      return $"*/{Math.Max(1, (int)timeSpan.TotalMinutes)} * * * *";

    if (timeSpan.TotalHours < 24)
      return $"{timeSpan.Minutes} */{(int)timeSpan.TotalHours} * * *";

    return $"{timeSpan.Minutes} {timeSpan.Hours} */{(int)timeSpan.TotalDays} * *";
  }
}
