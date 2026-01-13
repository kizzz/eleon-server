using Common.Module.Constants;
using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Lifecycle.Feature.Module.Reporting;

namespace VPortal.Lifecycle.Feature.Module.Specifications
{
  public class ActionRequiredSpecification<T>
      : Specification<T>
      where T : LifecycleReport
  {

    public ActionRequiredSpecification()
    {
    }

    public override Expression<Func<T, bool>> ToExpression()
        => (report) =>
        report.LifecycleGroupStatus == LifecycleStatus.Enroute
        && report.LifecycleStateStatus == LifecycleStatus.Enroute
        && report.LifecycleActorStatus == LifecycleActorStatus.Enroute
        && report.StateOrderIndex == report.CurrentStateOrderIndex
        && (
            (report.ApprovalType == LifecycleApprovalType.Regular
            && report.ActorOrderIndex == report.CurrentActorOrderIndex)
            || report.ApprovalType != LifecycleApprovalType.Regular);
  }
}
