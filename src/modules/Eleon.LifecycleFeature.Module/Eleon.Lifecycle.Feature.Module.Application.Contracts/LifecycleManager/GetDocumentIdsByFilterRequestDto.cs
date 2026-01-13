using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.LifecycleFeatureModule.Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
public class GetDocumentIdsByFilterRequestDto
{
  public string DocumentObjectType { get; set; }
  public Guid? UserId { get; set; }
  public List<string> Roles { get; set; }
  public List<LifecycleStatus> LifecycleStatuses { get; set; }
}
