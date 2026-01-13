using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain;

public class CreateBulkFeaturesForMicroserviceRequestMsg
{
  public string SourceId { get; set; }

  public List<FeatureDefinitionEto> Features { get; set; }

  public List<FeatureGroupDefinitionEto> Groups { get; set; }
}
public class CreateFeatureGroupDefinitionRequestMsg
{
  public FeatureGroupDefinitionEto Group { get; set; }
  public Guid? ServiceId { get; set; }
}

public class DeleteFeatureRequestMsg
{
  public string Name { get; set; }
}

public class DeleteFeatureGroupRequestMsg
{
  public string Name { get; set; }
}

public class UpdateFeatureDefinitionRequestMsg
{
  public FeatureDefinitionEto Feature { get; set; }
}

public class UpdateFeatureGroupDefinitionRequestMsg
{
  public FeatureGroupDefinitionEto Group { get; set; }
}

public class FeatureGroupDefinitionEto
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string DisplayName { get; set; }
  public string CategoryName { get; set; }
  public bool IsDynamic { get; set; }
  public string SourceId { get; set; }
}

public class CreateFeatureDefinitionRequestMsg
{
  public FeatureDefinitionEto Feature { get; set; }
}

public class FeaturesServiceResponseMsg<T>
{
  public bool Success { get; set; }
  public T Result { get; set; }
}

public class GetFeaturesGroupsQueryMsg { }

public class GetFeaturesQueryMsg { }

public class FeatureDefinitionEto
{
  public Guid Id { get; set; }

  public string GroupName { get; set; }

  public string Name { get; set; }

  public string ParentName { get; set; }

  public string DisplayName { get; set; }

  public string Description { get; set; }

  public string DefaultValue { get; set; }

  public bool IsVisibleToClients { get; set; }

  public bool IsAvailableToHost { get; set; }

  public string AllowedProviders { get; set; }
  public string ValueType { get; set; }

  public bool IsDynamic { get; set; }

  public string SourceId { get; set; }
}