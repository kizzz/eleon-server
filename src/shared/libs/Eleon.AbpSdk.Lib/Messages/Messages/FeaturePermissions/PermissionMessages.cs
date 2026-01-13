using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
public class PermissionDefinitionEto
{
  public Guid Id { get; set; }

  public string GroupName { get; set; }

  public string Name { get; set; }

  public string ParentName { get; set; }

  public string DisplayName { get; set; }

  public bool IsEnabled { get; set; }

  public MultiTenancySides MultiTenancySide { get; set; }

  public string Providers { get; set; }

  public string StateCheckers { get; set; }

  public int Order { get; set; }
  public bool Dynamic { get; set; }

  public string SourceId { get; set; }
}

public class PermissionGroupDefinitionEto
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string DisplayName { get; set; }
  public string CategoryName { get; set; }
  public int? Order { get; set; }
  public bool Dynamic { get; set; }
  public string SourceId { get; set; }
}

public class CreateBulkPermissionsForMicroserviceRequestMsg
{
  public string SourceId { get; set; }

  public List<PermissionDefinitionEto> Permissions { get; set; }

  public List<PermissionGroupDefinitionEto> Groups { get; set; }
}

public class CreatePermissionRequestMsg
{
  public PermissionDefinitionEto Permission { get; set; }
}

public class CreatePermissionGroupRequestMsg
{
  public PermissionGroupDefinitionEto Group { get; set; }
  public Guid? ServiceId { get; set; }
}

public class UpdatePermissionRequestMsg
{
  public PermissionDefinitionEto Permission { get; set; }
}

public class UpdatePermissionGroupRequestMsg
{
  public PermissionGroupDefinitionEto Group { get; set; }
}

public class DeletePermissionRequestMsg
{
  public string Name { get; set; }
}

public class DeletePermissionGroupRequestMsg
{
  public string Name { get; set; }
}

public class PermissionServiceResponseMsg<T>
{
  public bool Success { get; set; }
  public T Result { get; set; }
}

public class GetPermissionGroupsQueryMsg { }

public class GetPermissionsQueryMsg { }