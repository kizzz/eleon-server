using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Contracts;

public interface ISystemLogEnricher
{
  void Enrich(Dictionary<string, string> entry);
}
