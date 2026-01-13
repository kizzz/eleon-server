using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Core.Infrastructure.Module.Entities;

namespace VPortal.Core.Infrastructure.Module.Specifications
{
  public class FeatureSettingsByTenantSpecification : Specification<FeatureSettingEntity>
  {
    Guid? _tenant;

    public FeatureSettingsByTenantSpecification(Guid? tenant)
    {
      this._tenant = tenant;
    }

    public override Expression<Func<FeatureSettingEntity, bool>> ToExpression()
    {
      return (featureSetting) => (featureSetting.TenantId == this._tenant);
    }

  }
}
