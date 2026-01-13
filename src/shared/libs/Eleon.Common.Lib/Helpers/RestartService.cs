using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharedModule.modules.Helpers.Module;

public class RestartService
{
  private readonly IHostApplicationLifetime _lifetime;
  private readonly IWebHostEnvironment _env;

  public RestartService(IHostApplicationLifetime lifetime, IWebHostEnvironment env)
  {
    _lifetime = lifetime;
    _env = env;
  }

  public virtual void Restart()
  {
    if (_env.IsDevelopment())
    {
      Console.WriteLine("Restart requested (dev mode)...");
    }

    // Platform detection
    if (OperatingSystem.IsWindows())
    {
      // On IIS, touching web.config triggers restart
      var configPath = Path.Combine(AppContext.BaseDirectory, "web.config");
      if (File.Exists(configPath))
        File.SetLastWriteTimeUtc(configPath, DateTime.UtcNow);
    }

    // Graceful shutdown
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      _lifetime.StopApplication();

      // As a fallback, force exit
      await Task.Delay(3000);
      Environment.Exit(0);
    });
  }
}
