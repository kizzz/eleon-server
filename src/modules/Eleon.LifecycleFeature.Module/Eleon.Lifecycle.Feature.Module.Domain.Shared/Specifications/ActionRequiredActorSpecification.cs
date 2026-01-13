using System;
using System.Collections.Generic;
using Volo.Abp.Specifications;
using VPortal.Lifecycle.Feature.Module.Reporting;

namespace VPortal.Lifecycle.Feature.Module.Specifications
{
  public class ActionRequiredActorSpecification<T>
      : AndSpecification<T>
      where T : LifecycleReport
  {
    public ActionRequiredActorSpecification(Guid userId, IEnumerable<string> roles)
        : base(
              new ActionRequiredSpecification<T>(),
              new ActorSpecification<T>(userId, roles))
    {
    }
  }
}
