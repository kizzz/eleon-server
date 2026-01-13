using DeviceDetectorNET;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VPortal.Identity.Module.Sessions;

public class DeviceInfo
{
  public string OsName { get; }
  public string OsVersion { get; }
  public string DeviceName { get; }
  public string BrowserName { get; }
  public bool IsMobile { get; }

  public DeviceInfo(string osName, string osVersion, string deviceName, string browserName, bool isMobile)
  {
    OsName = osName;
    OsVersion = osVersion;
    DeviceName = deviceName;
    BrowserName = browserName;
    IsMobile = isMobile;
  }

  public override string ToString()
  {
    return $"OsName: {OsName}, OsVersion: {OsVersion}, DeviceName: {DeviceName}, BrowserName: {BrowserName}, IsMobile: {IsMobile}";
  }
}

public static class ParseSessionHelper
{
  private static DeviceInfo DeviceNotDetected => new DeviceInfo(
                  osName: string.Empty,
                  osVersion: string.Empty,
                  deviceName: "DEVICE_NOT_DETECTED",
                  browserName: string.Empty,
                  isMobile: false
                  );

  public static DeviceInfo GetDeviceInfo(string userAgent)
  {
    try
    {
      if (!string.IsNullOrEmpty(userAgent))
      {
        var deviceDetector = new DeviceDetector(userAgent);
        deviceDetector.Parse();

        var browserClient = deviceDetector.GetBrowserClient();
        var os = deviceDetector.GetOs();
        var browser = deviceDetector.GetClient();

        var model = deviceDetector.GetModel();
        var device = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(deviceDetector.GetDeviceName()?.ToLower() ?? string.Empty);

        var deviceName = string.IsNullOrEmpty(model) ? device : $"{model} {device}";

        return new DeviceInfo(
            osName: os.Match?.Name ?? string.Empty,
            osVersion: os.Match?.Version ?? string.Empty,
            deviceName: deviceName,
            browserName: browser.Match?.Name ?? string.Empty,
            isMobile: deviceDetector.IsMobile()
            );
      }
      return DeviceNotDetected;
    }
    catch (Exception)
    {
      return DeviceNotDetected;
    }
  }

  public static string GenerateSessionId(HttpContext context)
  {
    var device = GetDeviceInfo(context.Request.Headers.UserAgent.FirstOrDefault());
    var ip = GetIpAddress(context);
    try
    {
      var data = $"{device.OsName}-{device.DeviceName}-{device.BrowserName}-{device.IsMobile}-{ip}";
      var hashBytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(data));
      return Convert.ToHexString(hashBytes);
    }
    catch (Exception)
    {
      return string.Empty;
    }
  }

  public static string GetIpAddress(HttpContext httpContext)
  {
    return httpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
  }
}
