using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Core.Infrastructure.Module.Entities;

namespace VPortal.Core.Infrastructure.Module.Specifications
{
  public class FeatureSettingsByGroupSpecification : Specification<FeatureSettingEntity>
  {
    string _group;

    public FeatureSettingsByGroupSpecification(string group)
    {
      this._group = group;
    }

    public override Expression<Func<FeatureSettingEntity, bool>> ToExpression()
    {
      return (featureSetting) => (featureSetting.Group == this._group);
    }

  }
}
