using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Logger;
public class EleonsoftLoggerFactory : ILoggerFactory
{
  private readonly EleonsoftLoggerProvider _eleonsoftLoggerProvider;
  private readonly List<ILoggerProvider> _providers = new List<ILoggerProvider>();

  public EleonsoftLoggerFactory() : this(null)
  {
  }

  public EleonsoftLoggerFactory(EleonsoftLoggerProvider? eleonsoftLoggerProvider)
  {
    _eleonsoftLoggerProvider = eleonsoftLoggerProvider ?? new EleonsoftLoggerProvider();
  }

  public void AddProvider(ILoggerProvider provider)
  {
    _providers.Add(provider);
  }

  public ILogger CreateLogger(string categoryName)
  {
    return _eleonsoftLoggerProvider.CreateLogger(categoryName);
  }

  public void Dispose()
  {
    _eleonsoftLoggerProvider.Dispose();
    _providers.ForEach(p => p.Dispose());
  }

  private static EleonsoftLoggerFactory _instance = null!;
  public static ILoggerFactory Instance => _instance ??= new EleonsoftLoggerFactory();
  public static void Configure(EleonsoftLoggerProvider eleonsoftLoggerProvider)
  {
    _instance = new EleonsoftLoggerFactory(eleonsoftLoggerProvider);
  }
}
