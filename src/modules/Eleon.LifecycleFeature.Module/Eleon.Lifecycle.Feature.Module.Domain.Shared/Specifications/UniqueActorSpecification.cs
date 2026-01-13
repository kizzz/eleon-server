using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Lifecycle.Feature.Module.Reporting;

namespace VPortal.Lifecycle.Feature.Module.Specifications
{
  public class UniqueActorSpecification<T>
      : Specification<T>
      where T : LifecycleReport
  {
    public override Expression<Func<T, bool>> ToExpression()
        => (report) => report.IsUnique;
  }
}
