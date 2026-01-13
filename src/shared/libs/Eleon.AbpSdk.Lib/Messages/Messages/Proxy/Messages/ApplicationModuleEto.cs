using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.ETO
{
  public class ApplicationModuleEto
  {
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string PluginName { get; set; }
    public Guid? ParentId { get; set; }
    public UiModuleLoadLevel LoadLevel { get; set; }
    public int OrderIndex { get; set; }
    public string Expose { get; set; }
    public List<ApplicationPropertyEto> Properties { get; set; }
    public Guid ClientApplicationEntityId { get; set; }
  }
}
