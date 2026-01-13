using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Core.Infrastructure.Module.Entities;

namespace VPortal.Core.Infrastructure.Module.Specifications
{
  public class FeatureSettingsByKeySpecification : Specification<FeatureSettingEntity>
  {
    string _key;

    public FeatureSettingsByKeySpecification(string key)
    {
      this._key = key;
    }

    public override Expression<Func<FeatureSettingEntity, bool>> ToExpression()
    {
      return (featureSetting) => (featureSetting.Key == this._key);
    }

  }
}
