using System;
using System.Collections.Generic;
using Volo.Abp.Specifications;
using VPortal.Lifecycle.Feature.Module.Reporting;

namespace VPortal.Lifecycle.Feature.Module.Specifications
{
  public class ArchiveActorSpecification<T>
      : AndSpecification<T>
      where T : LifecycleReport
  {
    public ArchiveActorSpecification(Guid userId, IEnumerable<string> roles)
        : base(
              new ArchiveSpecification<T>(),
              new ActorSpecification<T>(userId, roles))
    {
    }
  }
}
